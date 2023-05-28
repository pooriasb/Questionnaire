using System.ComponentModel.DataAnnotations;

namespace QuestionnaireProject.Areas.Responder.Models
{
    public class ResponderQuestionPackViewModel
    {
        [Required]
        public string QuestionId { get; set; }

        [Required(ErrorMessage = "پاسخ دادن به این سوال الزامی است.")]
        public string Answer { get; set; }

        //[StringDependsOnTheOtherAttr("AnswerId", ErrorMessage = "به همه سوالات پاسخ بگویید.")]
        //public string AnatomicalAnswer { get; set; }

        //[StringDependsOnTheOtherAttr("AnatomicalAnswer", ErrorMessage = "به همه سوالات پاسخ بگویید.")]
        //public string AnswerId { get; set; }

    }
}