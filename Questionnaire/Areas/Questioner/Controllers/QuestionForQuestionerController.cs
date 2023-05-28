using QuestionnaireProject.Areas.Questioner.Models;
using QuestionnaireProject.Areas.Questioner.Persistence;
using QuestionnaireProject.Models;
using QuestionnaireProject.Models.ViewModels;
using QuestionnaireProject.Persistence;
using QuestionnaireProject.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Questioner.Controllers
{
    public class QuestionForQuestionerController : Controller
    {
        private readonly QuestionnaireEntities _context;

        private readonly QuestionService _questionService;
        private readonly MediaRepository _media;
        private readonly NotificationRepository _notification;
        private readonly QuestionnaireRepository _questionnaire;

        public QuestionForQuestionerController()
        {
            _context = new QuestionnaireEntities();
            _questionService = new QuestionService(_context);
            _media = new MediaRepository(_context);
            _notification = new NotificationRepository(_context);
            _questionnaire = new QuestionnaireRepository(_context);

        }


        


        /// <summary>
        /// This ActionResult returns a list of questions.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public ActionResult Index(string questionnaireId)
        {
            if (questionnaireId == null)
            {
                questionnaireId = TempData["questionnaireId"] as string;
            }
            var model = new QuestionForQuestionerModel();
            model.QuestionnaireId = questionnaireId;
            model.QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId)?.Name;
            if (model.MainQuestionPackViewModel == null)
            {
                model.MainQuestionPackViewModel = new MainQuestionPackViewModel()
                {
                    QuestionId = null
                };
            }

            if (model.VbQuestionPackViewModel == null)
            {
                model.VbQuestionPackViewModel = new VBQuestionPackViewModel()
                {
                    QuestionId = null
                };
            }

            model.FullQuestionPackDtos = _questionService.GetQuestionPacks(model.QuestionnaireId).Where(q => q.QuestionType != Enums.QuestionType.BenchMarking).OrderBy(q=>q.QuestionId);
            return View(model);
        }

        [HttpPost]
        public ActionResult QuesitonDetails(string questionId)
        {
            var question = _context.Questions.Where(q => q.Id == questionId).Select(s => new
            {
                question = s.Text,
                type = s.QuestionType,
                media = s.MediaType,
                mediaUrl = s.Medias.Where(m=>m.Question_Id == questionId).Select(m=>m.Url).FirstOrDefault(),
                editorComment = s.EditorComment

            }).FirstOrDefault();
            List<string> answers = null;
            answers = _context.Answers.Where(a => a.Question_Id == questionId).Select(a => a.AnswerText).ToList();
            //if (question.type == (int) Enums.QuestionType.Validation)
            //{
            //    answers = _context.VBAnswers.Where(a=>a.Question_Id == questionId).Select(a=>a.Answer).ToList();

            //}
            //if (question.type == (int)Enums.QuestionType.MainWithPicture ||
            //    question.type == (int)Enums.QuestionType.Main)
            //{
            //    answers = _context.MultipleChoiceAnswers.Where(a => a.Question_Id == questionId).Select(a => a.Answer).ToList();

            //}


            return Json(new {question, answers });
        }

       
        [HttpPost]
        public ActionResult CreateQuestion(QuestionForQuestionerModel model)
        {
            if (!ModelState.IsValid)
            {
                if (model.MainQuestionPackViewModel == null)
                {
                    model.MainQuestionPackViewModel = new MainQuestionPackViewModel()
                    {
                        QuestionId = null
                    };
                }

                if (model.VbQuestionPackViewModel == null)
                {
                    model.VbQuestionPackViewModel = new VBQuestionPackViewModel()
                    {
                        QuestionId = null
                    };
                }
                //todo sequence of questions should be checked.
                model.FullQuestionPackDtos = _questionService.GetQuestionPacks(model.QuestionnaireId).Where(q => q.QuestionType != Enums.QuestionType.BenchMarking);

                return View("Index", model);
            }

            //todo chech the amount of the main and validation questions?
            if (model.VbQuestionPackViewModel == null && model.MainQuestionPackViewModel != null)
            {
                //select and save type and url of the media
                if (model.MainQuestionPackViewModel.MediaType == Enums.MediaType.Picture)
                {
                    model.MainQuestionPackViewModel.MediaUrl =
                        _media.CreatePictureLink(model.MainQuestionPackViewModel.Picture, server: Server, prefix:"");
                }
                else if (model.MainQuestionPackViewModel.MediaType == Enums.MediaType.Film)
                {
                    var pureUrl = model.MainQuestionPackViewModel.MediaUrl;
                    int position = pureUrl.LastIndexOf("/") + 1;
                    model.MainQuestionPackViewModel.MediaUrl = pureUrl.Substring(position, pureUrl.Length - position);
                }


                //this code checks the quesiton type and if it is with media choice it would create a url for
                //each of them and then would put the url in a new answer text to show to the user.
                if (model.MainQuestionPackViewModel.QuestionType == Enums.QuestionType.MainWithPicture)
                {
                    model.MainQuestionPackViewModel.AnswersText = new List<string>();
                    var pictures = model.MainQuestionPackViewModel.PictureChoices;
                    int choiceNumber = 1;
                    foreach (var picture in pictures)
                    {
                        if (picture != null)
                        {
                            string prefix = $"{choiceNumber}" + "-";
                            var file = _media.CreatePictureLink(picture, server: Server, prefix: prefix);
                            model.MainQuestionPackViewModel.AnswersText.Add(file);
                        }

                        choiceNumber++;
                    }
                }


                if (model.MainQuestionPackViewModel.QuestionId == null)
                {
                    _questionService.ConditionValidator(model.QuestionnaireId,
                        model.MainQuestionPackViewModel.QuestionType,
                        Enums.QuestionServiceMode.CreateMode,
                        model.MainQuestionPackViewModel);
                    _questionnaire.ChangeStatusDueToEdit(model.QuestionnaireId);
                    _context.SaveChanges();
                }
                else if (model.MainQuestionPackViewModel.QuestionId != null)
                {
                    _questionService.ConditionValidator(model.QuestionnaireId,
                        model.MainQuestionPackViewModel.QuestionType,
                        Enums.QuestionServiceMode.EditMode,
                        model.MainQuestionPackViewModel);
                    _questionnaire.ChangeStatusDueToEdit(model.QuestionnaireId);
                    _context.SaveChanges();

                }

            }
            if (model.VbQuestionPackViewModel != null && model.MainQuestionPackViewModel == null)
            {
                if (model.VbQuestionPackViewModel.QuestionId == null)
                {
                    _questionService.ConditionValidator(model.QuestionnaireId,
                        (Enums.QuestionType)model.VbQuestionPackViewModel.QuestionType,
                        Enums.QuestionServiceMode.CreateMode,
                        model.VbQuestionPackViewModel);
                    _questionnaire.ChangeStatusDueToEdit(model.QuestionnaireId);
                    _context.SaveChanges();
                }
                else if (model.VbQuestionPackViewModel.QuestionId != null)
                {
                    _questionService.ConditionValidator(model.QuestionnaireId,
                        (Enums.QuestionType)model.VbQuestionPackViewModel.QuestionType,
                        Enums.QuestionServiceMode.EditMode,
                        model.VbQuestionPackViewModel);
                    _questionnaire.ChangeStatusDueToEdit(model.QuestionnaireId);
                    _context.SaveChanges();

                }

            }

            //todo change the questionnaire status if any changes happen.
            TempData["questionnaireId"] = model.QuestionnaireId;
            return RedirectToAction("Index");            //model.QuestionPackViewModel.QuestionnaireId = questionnaireId;
        }









        /// <summary>
        /// This ActionResult return the maiin questions to edit.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public ActionResult EditMainQuestion(string questionnaireId, string questionId)
        {

            //todo: chekc the questionnaire status?
            //todo check the user access to question.
            var model = new QuestionForQuestionerModel()
            {
                MainQuestionPackViewModel = (MainQuestionPackViewModel)_questionService.GetQuestionPack(questionId, questionnaireId, Enums.QuestionType.Main),
                QuestionnaireId = questionnaireId,
                FullQuestionPackDtos = _questionService.GetQuestionPacks(questionnaireId).Where(q => q.QuestionType != Enums.QuestionType.BenchMarking),

                VbQuestionPackViewModel = new VBQuestionPackViewModel()
                {
                    QuestionId = null
                },
                QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId)?.Name
            };

            _questionnaire.ChangeStatusDueToEdit(model.QuestionnaireId);
            _context.SaveChanges();

            return View("Index", model);

        }


        /// <summary>
        /// This ActionResult return the validation questions to edit.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public ActionResult EditValidationQuestion(string questionnaireId, string questionId)
        {

            //todo: chekc the questionnaire status?
            //todo check the user access to question.
            var model = new QuestionForQuestionerModel()
            {
                VbQuestionPackViewModel = (VBQuestionPackViewModel)_questionService.GetQuestionPack(questionId, questionnaireId, Enums.QuestionType.Validation),
                QuestionnaireId = questionnaireId,
                FullQuestionPackDtos = _questionService.GetQuestionPacks(questionnaireId).Where(q => q.QuestionType != Enums.QuestionType.BenchMarking),
                MainQuestionPackViewModel = new MainQuestionPackViewModel()
                {
                    QuestionId = null
                },
                QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId)?.Name
            };

            return View("Index", model);
        }


        /// <summary>
        /// This ActionResult deletes the validation and main question.
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public ActionResult QuestionDelete(string questionId, string questionnaireId)
        {

            //todo: chekc the questionnaire status?
            //todo check the user access to question.
            //no matter it is vb or main they do the same thing
            TempData["questionnaireId"] = questionnaireId;

            _questionService.VBQuestionService.DeleteVB(questionId);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        
    }
}