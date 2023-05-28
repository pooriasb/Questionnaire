using System.Collections.Generic;
using System.Web;

namespace QuestionnaireProject.Models.DTOS
{
    public class FullQuestionPackDto
    {
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<string> AnswersText { get; set; }
        public List<HttpPostedFileBase> AnswersPicture { get; set; }
        public int? CorrectAnswerChoiceNumber { get; set; }
        public Enums.QuestionType QuestionType { get; set; }
        public string MediaUrl { get; set; }
        public Enums.MediaType MediaType { get; set; }
        public HttpPostedFileBase Picture { get; set; }
        public string EditorComment { get; set; }
    }
}