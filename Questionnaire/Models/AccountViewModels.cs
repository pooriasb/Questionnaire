using Newtonsoft.Json;
using QuestionnaireProject.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestionnaireProject.Models
{
    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success
        {
            get;
            set;
        }
        [JsonProperty("error-codes")]
        public List<string> ErrorMessage
        {
            get;
            set;
        }
    }
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "کد")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "این مرورگر را بخاطر داشته باش؟")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "کد ملی خود را وارد نمایید.")]
        [Display(Name = "کد ملی")]
        [MinLength(10, ErrorMessage = "حداقل 10 کاراکتر قابل قبول است.")]
        [MaxLength(10,ErrorMessage = "حداکثر 10 کاراکتر قابل قبول است.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "تنها وارد نمودن عدد قابل قبول است.")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "رمز خود را وارد نمایید.")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "مرا بخاطر داشته باش؟")]
        public bool RememberMe { get; set; }
    }

    public class LoginAndroidViewModel
    {
        [Required]
        [Display(Name = "کد ملی")]
        [MinLength(10, ErrorMessage = "حداقل 10 کاراکتر قابل قبول است.")]
        [MaxLength(10, ErrorMessage = "حداکثر 10 کاراکتر قابل قبول است.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "تنها وارد نمودن عدد قابل قبول است.")]
        public string NationalId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        public string PId { get; set; }

        [Display(Name = "مرا بخاطر داشته باش؟")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "نام خود را وارد نمایید.")]
        [StringLength(40, ErrorMessage = "حداکثر {0} کاراکتر قابل قبول است.")]
        [Display(Name = "نام")]
        [RegularExpression(@"^[\u0600-\u06FF\s]+$",ErrorMessage = "تنها حروف فارسی قابل قبول است.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "نام خانوادگی خود را وارد نمایید.")]
        [StringLength(40, ErrorMessage = "حداکثر {0} کاراکتر قابل قبول است.")]
        [Display(Name = "نام خانوادگی")]
        [RegularExpression(@"^[\u0600-\u06FF\sB]+$", ErrorMessage = "تنها حروف فارسی قابل قبول است.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "کد ملی خود را وارد نمایید.")]
        [StringLength(10, ErrorMessage = "حداکثر {0} کاراکتر قابل قبول است.")]
        [Display(Name = "کد ملی")]
        [MaxLength(10, ErrorMessage = "حداکثر 10 کاراکتر قابل قبول است.")]
        [MinLength(10, ErrorMessage = "حداقل 10 کاراکتر قابل قبول است.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "تنها استفاده از اعداد قابل قبول میباشد.")]
        [Attributes.AvailableNationalIdAttr(ErrorMessage = "شماره ملی وارد شده موجود میباشد.")]
        public string NationalCode { get; set; }

        [Required(ErrorMessage = "تاریخ تولد خود را وارد نمایید.")]
        [DataType(DataType.Date)]
        [Display(Name = "تاریخ تولد")]
        [DateTimeLesserThanNowAttr(ErrorMessage = "تاریخ تولد بایستی قبل از زمان حاضر باشد.")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "شماره همراه خود را وارد نمایید.")]
        [StringLength(11, ErrorMessage = "حداکثر {0} کاراکتر قابل قبول است.")]
        [Display(Name = "تلفن همراه")]
        [MaxLength(11, ErrorMessage = "حداکثر 11 کاراکتر قابل قبول است.")]
        [MinLength(11, ErrorMessage = "حداقل 11 کاراکتر قابل قبول است.")]
        [RegularExpression(@"^(0)?9\d{9}$", ErrorMessage = "لطفا شماره همراه معتبر وارد نمایید.")]
        [Attributes.AvailablePhoneNumberAttr(ErrorMessage = "شماره همراه وارد شده موجود میباشد.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "رشته تحصیلی خود را انتخاب نمایید.")]
        [Display(Name = "رشته تحصیلی")]
        [Range(0, Int16.MaxValue, ErrorMessage ="رشته مناسب را انتخاب نمایید." )]
        public int StudyFieldId { get; set; }

        [Required(ErrorMessage = "شهر محل سکونت خود را انتخاب نمایید.")]
        [Display(Name = "شهر")]
        [Range(0, Int16.MaxValue, ErrorMessage = "شهر مناسب را انتخاب نمایید.")]
        public int CityId { get; set; }

        [EmailAddress(ErrorMessage = "قالب ایمیل ورودی صحیح نمیباشد.")]
        [Display(Name = "ایمیل معرف")]
        public string ReagentEmail { get; set; }

        [Required(ErrorMessage = "جنسیت خود را انتخاب نمایید.")]
        [Range(0,1,ErrorMessage = "جنسیت را اشتباه وارد نموده اید.")]
        [Display(Name = "جنسیت")]
        public int Sex { get; set; }

        [Required(ErrorMessage = "وارد نمودن ایمیل الزامی میباشد.")]
        [EmailAddress(ErrorMessage = "قالب ایمیل ورودی صحیح نمیباشد.")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد نمودن رمز عبور الزامی میباشد.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور")]
        [Compare("Password", ErrorMessage = "عدم تطابق رمز عبورهای وارد شده.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "کد ملی")]
        public string NationalId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمیز عبور")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        //[MaxLength(11)]
        //[MinLength(11)]
        [MaxLength(10,ErrorMessage ="تنها 10 رقم قابل قبول میباشد.")]
        [Display(Name = "کد ملی")]
        public string NationalId { get; set; }
    }
}