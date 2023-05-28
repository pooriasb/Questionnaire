using Microsoft.AspNet.Identity;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System.Linq;
using System.Web.Http;

namespace QuestionnaireProject.Controllers.API
{
    public class NotificationsController : ApiController
    {
        public readonly NotificationRepository _notification;
        public readonly QuestionnaireEntities _context;

        public NotificationsController()
        {
            _context = new QuestionnaireEntities();
            _notification = new NotificationRepository(_context);
        }
       

        [HttpPost]
        public IHttpActionResult GetNotification()
        {

            var userId = User.Identity.GetUserId();
            var notifications = _notification.GetAll().Where(n=>n.ToUser_Id == userId).Select(s=>new
            {
                s.Id,
                s.Title, 
                s.Type,
                s.IsSeen
            }).OrderByDescending(n=>n.Id);
            return Json(notifications);
        }

        public class Notification
        {
            public int Id { get; set; }
        }
        [HttpPost]
        public IHttpActionResult GetNotificationDetails([FromBody]Notification notification)
        {

            var userId = User.Identity.GetUserId();
            var notificationDetail = _notification.GetAll().Where(n => n.ToUser_Id == userId && n.Id == notification.Id).Select(s=>new
            {
                s.Title,
                s.Text,
                s.DateCreated,
                s.Type,                
            }).FirstOrDefault();

            var notificationEdit = _context.Notifications.Find(notification.Id);
            notificationEdit.IsSeen = true;
            _context.SaveChanges();
            //_notification.Edit(new Models.Notification()
            //{
            //    IsSeen = true
            //});
            //_context.SaveChanges();
            return Json(notificationDetail);
        }

        [HttpPost]
        public IHttpActionResult GetNotificationAndroid()
        {

            var userId = User.Identity.GetUserId();
            var notifications = _notification.GetAll().Where(n => n.ToUser_Id == userId).Select(s => new
            {
                s.Id,
                s.Title,
                s.Type,
                s.Text,
                s.IsSeen,
                s.DateCreated
            }).OrderByDescending(n => n.Id);
            return Json(notifications);
        }

        [HttpPost]
        public IHttpActionResult NotificationIsSeenAndroid(Notification notification)
        {

            var notificationEdit = _context.Notifications.Find(notification.Id);
            notificationEdit.IsSeen = true;
            _context.SaveChanges();       
            return Json(true);
        }

    }
}
