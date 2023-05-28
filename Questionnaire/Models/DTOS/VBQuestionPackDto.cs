using System.Collections.Generic;

namespace QuestionnaireProject.Models.DTOS
{
    public class VBQuestionPackDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<string> AnswersText { get; set; }
        public int QuestionType { get; set; }        
        public int? CorrectAnswerChoiceNumber { get; set; }
    }
}