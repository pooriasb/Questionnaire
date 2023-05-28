using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly QuestionnaireEntities _context;
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
        public AdminController()
        {
            _context = new QuestionnaireEntities();
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddNewEditor() {
            //var users = _context.AspNetUsers.Select(x => new
            //{
            //    x.National_Id,
            //    x.FirstName,
            //    x.LastName,
            //    x.Email,
            //    x.PhoneNumber,
            //    x.LockoutEndDateUtc
                
            //});
            var roleManager =
  new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

      var usersID =      roleManager.FindByName("editor").Users.ToList().Select(x=>x.UserId);
            var dtos = new List<UsersDto>();
            foreach (var id in usersID)
            {
                var FindedUser = _context.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
                dtos.Add(new UsersDto()
                {
                    Email = FindedUser.Email,
                    FirstName = FindedUser.FirstName,
                    IsBlocked = FindedUser.LockoutEndDateUtc
                    , LastName = FindedUser.LastName,
                    NationalId = FindedUser.National_Id
                    ,PhoneNumber = FindedUser.PhoneNumber
                });
            }
            ViewBag.users = dtos;

            return View();
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
        [HttpPost]
        public ActionResult AddNewEditor(RegisterViewModel model) {
            try
            {

           
            UserManager.UserValidator = new CustomUserValidator<ApplicationUser>(UserManager, model.Email);
            var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    National_Id = model.NationalCode,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.NationalCode,
                    Email = model.Email,
                    HasPaymentRequest = false,
                    BankId = null, 
                    UniqueCode = (Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"))+ _context.AspNetUsers.Count()).ToString()      
            };
            var result =  UserManager.Create(user, model.Password);
            if (result.Succeeded)
            {
                UserManager.AddToRole(user.Id, "Editor");

            }
            _context.SaveChanges();
            //var roleStore = new RoleStore<IdentityRole>(_context);
            //var roleManager = new RoleManager<IdentityRole>(roleStore);
             UserManager.AddToRole(user.Id, "Editor");
            UserManager.FindById(user.Id).PhoneNumberConfirmed = true;
            UserManager.Update(UserManager.FindById(user.Id));
            _context.SaveChanges();
            TempData["ok"] = "ویراستار جدید اضافه شد";
            }
            catch (Exception)
            {

                TempData["Error"] = "کد ملی یا ایمیل تکراری میباشد لطفا دوباره تلاش کنید.";
            }
            return View();
        }
        public ActionResult BlockUser(string nationalId)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.National_Id == nationalId);
            if (user != null)
            {
                user.LockoutEndDateUtc = DateTime.MaxValue;
                _context.SaveChanges();
            }
            return RedirectToAction("AddNewEditor");
        }

        public ActionResult UnblockUser(string nationalId)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.National_Id == nationalId);
            if (user != null)
            {
                user.LockoutEndDateUtc = null;
                _context.SaveChanges();
            }
            return RedirectToAction("AddNewEditor");
        }
    }
}