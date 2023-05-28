using QuestionnaireProject.Areas.Questioner.Core.IRepositories;

namespace QuestionnaireProject.Areas.Questioner.Core
{
    public interface IUnitOfWork
    {
        IRegisterRepository RegisterRepository { get; }
        void Complete();

    }
}
