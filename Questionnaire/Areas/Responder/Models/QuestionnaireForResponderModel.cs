using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Responder.Models
{
    public class AvailableQuestionnaireDto
    {
        public string QuestionnaireId { get; set; }
        public string QuestionnaireName { get; set; }
        public int QuestionsNumber { get; set; }
        public decimal? EachPersonMoney { get; set; }
        public int? Special { get; set; }
        public int? Rate { get; set; }
        public string StudyField { get; set; }
        public string QuestionnaireCategory { get; set; }
        public int DateToExpire { get; set; }
        public int? PeapleCount { get; set; }
    }
    public class BenchMarkingQuestionModel
    {
        public string QuestionnaireId { get; set; }
        public string QuestionnaireName { get; set; }
        public List<ResponderQuestionPackDto> ResponderQuestionPackDtos { get; set; }
        public List<ResponderQuestionPackViewModel> ResponderQuestionPackViewModels { get; set; }

    }

    public class MainQuestionPackModel
    {
        public string QuestionnaireId { get; set; }
        public string QuestionnaireName { get; set; }
        public List<ResponderQuestionPackDto> ResponderQuestionPackDtos { get; set; }
        public List<ResponderQuestionPackViewModel> ResponderQuestionPackViewModels { get; set; }
    }
}