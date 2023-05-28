using QuestionnaireProject.Areas.Questioner.Core.Attributes;
using QuestionnaireProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using QuestionnaireProject.Attributes;

namespace QuestionnaireProject.Areas.Questioner.Models
{
    public class QuestionerRegisterViewModel
    {
        [Required(ErrorMessage = "وارد نمودن نام الزامی است.")]
        [MaxLength(40)]
        [Display(Name = "نام")]
        [RegularExpression(@"^[\u0600-\u06FF]+$", ErrorMessage = "تنها حروف فارسی قابل قبول است.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "وارد نمودن نام خانوادگی الزامی است.")]
        [MaxLength(40)]
        [Display(Name = "نام خانوادگی")]
        [RegularExpression(@"^[\u0600-\u06FF]+$", ErrorMessage = "تنها حروف فارسی قابل قبول است.")]
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

        [Required(ErrorMessage = "وارد نمودن رمز الزامی است.")]
        [DataType(DataType.Password)]
        [Display(Name = "تایید رمز عبور")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "عبارت تایید رمز عبور با رمز عبور وارد شده یکسان نمیباشد.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "جنسیت خود را انتخاب نمایید.")]
        [Range(0, 1, ErrorMessage = "جنسیت را اشتباه وارد نموده اید.")]
        [Display(Name = "جنسیت")]
        public int Sex { get; set; }

        [Required(ErrorMessage = "انتخاب رشته الزامی است.")]
        [Display(Name = "رشته")]
        public int StudyFieldId { get; set; }
        public IEnumerable<StudyField> StudyFields { get; set; }

        [Required(ErrorMessage = "انتخاب استان الزامی است.")]
        [Display(Name = "استان")]
        public int ProvinceId { get; set; }
        public IEnumerable<Province> Provinces { get; set; }


        [Required(ErrorMessage = "انتخاب شهر الزامی است.")]
        [Display(Name = "شهر")]
        public int CityId { get; set; }        
        public IEnumerable<City> Cities { get; set; }

       
        [Required(ErrorMessage = "وارد نمودن تاریخ تولد الزامی است.")]
        [Display(Name = "تاریخ تولد")]
        [DateTimeLesserThanNowAttr(ErrorMessage = "در انتخاب تاریخ تولد دقت نمایید.")]
        public System.DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "وارد نمودن تلفن همراه الزامی است.")]
        [Display(Name = "تلفن همراه")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "وارد نمودن کد ملی الزامی است.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "تنها عدد قابل قبول است.")]
        [Display(Name = "کد ملی")]
        [MinLength(10, ErrorMessage = "حداقل 10 کاراکتر قابل قبول است.")]
        [MaxLength(10, ErrorMessage = "حداکثر 10 کاراکتر قابل قبول است.")]
        public string NationalId { get; set; }


        [Display(Name = "تصویر کارت ملی")]
        [FileExtensionAttr( fileExtension: "jpg", ErrorMessage = "فایل ارسالی بایستی JPG باشد.")]        
        [PictureMaxLengthAttr(pictureMaxLength: 2 * 1024 * 1024, ErrorMessage = "حداکثر فایلی با حجم 2 مگابایت قابل قبول است.")]
        public HttpPostedFileBase NationalCardImage { get; set; }

        [EmailAddress]
        [Display(Name = "ایمیل معرف")]
        public string ReagentEmail { get; set; }

    }


}