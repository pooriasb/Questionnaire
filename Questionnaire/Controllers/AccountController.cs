using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using QuestionnaireProject.Controllers.API;
using QuestionnaireProject.Extensions;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using QuestionnaireProject.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace QuestionnaireProject.Controllers
{

    [System.Web.Mvc.Authorize]
    public class AccountController : Controller
    {

        private readonly QuestionnaireEntities _context;
        private readonly NotificationRepository _notification;

        public AccountController()
        {
            _context = new QuestionnaireEntities();
            _notification = new NotificationRepository(_context);
        }

        private CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // POST: /Account/Login
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl, string LoginAs)
        {
           // CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);

            //if (!response.Success || !ModelState.IsValid)
            //{
            //    ModelState.AddModelError(string.Empty, "اصالت کاربری شما تایید نشد.");
            //    return View(model);
            //}

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            var result = SignInManager.PasswordSignIn(model.NationalId, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = UserManager.FindByNameAsync(model.NationalId);
                    var role =  UserManager.GetRoles(user.Result.Id);
                    if ( UserManager.IsPhoneNumberConfirmed(user.Result.Id) == false)
                    {
                        return RedirectToAction("SendPhoneNumberCode");
                    }
                    //switch (role[0])
                    //{
                    //    case "Responder":
                    //        return RedirectToAction("Index", "Responder", new { area = "Responder" });
                    //    //case "Questioner":
                    //    //    return RedirectToAction("Index", "Questioner", new { area = "Questioner" });
                    //    case "Editor":
                    //        return RedirectToAction("Index", "Editor", new { area = "Editor" });
                    //    default:
                    //        return RedirectToAction("Index", "Responder", new { area = "Responder" });
                    //}
                    if (LoginAs == "q" && role.Contains("Questioner"))
                    {
                        return RedirectToAction("Index", "Questioner", new { area = "Questioner" });
                    } else if (LoginAs == "r" && role.Contains("Responder")) {
                        return RedirectToAction("Index", "Responder", new { area = "Responder" });
                    }
                    else if(LoginAs == "e" && role.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Editor", new { area = "Editor" });

                    }
                    else if (LoginAs == "e" && role.Contains("Editor"))
                    {
                        return RedirectToAction("Index", "Editor", new { area = "Editor" });

                    }
                    else
                    {
                        TempData["Error"] = "شما به این بخش دسترسی ندارید";

                        return RedirectToAction("login");

                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه وارد شده است.");
                    return View(model);
            }
        }





        //
        // GET: /Account/VerifyCode
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        public class CustomUserValidator<TUser> : IIdentityValidator<TUser>
            where TUser : class, Microsoft.AspNet.Identity.IUser
        {
            private readonly UserManager<TUser> _userManager;
            private readonly string _email;

            public CustomUserValidator(UserManager<TUser> manager, string email)
            {
                _userManager = manager;
                _email = email;
            }

            public async Task<IdentityResult> ValidateAsync(TUser user)
            {
                var errors = new List<string>();


                if (_userManager != null)
                {
                    //check username availability. and add a custom error message to the returned errors list.
                    var existingNationalId = await _userManager.FindByNameAsync(user.UserName);
                    if (existingNationalId != null && existingNationalId.Id != user.Id)
                        errors.Add(" کد ملی درج شده، تکراری میباشد.");
                    var existingEmail = await _userManager.FindByEmailAsync(_email);
                    if (existingEmail != null && existingEmail.Id != user.Id)
                        errors.Add("ایمیل درج شده، تکراری میباشد.");



                }

                //set the returned result (pass/fail) which can be read via the Identity Result returned from the CreateUserAsync
                return errors.Any()
                    ? IdentityResult.Failed(errors.ToArray())
                    : IdentityResult.Success;
            }
        }

        //
        // GET: /Account/Register
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Register(string email)
        {
            var model = new RegisterViewModel()
            {
                ReagentEmail = email
            };
            return View(model);
        }

        //
        // POST: /Account/Register
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);

            if (!response.Success || !ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "اصالت کاربری شما تایید نشد.");
                return View(model);
            }
            if (ModelState.IsValid || response.Success)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    National_Id = model.NationalCode,
                    PhoneNumber = model.PhoneNumber,
                    StudyFieldId = model.StudyFieldId,
                    CityId = model.CityId,
                    Sex = model.Sex,
                    UserName = model.NationalCode,
                    BirthDate = model.BirthDate,                    
                    Email = model.Email,
                    HasPaymentRequest = false,
                    BankId = null, 
                    UniqueCode = (Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"))+ _context.AspNetUsers.Count()).ToString()      
            };

                // todo: check fo reagent email
                UserManager.UserValidator = new CustomUserValidator<ApplicationUser>(UserManager, model.Email);
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    if (model.ReagentEmail != null)
                    {
                        var referer = await UserManager.FindByEmailAsync(model.ReagentEmail);
                        if (referer != null)
                        {
                            RefererCharge refCharge = new RefererCharge()
                            {
                                Referer_Id = referer.Id,
                                CountToPay = 5,
                                NewClient_Id = user.Id,                                
                            };
                            _context.RefererCharges.Add(refCharge);
                            //_context.Discounts.Add(new Discount()
                            //{
                            //    User_Id = reagent.Id,
                            //    Id = Guid.NewGuid().ToString()
                            //});
                        }
                    }
                    _context.SaveChanges();

                    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user.Id, user.PhoneNumber);
                    // Session["verifyCode"] = code;

                    //Session["TimeToResend"] = DateTime.Now;
                    return View("VerifyPhoneNumber");
                }
                AddErrors(result);
            }
            ModelState.AddModelError(string.Empty, "در تکمیل فرم ثبت نام دقت نمایید.");
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> SendPhoneNumberCode()
        {
            var userId = User.Identity.GetUserId();
            var userPhoneNumber = User.Identity.GetPhoneNumber();

            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, userPhoneNumber);
            //Session["verifyCode"] = code;

            return RedirectToAction("VerifyPhoneNumber");
        }

        public class VerifyPhoneNumberViewModel
        {
            [StringLength(11, ErrorMessage = "شماره همراه بایستی 11 رقمی باشد.")]
            [Display(Name ="شماره همراه")]
            public string PhoneNumber { get; set; }

            [Display(Name = "کد فعالسازی")]
            public string Code { get; set; }
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpGet]
        public ActionResult VerifyPhoneNumber()
        {
            //var startTime = Session["TimeToResend"] as DateTime?;

            var model = new VerifyPhoneNumberViewModel()
            {
                Code = null,
                PhoneNumber = User.Identity.GetPhoneNumber(),
            };


            return View(model);
        }


        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);
            if (response.Success == false)
            {
                ModelState.AddModelError(string.Empty, "اصالت کاربری شما تایید نشد.");
                return View(model);
            }
            //var startTime = Session["TimeToResend"] as DateTime?;
            var userName = User.Identity.GetUserName();
            var userId = User.Identity.GetUserId();
            var userPhoneNumber = User.Identity.GetPhoneNumber();

            var result = await UserManager.VerifyChangePhoneNumberTokenAsync(userId, model.Code, userPhoneNumber);

            if (result)
            {
                //var checkResponder = _context.ResponderProfiles.SingleOrDefault(p => p.User_Id == userId);
                //var checkQuestioner = _context.QuestionerProfiles.SingleOrDefault(p => p.User_Id == userId);
                //string role = null;
                //if (checkResponder != null)
                //{
                //    role = "Responder";
                //    await UserManager.AddToRoleAsync(userId, "Responder");

                //}
                //if (checkQuestioner != null)
                //{
                //    role = "Questioner";
                //    await UserManager.AddToRoleAsync(userId, "Questioner");

                //}
                await UserManager.AddToRoleAsync(userId, "Responder");
                UserManager.FindById(userId).PhoneNumberConfirmed = true;
                UserManager.Update(UserManager.FindById(userId));
                await _context.SaveChangesAsync();
                AuthenticationManager.SignOut();
                var user = await UserManager.FindByNameAsync(userName);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return RedirectToAction("Index", "Responder", new { area = "Responder" });
                //if (role == "Responder")
                //{
                //    return RedirectToAction("Index", "Responder", new { area = "Responder" });

                //}
                //else if (role == "Questioner")
                //{
                //    return RedirectToAction("Index", "Questioner", new { area = "Questioner" });
                //}
            }
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        public async Task<ActionResult> ChangePhoneNumber(string phoneNumber)
        {
            var match = System.Text.RegularExpressions.Regex.Match(phoneNumber, @"^0(9)\d{9}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (match.Success)
            {
                var userId = User.Identity.GetUserId();
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, phoneNumber);
                new MySmsService(phoneNumber, "کد شما جهت تغییر شماره همراه: " + code);

                return Json(new { result = true, message = "شماره همراه خود را تایید نمایید.", phoneNumber });
            }
            return Json(new { result = false, message = "لطفا در وارد نمودن شماره دقت نمایید.", phoneNumber });

        }

        [Serializable]
        public class ChangePhoneNumberModel
        {
            public string PhoneNumber { get; set; }
            public string Code { get; set; }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        public async Task<ActionResult> ChangePhoneNumberConfirm(ChangePhoneNumberModel model)
        {

            var userId = User.Identity.GetUserId();
            //var userPhone = User.Identity.GetPhoneNumber();
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, model.PhoneNumber);
            if (model.Code == code)
            {
                await UserManager.SetPhoneNumberAsync(userId, model.PhoneNumber);
                UserManager.FindById(userId).PhoneNumberConfirmed = true;
                UserManager.Update(UserManager.FindById(userId));
                return Json(new { result = true, message = "شماره همراه شما با موفقیت تغییر نمود." });
            }
            return Json(new { result = false, message = "خطایی رخ داده است." });
        }


        //
        // GET: /Account/ConfirmEmail
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.NationalId);
                if (user == null || !(await UserManager.IsPhoneNumberConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Json(new { result = false, message = "کد ملی وارد شده صحیح نیست." });
                }

                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user.Id, user.PhoneNumber);


                MySmsService sms = new MySmsService(user.PhoneNumber, "کد امنیتی: " + code);

                return View("ForgotPasswordConfirmation");
            }


            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public class ForgotPasswordConfirmationModel
        {
            [Required(ErrorMessage = "وارد نمودن کد ملی الزامی میباشد.")]
            [Display(Name = "کد ملی")]
            [StringLength(10, ErrorMessage = "تنها 10 کاراکتر قابل قول است.")]
            public string NationalId { get; set; }

            [Required(ErrorMessage = "وارد نمودن کد امنیتی الزامی میباشد.")]
            [Display(Name = "کد امنیتی")]
            public string Code { get; set; }

            [Required(ErrorMessage = "وارد نمودن رمز عبور الزامی میباشد.")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            public string Password { get; set; }

            [Required(ErrorMessage = "وارد نمودن تکرار رمز عبور الزامی میباشد.")]
            [DataType(DataType.Password)]
            [Display(Name = "تکرار رمز عبور")]
            [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "عدم تطابق رمز عبورهای وارد شده.")]
            public string PasswordConfirm { get; set; }
        }
        //
        // GET: /Account/ForgotPasswordConfirmation
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {

            return View();
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ForgotPasswordConfirmation(ForgotPasswordConfirmationModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.NationalId);
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user.Id, user.PhoneNumber);
                if (model.Code == code)
                {
                    var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var resetPassword = await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
                    await _context.SaveChangesAsync();
                    if (resetPassword.Succeeded)
                    {
                        return View("Login");
                    }

                }

            }
            return View(model);
        }
        //
        // GET: /Account/ResetPassword
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.NationalId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        // POST: /Account/LogOff
        [System.Web.Mvc.HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        // GET: /Account/ExternalLoginFailure
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize(Roles = "Responder")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PromoteUserRole(HttpPostedFileBase nationalCardImage)
        {
            var a = Request.Files;
            var userId = User.Identity.GetUserId();
            var user = _context.AspNetUsers.Find(userId);
            if (user != null)
            {


                if (nationalCardImage != null)
                {
                    if (System.IO.Path.GetExtension(nationalCardImage.FileName)?.ToLower() == ".jpg")
                    {
                        if (nationalCardImage.ContentLength <= 2 * Math.Pow(1024, 2))
                        {
                            byte[] b = new byte[nationalCardImage.ContentLength];
                            await nationalCardImage.InputStream.ReadAsync(b, 0, nationalCardImage.ContentLength);
                            user.NationalCardImage = b;
                            await UserManager.AddToRoleAsync(userId, "Questioner");
                            _notification.Add(new Notification()
                            {
                                DateCreated = DateTime.Now,
                                Title = "ارتقا حساب کاربری",
                                Text = "حسابی کاربری شما ارتقا نموده است. لطفا جهت استفاده از امکانات سایت یک بار از سایت خارج شده و دوباره وارد شوید.",
                                ToUser_Id = userId,
                                IsSeen = false,
                                Type = (int)Enums.NotificationType.Success
                            });
                            _context.SaveChanges();
                            return RedirectToAction("Responder", "Responder", new{area=""});

                        }
                    }
                }
            }

            _notification.Add(new Notification()
            {
                DateCreated = DateTime.Now,
                Title = "ارتقا حساب کاربری",
                Text = "حساب کاربری شما ارتقا نیافته است. در صورت بروز مشکل با پشتیبانی تماس بگیرید.",
                ToUser_Id = userId,
                IsSeen = false,
                Type = (int)Enums.NotificationType.Danger
            });
            _context.SaveChanges();




            return RedirectToAction("Responder", "Responder", new { area = "" });
        }
        #endregion


        #region AndroidRegion

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAndroid(LoginAndroidViewModel model)
        {
            //await LogOffAndroid();
            var appCommand = _context.Options.FirstOrDefault(o => o.Name == "AppCommand").Value;
            var appCommandMessage = _context.Options.FirstOrDefault(o => o.Name == "AppCommandMessage").Value;

            var result = await SignInManager.PasswordSignInAsync(model.NationalId, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.National_Id == model.NationalId);                    
                    var role = user.AspNetRoles.Select(r => r.Name);
                    //if (user.AspNetRoles.FirstOrDefault() != "Responder")
                    //{
                    //    return Json(new { result = false, message = "در ورود شما اختلالی پیش آمده است.", appCommand, appCommandMessage });
                    //}
                    if (await UserManager.IsPhoneNumberConfirmedAsync(user.Id) == false)
                    {
                        var phoneNumber = User.Identity.GetPhoneNumber();
                        var code = UserManager.GenerateChangePhoneNumberTokenAsync(user.Id, phoneNumber).Result;
                        string text = "کد شما جهت تایید شماره همراه: " + code;
                        //todo: uncomment sms
                        MySmsService sms = new MySmsService(phoneNumber, text);


                        return Json(new { result = false, message = "شماره تماس تایید نشده است.", code });
                    }

                    var userData = _context.AspNetUsers.FirstOrDefault(u => u.Id == user.Id);
                    userData.PId = model.PId;
                    _context.SaveChanges();

                    return Json(new { result = true, message = "موفقیت", appCommand, appCommandMessage });
            }
            return Json(new { result = false, message = "در ورود شما اختلالی پیش آمده است.", appCommand, appCommandMessage });

        }

        // POST: /Account/LogOff
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult LogOffAndroid()
        {

            AuthenticationManager.SignOut();
            return Json(new { result = true, message = "خروج با موفقیت صورت گرفت." });
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> RegisterAndroid([FromBody]API.SignUpController.SignUpViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    National_Id = model.NationalId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Sex = model.Sex,
                    StudyFieldId = model.StudyFieldId,
                    CityId = model.CityId,
                    BirthDate = model.BirthDate,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.NationalId,
                    HasPaymentRequest = false,
                    BankId = null,
                    UniqueCode = (int.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) + _context.AspNetUsers.Count()).ToString()
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //todo send sms

                    var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user.Id, user.PhoneNumber);
                    //var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user.Id, user.PhoneNumber);

                    MySmsService sms = new MySmsService(model.PhoneNumber, "کد امنیتی: " + code);
                    return Json(new { result = true });
                }
                return Json(new { result = false });
            }

            return Json(new { result = false });
        }

        

        // POST: /Account/ForgotPassword
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ForgotPasswordAndroid(SignUpController.ForgotPasswordAndroidViewModel model)
        {
            if (!Request.Browser.IsMobileDevice)
            {
                return Json(new { message = "شما دسترسی ندارید." });
            }

            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.NationalId);
                if (user == null || !(await UserManager.IsPhoneNumberConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return Json(new { result = false, message = "کد ملی وارد شده صحیح نیست." });
                }

                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(user.Id, user.PhoneNumber);
                MySmsService sms = new MySmsService(user.PhoneNumber, "کد شما جهت تایید شماره همراه: " + code);

                return Json(new { result = true, message = "پیام کوتاه بازیابی رمز برای شما ارسال گردید." });
            }


            // If we got this far, something failed, redisplay form
            return Json(new { message = "در سیستم اشکالی رخ داده است.", result = false });
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        public async Task<ActionResult> IsPhoneNumberConfirmed()
        {

            var userId = User.Identity.GetUserId();
            var result = await UserManager.IsPhoneNumberConfirmedAsync(userId);
            if (result)
            {
                return Json(new { result = true, message="شماره همراه تایید شده است." });

            }
            return Json(new { result = false,  message = "شماره همراه تایید نشده است." });

        }



        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        public async Task<ActionResult> ChangePhoneNumberAndroid(SignUpController.NewPhoneNumber model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var userPhone = User.Identity.GetPhoneNumber();
                await UserManager.SetPhoneNumberAsync(userId, model.PhoneNumber);
                var code = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, userPhone);

                //todo uncomment sms...
                MySmsService sms = new MySmsService(userPhone, "کد امنیتی:" + code);
                return Json(new { result = true, message = "شماره همراه خود را تایید نمایید." });
            }
            
            return Json(new { result = false, message = "در وارد نمود شماره همراه دقت نمایید."});



        }

        // GET: /Account/ConfirmEmail
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ConfirmPhoneNumberAndroid(string code)
        {

            var userId = User.Identity.GetUserId();
            if (userId == null || code == null)
            {
                return Json(new { result = false, message="کد اشتباه است!" });
            }

            var phoneNumber = User.Identity.GetPhoneNumber();
            var result = await UserManager.VerifyChangePhoneNumberTokenAsync(userId, code, phoneNumber);
            if (result)
            {
                UserManager.FindById(userId).PhoneNumberConfirmed = true;
                UserManager.Update(UserManager.FindById(userId));
                return Json(new { result = true, message="کد صحیح است" });
            }
            return Json(new { result = false, message = "اشکال در سیستم" });

        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public async Task<ActionResult> ResetPasswordAndroid(SignUpController.ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return Json(new { message = "در سیستم اشکالی رخ داده است.", result = false });
            }
            var user = await UserManager.FindByNameAsync(model.NationalId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Json(new { result = false, message = "کد ملی اشتباه است." });
            }
            var result = await UserManager.VerifyChangePhoneNumberTokenAsync(user.Id, model.Code, user.PhoneNumber);

            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result)
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var r2 = await UserManager.ResetPasswordAsync(user.Id, token, model.Password);
                await _context.SaveChangesAsync();
                if (r2.Succeeded)
                {
                    return Json(new { result = true, message = "رمز عبور با موفقیت تغییر نمود." });

                }
                return Json(new { result = false, message = "خطا در تغییر رمز عبور.", /*r2*/ });


            }
            return Json(new { message = "در سیستم اشکالی رخ داده است.", result = false });
        }

        
        
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        public async Task<ActionResult> ChangePhoneNumberConfirmAndroid(ChangePhoneNumberModel model)
        {


            var userId = User.Identity.GetUserId();
            //var userPhone = User.Identity.GetPhoneNumber();
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(userId, model.PhoneNumber);
            if (model.Code == code)
            {
                await UserManager.SetPhoneNumberAsync(userId, model.PhoneNumber);
                UserManager.FindById(userId).PhoneNumberConfirmed = true;
                UserManager.Update(UserManager.FindById(userId));
                return Json(new { result = true, message = "شماره همراه شما با موفقیت تغییر نمود." });
            }
            return Json(new { result = false, message = "خطایی رخ داده است." });
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult UserProfile()
        {

            var userId = User.Identity.GetUserId();
            var user = _context.AspNetUsers.Where(u => u.Id == userId).Select(u => new
            {
                u.National_Id,
                u.FirstName,
                u.LastName,
                BirthDate = u.BirthDate.ToString(),
                u.StudyFieldId,
                u.CityId,
                u.Sex,

            });

            return Json(new { user = user });

        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        public ActionResult ChangeUserProfile(SignUpController.SignUpViewModel model)
        {

            var userId = User.Identity.GetUserId();
            var user = _context.AspNetUsers.Find(userId);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.BirthDate = model.BirthDate;
            //user.National_Id = model.NationalId;
            user.StudyFieldId = model.StudyFieldId;
            user.CityId = model.CityId;
            //_context.SaveChanges();

            return Json(new { result = true, message = "اطلاعات با موفقیت تغییر نمود." });

        }

        
        
        #endregion
    }
}