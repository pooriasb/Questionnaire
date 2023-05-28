using Microsoft.AspNet.Identity;
using QuestionnaireProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace QuestionnaireProject.Controllers.API
{
    public class DiscountController : ApiController
    {
        private readonly QuestionnaireEntities _context;

        public DiscountController()
        {
            _context = new QuestionnaireEntities();
        }
        [HttpPost]
        public IHttpActionResult GetUserDicounts()
        {
            string userId = User.Identity.GetUserId();
            var discounts = _context.Discounts./*Where(d => d.User_Id == userId).*/Select(x=> new
            {
                x.User_Id,
                x.Id,
                x.SitePercent
            }).ToList();
            return Json(discounts);
        }
    }
}
