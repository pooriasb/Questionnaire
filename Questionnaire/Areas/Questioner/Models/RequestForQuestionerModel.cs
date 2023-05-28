using QuestionnaireProject.Models;

namespace QuestionnaireProject.Areas.Questioner.Models
{
    public class QuestionnaireSummaryModel
    {
        public QuestionnaireSummaryDto QuestionnaireSummaryDto { get; set; }
    }

    public class QuestionnaireSummaryDto
    {
        public string QuestionnaireId { get; set; }
        public string QuestionnaireName { get; set; }
        public QuestionCounter BenchMarkingQuestion { get; set; }
        public QuestionCounter ValidationQuestion { get; set; }
        public QuestionCounter MainQuestion { get; set; }
        public QuestionCounter AnatomicalQuestion { get; set; }
        public Enums.RequestStatus RequestStatus { get; set; }
    }

    public class QuestionCounter
    {
        public int Total { get; set; }
        public int NoMedia { get; set; }
        public int HasPicture { get; set; }
        public int HasFilm { get; set; }
    }
}