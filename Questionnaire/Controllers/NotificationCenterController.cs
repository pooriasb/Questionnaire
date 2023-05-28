using Microsoft.AspNet.Identity;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace QuestionnaireProject.Controllers
{
    [System.Web.Mvc.Authorize]
    public class NotificationCenterController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly NotificationRepository _notification;

        public NotificationCenterController()
        {
            _context = new QuestionnaireEntities();
            _notification = new NotificationRepository(_context);
        }
        // GET: NotificationCenter
        public ActionResult Index(string Dashboard = "def")
        {
            ViewBag.Dashboard = Dashboard;
            string userId = User.Identity.GetUserId();
            var notifications = _notification.GetAll().OrderByDescending(n=> n.DateCreated).Where(n => n.ToUser_Id == userId).ToList();
            return View(notifications);
        }

        public class SendNotificationModel
        {
            public string NationalId { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
            public bool Push { get; set; }
        }

        [System.Web.Mvc.Authorize(Roles = "Editor")]
        [System.Web.Mvc.HttpPost]
        public JsonResult SendNotification([FromBody]SendNotificationModel model)
        {
            var request = Request;
            string userId = _context.AspNetUsers.FirstOrDefault(u => u.National_Id == model.NationalId)?.Id;
            if (userId != null && model.Text!="" && model.Title!=null)
            {
                Notification notification = new Notification()
                {
                    FromUser_Id = User.Identity.GetUserId(),
                    Type = (int)Enums.NotificationType.Info,
                    DateCreated = DateTime.Now,
                    Title = model.Title,
                    IsSeen = false,
                    ToUser_Id = userId,
                    Text = model.Text
                };

                _notification.Add(notification);
                if (model.Push == true)
                {
                    //pushe should be added
                }

                _context.SaveChanges();
                return Json(new { result = true, message = "اعلان با موفقیت ارسال گردید." });
            }
            return Json(new { result = false, message = "در ارسال اعلان اشکالی به وجود آمده است. دقت نمایید که درج عنوان و متن پیام الزامی است." });

        }

        public class SendNotificationAllModel
        {
            public string Title { get; set; }
            public string Text { get; set; }
            public bool Push { get; set; }
        }

        [System.Web.Mvc.Authorize(Roles = "Editor")]
        public JsonResult SendNotificationToAll(SendNotificationAllModel model)
        {
            if (model.Text != "" && model.Title != null)
            {
                List<string> allUserIds = _context.AspNetUsers.Select(u => u.Id).ToList();
                foreach (string userId in allUserIds)
                {
                    Notification notification = new Notification()
                    {
                        FromUser_Id = User.Identity.GetUserId(),
                        Type = (int)Enums.NotificationType.Info,
                        DateCreated = DateTime.Now,
                        Title = model.Title,
                        IsSeen = false,
                        ToUser_Id = userId,
                        Text = model.Text
                    };

                    _notification.Add(notification);
                    if (model.Push == true)
                    {
                        //pushe
                    }
                }
                _context.SaveChanges();

                return Json(new { result = true, message = "اعلان ها با موفقیت ارسال گردید." });
            }
            return Json(new { result = false, message = "در ارسال اعلان اشکالی به وجود آمده است. دقت نمایید که درج عنوان و متن پیام الزامی است." });


        }
    }
}