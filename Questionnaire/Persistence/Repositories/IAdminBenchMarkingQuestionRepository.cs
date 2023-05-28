using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Models;
using System.Linq;

namespace QuestionnaireProject.Persistence.Repositories
{
    interface IAdminBenchMarkingQuestionRepository
    {
        void Create(AdminNewQuestion question);
        void Delete(string questionId);
        void Edit(AdminNewQuestion question);
        IQueryable<AdminQuestion> GetAll();
        AdminNewQuestion GetSingle(string questionId);
    }
}
