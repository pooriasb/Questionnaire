using QuestionnaireProject.Areas.Questioner.Persistence;
using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System.Web.Mvc;

// This module doesn't useful right now but it may become available in next versions
namespace QuestionnaireProject.Areas.Editor.Controllers
{
    [Authorize(Roles = "Editor")]
    public class DownloadFileForEditorController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly QuestionRepositpry _question;
        private readonly VBAnswerRepository _vbAnswers;
        private readonly MultipleChoiceAnswerRepository _multiplechoiceAnswer;
        private readonly NotificationRepository _notification;

        public DownloadFileForEditorController()
        {
            _context = new QuestionnaireEntities();
            _question = new QuestionRepositpry(_context);
            //_vbAnswers = new VBAnswerRepository(_context);
            //_multiplechoiceAnswer = new MultipleChoiceAnswerRepository(_context);
            _notification = new NotificationRepository(_context);

        }
        public ActionResult Index()
        {
            return View();
        }

       
    }
}