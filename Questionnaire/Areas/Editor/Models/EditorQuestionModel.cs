using QuestionnaireProject.Attributes;
using QuestionnaireProject.Models;
using QuestionnaireProject.Models.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestionnaireProject.Areas.Editor.Models
{
    public class EditorQuestionModel
    {
        public string QuestionId { get; set; }
        public Enums.QuestionType QuestionType { get; set; }
        public VBQuestionPackViewModel VbQuestionViewModel { get; set; }
        public MainQuestionViewModel MainQuestionViewModel { get; set; }
    }

    public class AdminNewQuestion
    {
        public string QuestionId { get; set; }

        [Required(ErrorMessage = "ورود پرسش الزامی است")]
        [MaxLength(255, ErrorMessage = "حداکثر 255 کاراکتر")]
        [Display(Name = "صورت پرسش")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "ورود پاسخ صحیح الزامی است")]
        [MaxLength(255, ErrorMessage = "حداکثر 255 کاراکتر")]
        [Display(Name = "پاسخ صحیح")]
        public string CorrectAnswer { get; set; }

        [ListMaxLengthAttr((int)Enums.BenchMarkingQuestion.MaxAnswersNumber - 1, ErrorMessage = "حداکثر 10 پاسخ قابل قبول است.")]
        [ListMinLengthAttr((int)Enums.BenchMarkingQuestion.MinAnswersNumber - 1, ErrorMessage = "حداقل 4 پاسخ قابل قبول است.")]
        [Display(Name = "پاسخ غیر صحیح")]
        public List<string> AnswersText { get; set; }

    }


}