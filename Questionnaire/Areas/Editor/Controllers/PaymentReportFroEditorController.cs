using PagedList;
using QuestionnaireProject.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Editor.Controllers
{
    [Authorize(Roles = "Editor")]
    public class PaymentReportFroEditorController : Controller
    {
        private readonly QuestionnaireEntities _context;

        public PaymentReportFroEditorController()
        {
            _context = new QuestionnaireEntities();
        }        


        /// <summary>
        /// This ActionResult return a full report of payments.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="type"></param>
        /// <param name="days"></param>
        /// <param name="person"></param>
        /// <returns></returns>
        public ActionResult Index(int? page,int? type, string person, DateTime? fromDate, DateTime? toDate)
        {
            ViewBag.CurrentType = type;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.CurrentPerson = person;            

            IQueryable<Payment> payments = _context.Payments.Include(x=>x.AspNetUser).OrderBy(p => p.PaymentDate);
            if (type!=null)
            {
                payments = payments.Where(p => p.Type == type);
            }

            if (fromDate!=null)
            {                
                payments = payments.Where(p => p.PaymentDate >= fromDate);
            }

            if (toDate != null)
            {
                payments = payments.Where(p => p.PaymentDate <= toDate);
            }
            if (person != null)
            {
                payments = payments.Where(p => p.AspNetUser.FirstName.Contains(person) ||
                                               p.AspNetUser.LastName.Contains(person) ||
                                               p.AspNetUser1.FirstName.Contains(person) ||
                                               p.AspNetUser1.LastName.Contains(person));
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var model = payments.ToPagedList(pageNumber, pageSize);
            return View(model);
        }
    }
}

