using QuestionnaireProject.Models;
using System;
using System.Linq;
using System.Web.Mvc;


namespace QuestionnaireProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuestionnaireEntities _context;

        public HomeController()
        {
            _context = new QuestionnaireEntities();
        }


        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = _context.Options.Where(x=>x.Name== "SiteTitle").SingleOrDefault().Value.ToString();
            ViewBag.SiteBody1 = _context.Options.Where(x => x.Name == "SiteBody1").SingleOrDefault().Value.ToString();
            ViewBag.SiteBody2 = _context.Options.Where(x => x.Name == "SiteBody2").SingleOrDefault().Value.ToString();
            ViewBag.SitePurpose = _context.Options.Where(x => x.Name == "Sitepurpose").SingleOrDefault().Value.ToString();
            ViewBag.SiteAbout = _context.Options.Where(x => x.Name == "SiteAbout").SingleOrDefault().Value.ToString();
            ViewBag.SV1 = _context.Options.Where(x => x.Name == "SV1").SingleOrDefault().Value.ToString();
            ViewBag.SV2 = _context.Options.Where(x => x.Name == "SV2").SingleOrDefault().Value.ToString();
            ViewBag.SV3 = _context.Options.Where(x => x.Name == "SV3").SingleOrDefault().Value.ToString();
            ViewBag.SV1Title = _context.Options.Where(x => x.Name == "SV1Title").SingleOrDefault().Value.ToString();
            ViewBag.SV2Title = _context.Options.Where(x => x.Name == "SV2Title").SingleOrDefault().Value.ToString();
            ViewBag.SV3Title = _context.Options.Where(x => x.Name == "SV3Title").SingleOrDefault().Value.ToString();
            ViewBag.Address = _context.Options.Where(x => x.Name == "Address").SingleOrDefault().Value.ToString();
            ViewBag.Email = _context.Options.Where(x => x.Name == "Email").SingleOrDefault().Value.ToString();
            ViewBag.Tell = _context.Options.Where(x => x.Name == "Tell").SingleOrDefault().Value.ToString();

            return View("index2");
        }

        [HttpPost]
        public ActionResult Discount()
        {
            var text = _context.Options.FirstOrDefault(o =>
                o.Name == "DiscountOccasionalWord");
            var date = _context.Options.FirstOrDefault(o =>
                o.Name == "DiscountOccasionalDate");
            if (date.Value!=null)
            {
                var dateTime = DateTime.Parse(date.Value);

                if (DateTime.Now < DateTime.Parse(date.Value))
                {
                    return Json(new { text = text.Value, date = dateTime });

                }
                else
                {
                    var option = _context.Options.Find(text.Id);
                    option.Value = null;
                    var option2 = _context.Options.Find(date.Id);
                    option2.Value = null;
                    _context.SaveChanges();
                }
            }
            

            return Json(null);
        }
        
        [System.Web.Mvc.HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [System.Web.Mvc.HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        
    }


   
}
