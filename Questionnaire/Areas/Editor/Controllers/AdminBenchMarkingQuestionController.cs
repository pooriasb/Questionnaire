using PagedList;
using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System.Linq;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Editor.Controllers
{
    [Authorize(Roles = "Editor")]
    public class AdminBenchMarkingQuestionController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly AdminBanchMarkingQuestionRepository _adminQuestions;

        public AdminBenchMarkingQuestionController()
        {
            _context = new QuestionnaireEntities();
            _adminQuestions = new AdminBanchMarkingQuestionRepository(_context);
        }


        /// <summary>
        /// This ActionResult returns the list of Admin BenchMarking Questions with pagination.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            IQueryable<AdminQuestion> questions = _adminQuestions.GetAll();
            var model = questions.ToPagedList(pageNumber, pageSize);
            return View(model);
        }




        /// <summary>
        /// This ActionResult allow the user to create new or edit the Admin BenchMarking questions.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NewAdminQuestion(string id)
        {


            if (id != null)
            {
                var model = _adminQuestions.GetSingle(id);
                return View(model);
            }
            var modelEmpty = new AdminNewQuestion();

            return View(modelEmpty);
        }


        /// <summary>
        /// This [HttpPost] ActionResult save the changes in Admin BenchMarking Questions.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NewAdminQuestion(AdminNewQuestion question)
        {
            if (!ModelState.IsValid)
            {
                return View(question);
            }

            if (question.QuestionId == null)
            {
                _adminQuestions.Create(question);
                _context.SaveChanges();

            }
            else
            {
               _adminQuestions.Edit(question);
                _context.SaveChanges();

            }

            return RedirectToAction("Index", "AdminBenchMarkingQuestion");
        }


        /// <summary>
        /// This ActionResult delete the Admin BenchMarking Question By Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteAdminQuestion(string id)
        {
            _adminQuestions.Delete(id);
            _context.SaveChanges();
            return RedirectToAction("Index", "AdminBenchMarkingQuestion");

        }
        
        
    }
}