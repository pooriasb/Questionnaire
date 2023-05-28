using System.Web;

namespace QuestionnaireProject.Areas.Questioner.Core.IRepositories
{
    public interface IRegisterRepository
    {
        void InsertQuestionerProfile(string nationalId, HttpPostedFileBase nationalCardImage, string userId);
    }
}
