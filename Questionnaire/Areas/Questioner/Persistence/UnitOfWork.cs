using QuestionnaireProject.Areas.Questioner.Core;
using QuestionnaireProject.Areas.Questioner.Core.IRepositories;
using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;
using QuestionnaireProject.Models;

namespace QuestionnaireProject.Areas.Questioner.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuestionnaireEntities _context;
        public IRegisterRepository RegisterRepository { get; private set; }

        public UnitOfWork(QuestionnaireEntities context)
        {
            _context = context;
            RegisterRepository = new RegisterRepository(context);
        }

        

        public void Complete()
        {
            _context.SaveChanges();
        }

    }
}