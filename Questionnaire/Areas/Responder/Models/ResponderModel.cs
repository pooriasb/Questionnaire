using PagedList;

namespace QuestionnaireProject.Areas.Responder.Models
{
    public class ResponderIndexDto
    {
        public IPagedList<AvailableQuestionnaireDto> AvailableQuestionnaireDtos { get; set; }
        public int AvailableQuestionnaires { get; set; }
        public int AnsweredQuestionnaires { get; set; }
        public decimal MoneyToReceive { get; set; }
    }


}