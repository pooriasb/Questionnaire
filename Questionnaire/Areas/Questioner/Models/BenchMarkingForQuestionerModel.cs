using QuestionnaireProject.Models.DTOS;
using QuestionnaireProject.Models.ViewModels;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Questioner.Models
{
    public class BenchMarkingQuestionModel
    {
        public string QuestionnaireName { get; set; }
        public string QuestionnaireId { get; set; }

        public IEnumerable<FullQuestionPackDto> QuestionPacksDto { get; set; }

        public VBQuestionPackViewModel QuestionPackViewModel { get; set; }

    }
    //public class BenchMarkingQuestionModel
    //{
    //    public BenchMarkingQuestionDto BenchMarkingQuestionDto { get; set; }
    //    public BenchMarkingQuestionViewModel BenchMarkingQuestionViewModel { get; set; }
    //}

    //public class QuestionAndAnswers
    //{
    //    public int QuestionId { get; set; }
    //    public string Question { get; set; }
    //    public List<string> Answers { get; set; }
    //}

    //public class BenchMarkingQuestionDto
    //{
    //    public string QuestionnaireName { get; set; }
    //    public IEnumerable<QuestionAndAnswers> QuestionAndAnswers { get; set; }

    //}
    //public class BenchMarkingQuestionViewModel
    //{
    //    [Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    //    [MaxLength(255, ErrorMessage = "حداکثر 255 کاراکتر مجاز است.")]
    //    [Display(Name = "صورت پرسش")]
    //    public string Question { get; set; }

    //    //[Required(ErrorMessage = "وارد نمودن {0} الزامی است.")]
    //    //[MaxLength(300, ErrorMessage = "حداکثر 300 کاراکتر مجاز است.")]
    //    [Display(Name = "پاسخ پرسش")]
    //    public List<string> Answers { get; set; }

    //    public int? CorrectAnswer { get; set; }

    //    public int QuestionnaireId { get; set; }

    //    public int? QuestionId { get; set; }


    //}


}