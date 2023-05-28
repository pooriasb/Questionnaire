using QuestionnaireProject.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QuestionnaireProject.Persistence.Repositories
{
    interface INotificationRepository
    {
        IQueryable<Notification> GetAll();
        Notification GetSingle(int barId);
        IQueryable<Notification> FindBy(Expression<Func<Notification, bool>> predicate);
        void Add(Notification entity);
        void Delete(Notification entity);
        void Edit(Notification entity);
    }
}
