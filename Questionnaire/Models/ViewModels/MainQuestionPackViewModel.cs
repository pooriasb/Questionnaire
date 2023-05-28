using QuestionnaireProject.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace QuestionnaireProject.Models.ViewModels
{
    public class MainQuestionPackViewModel
    {
        public string QuestionId { get; set; }

        [Required(ErrorMessage = "وارد نمودن صورت پرسش الزامی است.")]
        [Display(Name = "صورت پرسش ")]
        public string QuestionText { get; set; }

        [Required]
        public Enums.QuestionType QuestionType { get; set; }


        //dependency on question type
        [Display(Name = "پاسخ های متنی")]
        [ListMaxLengthDepQTypeAtt((int)Enums.MainQuestion.MaxAnswersNumber, Enums.QuestionType.Main, ErrorMessage = "هر پرسش بایستی حداکثر 10 پاسخ داشته باشد.")]
        [ListMinLengthDepQTypeAtt((int)Enums.MainQuestion.MinAnswersNumber, Enums.QuestionType.Main, ErrorMessage = "هر پرسش بایستی حداقل 2 پاسخ داشته باشد.")]
        public List<string> AnswersText { get; set; }

        [Display(Name = "پاسخ های تصویری")]
        [PictureListMaxLengthDepQTypeAtt((int)Enums.MainQuestion.MaxAnswersNumber, Enums.QuestionType.MainWithPicture, ErrorMessage = "هر پرسش بایستی حداکثر 10 تصویر داشته باشد.")]
        [PictureListMinLengthDepQTypeAtt((int)Enums.MainQuestion.MinAnswersNumber, Enums.QuestionType.MainWithPicture, ErrorMessage = "هر پرسش بایستی حداقل 2 تصویر داشته باشد.")]
        //todo: add each picture size validator attribute.
        public List<HttpPostedFileBase> PictureChoices { get; set; }


        public Enums.MediaType MediaType { get; set; }

        [RequiredIfType(Enums.MediaType.Film, ErrorMessage = "بایستی حتما آدرس فیلم خود را وارد کنید.")]
        public string MediaUrl { get; set; }

        [RequiredIfType(Enums.MediaType.Picture, ErrorMessage = "بایستی حتما تصویر مورد نظر خود را انتخاب کنید.")]
        [PictureMaxLengthAttr(1 * 1024 * 1024, ErrorMessage = "حجم تصویر بایستی کمتر از 1 مگابایت باشد.")]
        public HttpPostedFileBase Picture { get; set; }
    }
}