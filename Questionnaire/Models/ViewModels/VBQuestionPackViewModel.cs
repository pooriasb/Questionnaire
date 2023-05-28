using QuestionnaireProject.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestionnaireProject.Models.ViewModels
{
    public class VBQuestionPackViewModel
    {

        public string QuestionId { get; set; }

        [Required(ErrorMessage = "وارد نمودن صورت پرسش الزامی است.")]
        [Display(Name = "صورت پرسش")]
        public string QuestionText { get; set; }

        [Display(Name = "پاسخ های پرسش")]
        [ListMinLengthAttr((int)Enums.BenchMarkingQuestion.MinAnswersNumber, ErrorMessage = "هر پرسش بایستی حداقل 2 پاسخ داشته باشد.")]
        [ListMaxLengthAttr((int)Enums.BenchMarkingQuestion.MaxAnswersNumber, ErrorMessage = "هر پرسش بایستی حداکثر 10 پاسخ داشته باشد.")]
        public List<string> AnswersText { get; set; }

        public List<string> AnswersId { get; set; }


        [Required]
        public int QuestionType { get; set; }
        //public string MediaUrl { get; set; }
        //public Enums.MediaType MediaType { get; set; }

        [Required(ErrorMessage = "انتخاب گزینه صحیح الزامی میباشد.")]
        [CorrectAnswerAttr(ErrorMessage = "در انتخاب گزینه صحیح دقت نمایید.")]
        public int CorrectAnswerChoiceNumber { get; set; }
    }
}