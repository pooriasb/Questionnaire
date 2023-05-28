using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using QuestionnaireProject.Areas.Questioner.Models;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

//todo: UnitOfWork Implementation

namespace QuestionnaireProject.Areas.Questioner.Controllers
{
    public class QuestionerAccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private readonly QuestionnaireEntities _context;
        private readonly NotificationRepository _notification;
        //private readonly IUnitOfWork _unitOfWork;

        public QuestionerAccountController()
        {
            _context = new QuestionnaireEntities();
            _notification = new NotificationRepository(_context);
        }

        public ActionResult Index()
        {
            return View();
        }
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

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [AllowAnonymous]
        public ActionResult QuestionerRegister()
        {


            //var roleManager = new RoleManager<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));


            //if (!roleManager.RoleExists("Questioner"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Questioner";
            //    roleManager.Create(role);

            //}


            QuestionerRegisterViewModel model = new QuestionerRegisterViewModel();
            model.BirthDate = DateTime.Now;
            model.StudyFields = _context.StudyFields.ToList();
            model.Provinces = _context.Provinces.ToList();
            model.Cities = _context.Cities.ToList();
            return View(model);
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
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> QuestionerRegister(QuestionerRegisterViewModel model)
        {

            model.StudyFields = _context.StudyFields.ToList();
            model.Provinces = _context.Provinces.ToList();
            model.Cities = _context.Cities.ToList();


            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.NationalId,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    StudyFieldId = model.StudyFieldId,
                    CityId = model.CityId,
                    BirthDate = model.BirthDate,
                    National_Id = model.NationalId,
                    PhoneNumber = model.PhoneNumber
                };
                UserManager.UserValidator = new CustomUserValidator<ApplicationUser>(UserManager, model.Email);
                var result = await UserManager.CreateAsync(user, model.Password);



                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    await UserManager.AddToRoleAsync(user.Id, "Questioner");
                    await UserManager.AddToRoleAsync(user.Id, "Responder");



                    //QuestionerProfile quetionerProfile = new QuestionerProfile();
                    //quetionerProfile.User_Id = user.Id;
                    //if (model.NationalCardImage != null)
                    //{
                    //    if (System.IO.Path.GetExtension(model.NationalCardImage.FileName)?.ToLower() == ".jpg")
                    //    {
                    //        if (model.NationalCardImage.ContentLength <= 2 * Math.Pow(1024, 2))
                    //        {
                    //            byte[] b = new byte[model.NationalCardImage.ContentLength];
                    //            model.NationalCardImage.InputStream.Read(b, 0, model.NationalCardImage.ContentLength);
                    //            quetionerProfile.NationalCardImage = b;
                    //        }
                    //    }
                    //}

                    //Add reagent email for a discount code
                    if (model.ReagentEmail != null)
                    {
                        var reagent = await UserManager.FindByEmailAsync(model.ReagentEmail);
                        if (reagent != null)
                        {
                            var discountCode = Guid.NewGuid().ToString();
                            _context.Discounts.Add(new Discount()
                            {
                                User_Id = reagent.Id,
                                Id = discountCode
                            });
                            _notification.Add(new Notification()
                            {
                                ToUser_Id = reagent.Id,
                                Title = "کد تخفیف",
                                Type = (int) Enums.NotificationType.Warning,
                                Text = "کد تخفیف شما: \n" +discountCode,
                                DateCreated = DateTime.Now,
                                IsSeen = false,                                                               
                            });
                        }
                        
                    }

                    //_context.QuestionerProfiles.Add(quetionerProfile);
                    _context.SaveChanges();

                       return RedirectToAction("SendPhoneNumberCode", "Account", new{area=""});
                   
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Questioner")]
        public ActionResult NationalCardImage()
        {
            try
            {

           
            var userId = User.Identity.GetUserId();
            var user = _context.AspNetUsers.FirstOrDefault(p=>p.Id == userId);
            if (user != null)
            {
                var image = Convert.ToBase64String(user.NationalCardImage);
                return Json(new {image});
            }
            }
            catch (Exception)
            {

            }
                return Json(new { error = "خطایی رخ داده است." });
          
        }

        [HttpPost]
        [Authorize(Roles = "Questioner")]
        [ValidateAntiForgeryToken]
        public ActionResult NationalCard(HttpPostedFileBase file)
        {
            var userId = User.Identity.GetUserId();
            var user = _context.AspNetUsers.FirstOrDefault(p => p.Id == userId);
            if (user!=null)
            {
                if (file != null)
                {
                    if (System.IO.Path.GetExtension(file.FileName)?.ToLower() == ".jpg")
                    {
                        if (file.ContentLength <= 2 * Math.Pow(1024, 2))
                        {
                            byte[] b = new byte[file.ContentLength];
                            file.InputStream.Read(b, 0, file.ContentLength);
                            user.NationalCardImage = b;
                            _context.SaveChanges();
                        }
                    }
                }
            }
           
            return RedirectToAction("Index","Manage", new {area=""});
        }
        
    }


}