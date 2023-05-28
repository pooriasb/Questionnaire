using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace QuestionnaireProject.Models
{
    public class QuestionPack
    {
        public int QuestionnaireId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }

        //[Required]
        //[MinLength(4)]
        public List<string> AnswersText { get; set; }
        public Enums.QuestionType QuestionType { get; set; }
        public string MediaUrl { get; set; }
        public Enums.MediaType MediaType { get; set; }
        public int? CorrectAnswer { get; set; }
        public HttpPostedFileBase Picture { get; set; }
    }
}