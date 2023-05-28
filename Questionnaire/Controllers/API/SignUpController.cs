using QuestionnaireProject.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;

namespace QuestionnaireProject.Controllers.API
{
    public class SignUpController : ApiController
    {
        private readonly QuestionnaireEntities _context;

        public SignUpController()
        {
            _context = new QuestionnaireEntities();
        }

        [HttpPost]
        public IHttpActionResult ProvincesAndFields()
        {
            var provinces = _context.Provinces.Select(p => new
            {
                p.Id,
                p.Name
            }).ToList();
            var fields = _context.StudyFields.Select(c => new
            {
                c.Id,
                c.Name
            });

            return Json(new { provinces = provinces , fields = fields });
        }

        [Serializable]
        public class Provience
        {
            public int Id { get; set; }
        }

        [HttpPost]
        public IHttpActionResult Cities([FromBody]Province province)
        {
            var cities = _context.Cities.Where(c => c.Province_Id == province.Id).Select(c => new
            {
                c.Id,
                c.Name
            });

            return Json(new { cities = cities });
        }

       
        public class SignUpViewModel
        {
            [Required(ErrorMessage = "وارد نمودن نام الزامی است.")]
            [MaxLength(40)]
            [Display(Name = "نام")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "وارد نمودن نام خانوادگی الزامی است.")]
            [MaxLength(40)]
            [Display(Name = "نام خانوادگی")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "وارد نمودن ایمیل الزامی است.")]
            [EmailAddress(ErrorMessage = "ایمیل را بدرستی وارد نمایید.")]
            [Display(Name = "ایمیل")]
            public string Email { get; set; }

            [Required(ErrorMessage = "وارد نمودن رمز الزامی است.")]
            [StringLength(100, ErrorMessage = " {0} بایستی حداقل شامل {2}حرف باشد.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            public string Password { get; set; }

            [Required]
            [Display(Name = "جنسیت")]
            //todo: add this to view
            public int Sex { get; set; }


            [Required(ErrorMessage = "انتخاب رشته الزامی است.")]
            [Display(Name = "رشته")]
            public int StudyFieldId { get; set; }

            


            [Required(ErrorMessage = "انتخاب شهر الزامی است.")]
            [Display(Name = "شهر")]
            public int CityId { get; set; }


            [Required(ErrorMessage = "وارد نمودن تاریخ تولد الزامی است.")]
            [Display(Name = "تاریخ تولد")]
            public DateTime BirthDate { get; set; }

            [Required(ErrorMessage = "وارد نمودن تلفن همراه الزامی است.")]
            [Display(Name = "تلفن همراه")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "وارد نمودن کد ملی الزامی است.")]
            [RegularExpression("^[0-9]*$", ErrorMessage = "تنها عدد قابل قبول است.")]
            [Display(Name = "کد ملی")]
            public string NationalId { get; set; }


            

        }


        public class ResetPasswordViewModel
        {
            [Required]
            [Display(Name = "کد ملی")]
            public string NationalId { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "رمز عبور")]
            public string Password { get; set; }            

            public string Code { get; set; }
        }



        public class ForgotPasswordAndroidViewModel
        {
            [Required]
            //[MaxLength(11)]
            //[MinLength(11)]
            [Display(Name = "کد ملی")]
            public string NationalId { get; set; }
        }

        public class NewPhoneNumber
        {
            [Required(ErrorMessage = "شماره همراه خود را وارد نمایید.")]
            [StringLength(11, ErrorMessage = "حداکثر {0} کاراکتر قابل قبول است.")]
            [Display(Name = "تلفن همراه")]
            [MaxLength(11, ErrorMessage = "حداکثر 11 کاراکتر قابل قبول است.")]
            [MinLength(11, ErrorMessage = "حداقل 11 کاراکتر قابل قبول است.")]
            [RegularExpression(@"^(0)?9\d{9}$", ErrorMessage = "لطفا شماره همراه معتبر وارد نمایید.")]
            [Attributes.AvailablePhoneNumberAttr(ErrorMessage = "شماره همراه وارد شده موجود میباشد.")]
            public string PhoneNumber { get; set; }
        }
    }
}
