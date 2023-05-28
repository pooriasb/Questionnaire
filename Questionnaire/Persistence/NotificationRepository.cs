using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence.Repositories;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QuestionnaireProject.Persistence
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly QuestionnaireEntities _context;

        public NotificationRepository(QuestionnaireEntities context)
        {
            _context = context;
        }
        public IQueryable<Notification> GetAll()
        {

            IQueryable<Notification> query = _context.Notifications;
            return query;
        }

        public Notification GetSingle(int notificationId)
        {

            var query = this.GetAll().FirstOrDefault(x => x.Id == notificationId);
            return query;
        }

        public IQueryable<Notification> FindBy(Expression<Func<Notification, bool>> predicate)
        {
            IQueryable<Notification> query = _context.Set<Notification>().Where(predicate);
            return query;
        }

        public void Add(Notification entity)
        {

            _context.Notifications.Add(entity);
        }

        public void Delete(Notification entity)
        {

            _context.Notifications.Remove(entity);
        }

        public void Edit(Notification entity)
        {

            _context.Entry<Notification>(entity).State = System.Data.Entity.EntityState.Modified;
        }

    }
}