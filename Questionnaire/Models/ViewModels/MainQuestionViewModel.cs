using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace QuestionnaireProject.Models.ViewModels
{
    public class MainQuestionViewModel
    {
        [Required(ErrorMessage = "لطفا صورت پرسش را وارد نمایید.")]
        [Display(Name = "صورت پرسش")]
        [MaxLength(255, ErrorMessage = "حداکثر 255 کاراکتر میتوانید به کار برید.")]
        public string QuestionText { get; set; }

        [Display(Name = "پاسخ پرسش")]
        public List<string> Answers { get; set; }

        [Required]
        public Enums.MediaType MediaType { get; set; }

        public string MediaUrl { get; set; }

        public HttpPostedFileBase PictureFile { get; set; }
    }
}