using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using PagedList;
using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Editor.Controllers
{
    [Authorize(Roles = "Editor")]
    public partial class UsersForEditorController : Controller
    {
        private readonly QuestionnaireEntities _context;

        public UsersForEditorController()
        {
            _context = new QuestionnaireEntities();
        }
        // GET: Editor/UsersForEditor
        //[HttpGet]
        //public ActionResult Index()
        //{
        //    return View();

        //}

        #region Index block and Unblock
        public ActionResult Index(int? page, string search)
        {
            var users = _context.AspNetUsers.Select(x => new
            {
                x.National_Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.PhoneNumber,
                x.LockoutEndDateUtc
            });
            if (!search.IsNullOrWhiteSpace())
            {

                users = users
                    .Where(u => u.FirstName.Contains(search) ||
                                u.LastName.Contains(search) ||
                                u.National_Id.Contains(search));
            }
            
            var dtos = new List<UsersDto>();
            foreach (var user in users)
            {
                var dto = new UsersDto()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    NationalId = user.National_Id,
                    PhoneNumber = user.National_Id,
                    Email = user.Email,
                    IsBlocked = user.LockoutEndDateUtc
                };    
                                dtos.Add(dto);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var model = dtos.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        public ActionResult BlockUser(string nationalId)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.National_Id == nationalId);
            if (user!=null)
            {
                user.LockoutEndDateUtc = DateTime.MaxValue;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult UnblockUser(string nationalId)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.National_Id == nationalId);
            if (user != null)
            {
                user.LockoutEndDateUtc = null;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        #endregion
        public ActionResult ReportedResponders() {

          ViewBag.users=  _context.ResponderReports.Where(x=>x.EditorID==null).ToList();
            return View();
        }
        public ActionResult ReportConfrim(int reportID) {
            try
            {
                var report = _context.ResponderReports.Find(reportID);
                report.ConfirmDate = DateTime.Now;
                report.EditorID = User.Identity.GetUserId();
                _context.SaveChanges();
                TempData["ok"] = "گزارش با موفقیت تایید شد";
            }
            catch (Exception)
            {

                TempData["Error"] = "خطایی رخ داده است";
            }
           
            return RedirectToAction("ReportedResponders");
        }
    }

}