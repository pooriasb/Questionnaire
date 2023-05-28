using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Responder.Models
{
    public class ResponderQuestionPackDto
    {
        public string QuestionnaireId { get; set; }
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }       
        public List<string> AnswersText { get; set; }
        public List<string> AnswerIds { get; set; }
        public string MediaUrl { get; set; }
        public Enums.MediaType MediaType { get; set; }
        public Enums.QuestionType QuestionType { get; set; }
    }
}