using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Areas.Questioner.Persistence;
using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;
using QuestionnaireProject.Models;
using QuestionnaireProject.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Editor.Controllers
{
    [Authorize(Roles = "Editor")]
    public class QuestionForEditorController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly AnswerRepository _answer;
        private readonly QuestionRepositpry _question;
        // private readonly VBCorrectAnswerRepository _vbCorrectAnswer;
        //private readonly MultipleChoiceAnswerRepository _multipleChoiceAnswer;


        public QuestionForEditorController()
        {
            _context = new QuestionnaireEntities();
            _answer = new AnswerRepository(_context);
            _question = new QuestionRepositpry(_context);
            //_vbCorrectAnswer = new VBCorrectAnswerRepository(_context);
            //_multipleChoiceAnswer = new MultipleChoiceAnswerRepository(_context);
        }
        // GET: QuestionForEditor
        public ActionResult Index()
        {
            return View();
        }       

        public ActionResult EditQuestion(string questionId, string uri)
        {
            var question =_context.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question !=null)
            {
                List<string> answers = null;

                // This adds empty rows to the answers to be showable in the edit page
                if (question.QuestionType == (int) Enums.QuestionType.Validation)
                {
                    answers = _answer.GetAllByQuestionId(questionId).Select(v => v.AnswerText).ToList();
                    if (answers.Count <= (int)Enums.ValidationQuestion.MaxAnswersNumber)
                    {
                        var answerCount = answers.Count;
                        for (int i = 0; i < (int)Enums.ValidationQuestion.MaxAnswersNumber - answerCount; i++)
                        {
                            answers.Add("");
                        }
                    }
                }


                // This adds empty rows to the answers to be showable in the edit page
                if (question.QuestionType == (int)Enums.QuestionType.BenchMarking)
                {
                    answers = _answer.GetAllByQuestionId(questionId).Select(a => a.AnswerText).ToList();
                    if (answers.Count <= (int)Enums.BenchMarkingQuestion.MaxAnswersNumber)
                    {
                        var answerCount = answers.Count;
                        for (int i = 0; i < (int)Enums.BenchMarkingQuestion.MaxAnswersNumber - answerCount; i++)
                        {
                            answers.Add("");
                        }
                    }
                }

                // This adds empty rows to the answers to be showable in the edit page
                if (question.QuestionType == (int)Enums.QuestionType.Main ||
                    question.QuestionType == (int)Enums.QuestionType.Anatomical)
                {
                    answers = _answer.GetAllByQuestionId(questionId).Select(a => a.AnswerText).ToList();
                    if (answers.Count <= (int)Enums.MainQuestion.MaxAnswersNumber)
                    {
                        var answerCount = answers.Count;
                        for (int i = 0; i < (int)Enums.MainQuestion.MaxAnswersNumber - answerCount; i++)
                        {
                            answers.Add("");
                        }
                    }
                }

                var model = new EditorQuestionModel();
                
                // This part passes the data of validtaion and benchMarking quesitons to the view to edit
                model.QuestionType = (Enums.QuestionType)question.QuestionType;
                if (model.QuestionType == Enums.QuestionType.Validation || model.QuestionType == Enums.QuestionType.BenchMarking)
                {
                    model.VbQuestionViewModel = new VBQuestionPackViewModel()
                    {
                        QuestionText = question.Text,
                        AnswersText = answers,
                        //CorrectAnswerChoiceNumber = question.VBAnswers.Where(a=>a.IsCorrect==true).Select(a=>a.Id).FirstOrDefault()
                    };
                }


                // This part passes the data of anatomical and main quesitons to the view to edit
                if (model.QuestionType == Enums.QuestionType.Main ||
                    model.QuestionType == Enums.QuestionType.Anatomical)
                {
                    model.MainQuestionViewModel = new MainQuestionViewModel()
                    {
                        QuestionText = question.Text,
                        MediaType = (Enums.MediaType) question.MediaType,
                        //MediaUrl = question.Medias.FirstOrDefault().Url,
                        Answers = answers
                    };
                }
                return View(model);
            }

            return Redirect(uri);

        }

        public ActionResult EditVBConfirm(EditorQuestionModel model, string uri)
        {
            if (model.QuestionType == Enums.QuestionType.BenchMarking)
            {
                //todo: apply validaiton conditions
                var questionId = model.QuestionId;
                _question.Edit(questionId, model.VbQuestionViewModel.QuestionText);
                _answer.DeleteAllByQuestionId(questionId);
                var newAnswers = model.VbQuestionViewModel.AnswersText.ToList();
                _answer.CreateAll(questionId, newAnswers);
                //var correctAnswerExtractor = _vbCorrectAnswer.CorrectAnswerSelector(model.VbQuestionViewModel.CorrectAnswer, newAnswers);
                //if (correctAnswerExtractor != null)
                //{
                //    _vbCorrectAnswer.EditByQuestionId(questionId, (int)correctAnswerExtractor);
                //}
                _context.SaveChanges();
            }

            else if (model.QuestionType == Enums.QuestionType.Validation)
            {
                //todo: apply validaiton conditions
                var questionId = model.QuestionId;
                _question.Edit(questionId, model.VbQuestionViewModel.QuestionText);
                _answer.DeleteAllByQuestionId(questionId);
                var newAnswers = model.VbQuestionViewModel.AnswersText.ToList();
                _answer.CreateAll(questionId, newAnswers);
                //var correctAnswerExtractor = _vbCorrectAnswer.CorrectAnswerSelector(model.VbQuestionViewModel.CorrectAnswer, newAnswers);
                //if (correctAnswerExtractor != null)
                //{
                //    _vbCorrectAnswer.EditByQuestionId(questionId, (int)correctAnswerExtractor);
                //}
                _context.SaveChanges();
            }

            

            return Redirect(uri);
        }

        public ActionResult EditMainConfirm(EditorQuestionModel model, string uri)
        {
            if (model.QuestionType == Enums.QuestionType.Main)
            {
                //todo: apply validaiton conditions
                var questionId = model.QuestionId;
                _question.Edit(questionId, model.VbQuestionViewModel.QuestionText);
                var newAnswers = model.VbQuestionViewModel.AnswersText.ToList();
                _context.SaveChanges();
            }


            // This
            else if (model.QuestionType == Enums.QuestionType.Anatomical)
            {
                //todo: apply validaiton conditions
                var questionId = model.QuestionId;
                _question.Edit(questionId, model.VbQuestionViewModel.QuestionText);
                var newAnswers = model.VbQuestionViewModel.AnswersText.ToList();
                _context.SaveChanges();
            }
            return Redirect(uri);
        }       
        
    }
}