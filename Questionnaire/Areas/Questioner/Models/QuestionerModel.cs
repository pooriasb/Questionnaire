using QuestionnaireProject.Models;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Questioner.Models
{
    public class QuestionerLandingPageDto
    {
        public int SentRequestsCount { get; set; }
        public int AcceptedRequestsCount { get; set; }
        public int RejectedRequsetsCount { get; set; }
        public int UploadedFilesCount { get; set; }
        public int NotPaidQuestionnairesCount { get; set; }
        public int InProcessQuestionnairesCount { get; set; }
        public int FinishedQuestionnairesCount { get; set; }
        public IEnumerable<Notification> Notifications { get; set; }
        public decimal? MoneyBalance { get; set; }
    }
}