using QuestionnaireProject.Models;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Questioner.Models
{
    public class EachPersonReportModel
    {
        public string Question { get; set; }
        public string Response { get; set; }
        public Enums.QuestionType QuestionType { get; set; }
        public Enums.MediaType MediaType { get; set; }
        public string MediaUrl { get; set; }
    }

    public class EachQuestionnaireAnswerModel
    {
        public string AnswerId { get; set; }
        public int Count { get; set; }
        public string Answer { get; set; }
    }

    public class EachQuestionnaireReportModel
    {
        public string Question { get; set; }
        public string QuestionId { get; set; }
        public Enums.QuestionType QuestionType { get; set; }
        public List<EachQuestionnaireAnswerModel> EachQuestionnaiireAnswerModels { get; set; }

    }

}