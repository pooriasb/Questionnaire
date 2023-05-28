using QuestionnaireProject.Areas.Questioner.Models;
using QuestionnaireProject.Models;
using QuestionnaireProject.Models.ViewModels;
using QuestionnaireProject.Persistence;
using QuestionnaireProject.Services;
using System.Linq;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Questioner.Controllers
{


    public class BenchMarkingForQuestionerController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly QuestionService _questionService;
        private readonly QuestionnaireRepository _questionnaire;



        public BenchMarkingForQuestionerController()
        {
            _context = new QuestionnaireEntities();
            _questionService = new QuestionService(_context);
            // todo needed QuestionnaireRepo
            _questionnaire = new QuestionnaireRepository(_context);
        }


        /// <summary>
        /// This ActionResult returns the list of questionnaires.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public ActionResult Index(string questionnaireId)
        {
            if (TempData["questionnaireId"]!=null)
            {
                questionnaireId = TempData["questionnaireId"] as string;
            }

            var model = new BenchMarkingQuestionModel
            {
                QuestionnaireId = questionnaireId,
                QuestionPackViewModel = new VBQuestionPackViewModel()
                {
                    QuestionId = null
                },
                QuestionnaireName = _context.Questionnaires.SingleOrDefault(q => q.Id == questionnaireId)?.Name,
                QuestionPacksDto = _questionService.VBQuestionService.GetVBQuestionPacks(questionnaireId)
                    .Where(q => q.QuestionType == Enums.QuestionType.BenchMarking)
            };

            return View(model);
        }
        /// <summary>
        /// this method returns the question data and answers to pass to a modal in client view.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public ActionResult QuestionDetails(string questionId)
        {
            var query = _context.Questions.Where(q => q.Id == questionId).Select(x => new
            {
                Question = x.Text,
                Answers = x.Answers.Select(y => y.AnswerText)
            }).ToList();

            var quesiton = _context.Questions.Where(q => q.Id == questionId).Select(q=>q.Text);
            var answers = _context.Answers.Where(a => a.Question_Id == questionId).Select(a=>a.AnswerText);

            return Json(new {quesiton, answers});
        }
        

        /// <summary>
        /// This ActionResult returns the editable question.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public ActionResult BenchMarkingQuestionEdit(string questionnaireId, string questionId)
        {
            var model = new BenchMarkingQuestionModel()
            {
                QuestionnaireId = questionnaireId
            };
            model.QuestionPackViewModel = _questionService.GetQuestionPack(questionId, questionnaireId, Enums.QuestionType.BenchMarking) as VBQuestionPackViewModel;
            model.QuestionnaireName = _context.Questionnaires.SingleOrDefault(q => q.Id == questionnaireId)?.Name;
            model.QuestionPacksDto = _questionService.VBQuestionService.GetVBQuestionPacks(questionnaireId)
                .Where(q => q.QuestionType == Enums.QuestionType.BenchMarking);
            return View("Index", model);
        }
       


        /// <summary>
        /// This ActionResult Creates and Edits the questions.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateAndEditBenchMarkingQuestion(BenchMarkingQuestionModel model)
        {
            
            if (!ModelState.IsValid)
            {
                model.QuestionnaireName = _context.Questionnaires.SingleOrDefault(q => q.Id == model.QuestionnaireId)?.Name;
                model.QuestionPacksDto = _questionService.VBQuestionService.GetVBQuestionPacks(model.QuestionnaireId)
                    .Where(q => q.QuestionType == Enums.QuestionType.BenchMarking);
                return View("Index", model);
            }

            //todo check the questionnaire status?
            //todo: add the max and min value of benchmarking questions for the questionnaire if it is neccessary
            var benchMarkingQuestionsCount = _context.Questions.Count(q => q.Questionnaire_Id == model.QuestionnaireId &&
                q.QuestionType == (int)Enums.QuestionType.BenchMarking);
            //model.QuestionPackViewModel.QuestionnaireId = questionnaireId;

            if (model.QuestionPackViewModel.QuestionId == null)
            {

                _questionService.ConditionValidator(model.QuestionnaireId, Enums.QuestionType.BenchMarking,
                    Enums.QuestionServiceMode.CreateMode, model.QuestionPackViewModel);
                _questionnaire.ChangeStatusDueToEdit(model.QuestionnaireId);

                //This method changes the status of questionnaier to CREATED due to any changes in questionnaier            
                _questionnaire.ChangeStatusDueToEdit(model.QuestionnaireId);


                _context.SaveChanges();
            }

            else if (model.QuestionPackViewModel.QuestionId != null)
            {
                _questionService.ConditionValidator(model.QuestionnaireId, Enums.QuestionType.BenchMarking,
                    Enums.QuestionServiceMode.EditMode, model.QuestionPackViewModel);
                _questionnaire.ChangeStatusDueToEdit(model.QuestionnaireId);

                //This method changes the status of questionnaier to CREATED due to any changes in questionnaier            
                _questionnaire.ChangeStatusDueToEdit(model.QuestionnaireId);


                _context.SaveChanges();
            }
            

            

            TempData["questionnaireId"] = model.QuestionnaireId;
            return RedirectToAction("Index");
        }


        /// <summary>
        /// This ActionResult returns a json file which can show the details of a questionnaire.
        /// </summary>
        /// <returns></returns>
        public ActionResult BenchMarkingQuestionDetail()
        {
            return View();
        }

        /// <summary>
        /// This ActionResult Deletes the benchmarking question.
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public ActionResult BenchMarkingQuestionDelete(string questionId, string questionnaireId)
        {
            //todo should I check for each user or not?
            //todo check the questionnaire status?
            TempData["questionnaireId"] = questionnaireId;
            _questionService.VBQuestionService.DeleteVB(questionId);

            //This method changes the status of questionnaier to CREATED due to any changes in questionnaier            
            _questionnaire.ChangeStatusDueToEdit(questionnaireId);

            _context.SaveChanges();
            return RedirectToAction("Index", new { questionnaireId });
        }


    }
}