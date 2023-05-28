using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Models;
using System.Linq;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Editor.Controllers
{
    //   [Authorize(Roles = "Editor")]
    public class EditorController : Controller
    {
        private readonly QuestionnaireEntities _context;

        public EditorController()
        {
            _context = new QuestionnaireEntities();
        }
        #region  Index - Site Content

        public ActionResult Index()
        {
            IQueryable<Questionnaire> questionnaires = _context.Questionnaires;
            var model = new EditorDto()
            {
                RecievedRequests = questionnaires.Count(q => q.Status == (int)Enums.RequestStatus.Pending),
                RejectedRequests = questionnaires.Count(q => q.Status == (int)Enums.RequestStatus.Rejected),
                AcceptedRequests = questionnaires.Count(q => q.Status == (int)Enums.RequestStatus.Accepted),
                RecievedFiles = 0,
                NotSubmitedQuestionnaires = questionnaires.Count(q => q.Status == (int)Enums.RequestStatus.Created),
                NotPaidQuestionnaires = questionnaires.Count(q => q.Status == (int)Enums.RequestStatus.Accepted),
                PaidQuestionnaires = questionnaires.Count(q => q.Status == (int)Enums.RequestStatus.Paid),
                FinishedQuestionnaires = questionnaires.Count(q => q.Status == (int)Enums.RequestStatus.Finished),
            };
            return View(model);
        }

        /// <summary>
        /// CRUD questionnair categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Categories()
        {

            try
            {
                ViewBag.Cats = _context.QuestionnaireCategories.ToList();
            }
            catch (System.Exception)
            {

                TempData["Error"] = "خطایی رخ داده است لطفا دوباره تلاش کنید.";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Categories(CategoryModel model)
        {
            try
            {
                if (model.id==null||model.id<=0)
                {
                    if (InsertCat(model))
                        TempData["ok"] = "با موفقیت اضافه شد";
                     else
                        TempData["Error"] = "خطایی رخ داده است لطفا دوباره تلاش کنید.";

                    
                }
                else
                {
                    if (UpdateCat(model))
                        TempData["ok"] = "با موفقیت ویرایش شد";
                    else
                        TempData["Error"] = "خطایی رخ داده است لطفا دوباره تلاش کنید.";


                }
         
            }
            catch (System.Exception)
            {

                TempData["Error"] = "خطایی رخ داده است لطفا دوباره تلاش کنید.";

            }
            return RedirectToAction("Categories");
        }

        private bool InsertCat(CategoryModel model) {

            try
            {
                QuestionnaireCategory cat = new QuestionnaireCategory() { Name = model.Name };
                _context.QuestionnaireCategories.Add(cat); _context.SaveChanges();
                return true;
            }
            catch (System.Exception)
            {

                return false;
            }
            
        }
        private bool UpdateCat(CategoryModel model) {
            try
            {
              var find =  _context.QuestionnaireCategories.Find(model.id);
                if (find!=null)
                {
                    find.Name = model.Name;
                    _context.SaveChanges();
                    return true;
                }
                else
                    return false;
                
            }
            catch (System.Exception)
            {

                return false;
            }
        }


        public ActionResult DeleteCategory(int id) {
            try
            {
                var find = _context.QuestionnaireCategories.Find(id);
                if (find != null)
                {
                    _context.QuestionnaireCategories.Remove(find);
                    _context.SaveChanges();
                    TempData["ok"] = "با موفقیت حذف شد";

                }
            }
            catch (System.Exception)
            {

                TempData["Error"] = "خطایی رخ داده است ممکن است این رکورد در جایی دیگر استفاده شده باشد.";

            }
            return RedirectToAction("Categories");
        }


        /// <summary>
        /// change first page content
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult SiteContent() {
            SiteContentModel Content = new SiteContentModel {
                Address = _context.Options.Where(x => x.Name == "Address").SingleOrDefault().Value.ToString(),
                Email = _context.Options.Where(x => x.Name == "Email").SingleOrDefault().Value.ToString()
               , SiteAbout = _context.Options.Where(x => x.Name == "SiteAbout").SingleOrDefault().Value.ToString()
               ,SiteBody1 = _context.Options.Where(x => x.Name == "SiteBody1").SingleOrDefault().Value.ToString()
               ,SiteBody2 = _context.Options.Where(x => x.Name == "SiteBody2").SingleOrDefault().Value.ToString()
               ,Sitepurpose= _context.Options.Where(x => x.Name == "Sitepurpose").SingleOrDefault().Value.ToString()
               ,SiteTitle= _context.Options.Where(x => x.Name == "SiteTitle").SingleOrDefault().Value.ToString()
               ,SV1 = _context.Options.Where(x => x.Name == "SV1").SingleOrDefault().Value.ToString()
               ,SV1Title= _context.Options.Where(x => x.Name == "SV1Title").SingleOrDefault().Value.ToString()
               , SV2 = _context.Options.Where(x => x.Name == "SV2").SingleOrDefault().Value.ToString()
               ,SV2Title = _context.Options.Where(x => x.Name == "SV2Title").SingleOrDefault().Value.ToString()
               ,SV3 = _context.Options.Where(x => x.Name == "SV3").SingleOrDefault().Value.ToString()
               ,SV3Title = _context.Options.Where(x => x.Name == "SV3Title").SingleOrDefault().Value.ToString()
               ,Tell = _context.Options.Where(x => x.Name == "Tell").SingleOrDefault().Value.ToString()

            };
            return View(Content);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public ActionResult SiteContent(SiteContentModel model)
        {
            try
            {
                _context.Options.Where(x => x.Name == "Address").FirstOrDefault().Value = model.Address;
                _context.Options.Where(x => x.Name == "Email").FirstOrDefault().Value = model.Email;
                _context.Options.Where(x => x.Name == "SiteAbout").FirstOrDefault().Value = model.SiteAbout;
                _context.Options.Where(x => x.Name == "SiteBody1").FirstOrDefault().Value = model.SiteBody1;
                _context.Options.Where(x => x.Name == "SiteBody2").FirstOrDefault().Value = model.SiteBody2;
                _context.Options.Where(x => x.Name == "Sitepurpose").FirstOrDefault().Value = model.Sitepurpose;
                _context.Options.Where(x => x.Name == "SiteTitle").FirstOrDefault().Value = model.SiteTitle;
                _context.Options.Where(x => x.Name == "SV1").FirstOrDefault().Value = model.SV1;
                _context.Options.Where(x => x.Name == "SV1Title").FirstOrDefault().Value = model.SV1Title;
                _context.Options.Where(x => x.Name == "SV2").FirstOrDefault().Value = model.SV2;
                _context.Options.Where(x => x.Name == "SV2Title").FirstOrDefault().Value = model.SV2Title;
                _context.Options.Where(x => x.Name == "SV3").FirstOrDefault().Value = model.SV3;
                _context.Options.Where(x => x.Name == "SV3Title").FirstOrDefault().Value = model.SV3Title;
                _context.Options.Where(x => x.Name == "Tell").FirstOrDefault().Value = model.Tell;
                _context.SaveChanges();
                TempData["ok"] = "اطلاعات سایت با موفقیت بروزرسانی شد";
            }
            catch (System.Exception ex)
            {

                TempData["error"] = "خطایی رخ داده است";
            }
            return RedirectToAction("SiteContent");
        }
        #endregion


    }
}