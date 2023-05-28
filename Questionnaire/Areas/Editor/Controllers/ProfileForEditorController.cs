using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Editor.Controllers
{
    [Authorize(Roles = "Editor")]
    public class ProfileForEditorController : Controller
    {
        // GET: ProfileForEditor
        public ActionResult Index()
        {
            return View();
        }
    }
}