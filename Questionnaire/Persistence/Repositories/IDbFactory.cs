using QuestionnaireProject.Models;

namespace QuestionnaireProject.Persistence.Repositories
{
    interface IDbFactory<C> where C: QuestionnaireEntities
    {
        C Init();
    }
}
