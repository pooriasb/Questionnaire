using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence.Repositories;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace QuestionnaireProject.Persistence
{
    public abstract class GenericRepository<C, T> :
        IGenericRepository<T> where T : class where C : QuestionnaireEntities, new()
    {
        private C _context = new C();
        public C Context
        {

            get { return _context; }
            set { _context = value; }
        }

        public IQueryable<T> GetAll()
        {
            IQueryable<T> query = _context.Set<T>();
            return query;
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>().Where(predicate);
            return query;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Edit(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;            
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
    
}
