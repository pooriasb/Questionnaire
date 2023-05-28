using QuestionnaireProject.Models;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Responder.Models
{
    public class QuestionPackServerViewModel
    {
        public string QuestionId { get; set; }
        public Enums.QuestionType QuestionType { get; set; }
        public List<string> AnswerId { get; set; }
        public string CorrectAnswer { get; set; }
    }
}