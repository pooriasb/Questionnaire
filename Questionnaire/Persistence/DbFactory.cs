using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence.Repositories;

namespace QuestionnaireProject.Persistence
{
    public class DbFactory<C> : IDbFactory<C> where C: QuestionnaireEntities, new()
    {
        private C _context;
        public C Init()
        {
            return _context ?? (_context = new C());
        }
        //protected override void DisposeCore()
        //{
        //    if (_context != null)
        //        _context.Dispose();
        //}
    }
}