using QuestionnaireProject.Models;
using System.Collections.Generic;
using System.Web;

namespace QuestionnaireProject.Areas.Questioner.Persistence.Repositories
{
    interface IMediaRepository
    {
        Media Create(string questionId, string url);
        Media Edit(string questionId, string url);
        Media GetByQuestionId(string questionId);
        Media GetById(int mediaId);
        IEnumerable<Media> GetAllMedias(string questionnaireId);
        void Delete(string questionId);
        string CreatePictureLink(HttpPostedFileBase file, HttpServerUtilityBase server, string prefix);

    }
}
