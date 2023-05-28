using QuestionnaireProject.Models;

namespace QuestionnaireProject.Persistence.Repositories
{
    interface IUnitOfWork<C> where C: QuestionnaireEntities
    {
        void Complete();
    }
}
