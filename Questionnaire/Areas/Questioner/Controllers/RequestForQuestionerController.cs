using Microsoft.AspNet.Identity;
using PagedList;
using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Areas.Questioner.Models;
using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Questioner.Controllers
{
    public class RequestForQuestionerController : Controller
    {
        private readonly QuestionnaireEntities _context;

        public RequestForQuestionerController()
        {
            _context = new QuestionnaireEntities();
        }

        private List<RequestDto> Dto()
        {

            //todo: I can add a view to receive all data once
            var userId = User.Identity.GetUserId();
            var allRequests = _context.QuestionnaireRequests.Where(r => r.User_Id == userId);
            var dtos = new List<RequestDto>();
            foreach (var request in allRequests)
            {
                var dto = new RequestDto()
                {
                    QuestionnaireId = request.Questionnaire_Id, //todo: remove nullable from questionnaireId in requests
                    QuestionnaireName = _context.Questionnaires.Where(q => q.Id == request.Questionnaire_Id).Select(q => q.Name).FirstOrDefault(),
                    //RequestStatus = (Enums.RequestStatus)request.Status,
                    EditorComment = request.EditorComment,
                    UpdateDate = (DateTime)request.DateUpdated

                };
                dtos.Add(dto);
            }
            return dtos;

        }
        // GET: RequestForQuestioner
        //public ActionResult Index()
        //{
        //    var model = new RequestModel();
        //    model.RequestDtos = Dto();
        //    return View(model);
        //}

        public ActionResult Index(int? page, string search, int? status)
        {
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search;

            string userId = User.Identity.GetUserId();
            IQueryable<Questionnaire> questionnaires = _context.Questionnaires.Where(q=>q.User_Id == userId).OrderByDescending(q => q.DateCreated);
            //var model = new EditorQuestionnaireModel();
            if (status != null)
            {
                questionnaires = questionnaires.Where(q => q.Status == status);
            }

            if (search != null)
            {
                questionnaires = questionnaires.Where(q => q.Name.Contains(search));

            }



            var dtos = new List<QuestionnairePack>();
            foreach (var questionnaire in questionnaires)
            {

                //todo: add automapper
                var dto = new QuestionnairePack()
                {
                    Id = questionnaire.Id,
                    Name = questionnaire.Name,
                    Status = questionnaire.Status,
                    DateCreated = questionnaire.DateCreated,
                    DateConfirmed = questionnaire.DateConfirmed,
                    DatePaid = questionnaire.DatePaid,
                    DateExpire = questionnaire.DateExpire,
                    SpecialId = questionnaire.Special_Id,
                    //UserId = questionnaire.User_Id,
                    Rate = questionnaire.Rate
                };
                dtos.Add(dto);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var model = dtos.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        /// <summary>
        /// This ActionResult Delete a record of the questionner.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public  ActionResult Delete(string questionnaireId)
        {
          //  return View("Index");
            //todo control the userId, accept, create, reject

            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            if (questionnaire != null)
            {                
                _context.Questionnaires.Remove(questionnaire);
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        public  ActionResult FinishBeforeExpireDate(string questionnaireId)
        {

            //todo control the userId, accept, create, reject
            var userId = User.Identity.GetUserId();
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            if (questionnaire != null && questionnaire.User_Id == userId)
            {
                questionnaire.DateExpire = DateTime.Now;
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }




        public ActionResult QuestionnaireSummary(string questionnaireId)
        {
            var userId = User.Identity.GetUserId();
            var model = new QuestionnaireSummaryModel()
            {
                QuestionnaireSummaryDto = new QuestionnaireSummaryDto()
                {
                    QuestionnaireId = questionnaireId,
                    QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId)?.Name,
                    BenchMarkingQuestion = new QuestionCounter()
                    {
                        Total = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId && q.QuestionType == (int)Enums.QuestionType.BenchMarking),
                    },
                    ValidationQuestion = new QuestionCounter()
                    {
                        Total = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId && q.QuestionType == (int)Enums.QuestionType.Validation),
                    },
                    MainQuestion = new QuestionCounter()
                    {
                        Total = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId && (q.QuestionType == (int)Enums.QuestionType.Main || q.QuestionType == (int)Enums.QuestionType.MainWithPicture)),
                        NoMedia = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId && q.QuestionType == (int)Enums.QuestionType.Main && q.MediaType == (int)Enums.MediaType.NoMedia),
                        HasPicture = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId &&                                                                   
                                                                   (q.QuestionType == (int)Enums.QuestionType.MainWithPicture ||
                                                                    (q.MediaType == (int)Enums.MediaType.Picture && q.QuestionType == (int)Enums.QuestionType.Main))),
                        HasFilm = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId && q.QuestionType == (int)Enums.QuestionType.Main && q.MediaType == (int)Enums.MediaType.Film),

                    },
                    AnatomicalQuestion = new QuestionCounter()
                    {
                        Total = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId && q.QuestionType == (int)Enums.QuestionType.Anatomical),
                        NoMedia = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId && q.QuestionType == (int)Enums.QuestionType.Anatomical && q.MediaType == (int)Enums.MediaType.NoMedia),
                        HasPicture = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId && q.QuestionType == (int)Enums.QuestionType.Anatomical && q.MediaType == (int)Enums.MediaType.Picture),
                        HasFilm = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId && q.QuestionType == (int)Enums.QuestionType.Anatomical && q.MediaType == (int)Enums.MediaType.Film),

                    },
                    //RequestStatus = (Enums.RequestStatus) 
                }
            };

            var questionnaire = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId);
            if (questionnaire?.Status != null && questionnaire.User_Id==userId)
            {
                model.QuestionnaireSummaryDto.RequestStatus = (Enums.RequestStatus)questionnaire.Status;

            }
            return View(model);
        }

        public ActionResult SubmitToEditor(string questionnaireId)
        {
            var questionnaire = _context.Questionnaires.Single(q => q.Id == questionnaireId);
            questionnaire.Status = (int) Enums.RequestStatus.Pending;
            var userId = User.Identity.GetUserId();
            var query = _context.QuestionnaireRequests.FirstOrDefault(r => r.Questionnaire_Id == questionnaireId);
            if (query == null)
            {
                //var newRequest = new QuestionnaireRequest()
                //{
                //    DateUpdated = DateTime.Now,
                //    User_Id = User.Identity.GetUserId(),
                //    Questionnaire_Id = questionnaireId,
                //    //Status = (int)Enums.RequestStatus.Pending,
                //    EditorComment = "پرسشنامه جهت بررسی ویراستار ارسال شده است."
                //};
                //_context.QuestionnaireRequests.Add(newRequest);

                var notificationToAdmin = new Notification()
                {
                    DateCreated = DateTime.Now,
                    FromUser_Id = userId,
                    ToUser_Id = null,
                    Title = "درخواست جدید",
                    Text = $"شما پرسشنامه ای با عنوان {questionnaire.Name} دریافت نموده اید.",
                    Type = (int)Enums.NotificationType.Warning,
                    IsSeen = false
                };

                var newNotification = new Notification()
                {
                    DateCreated = DateTime.Now,
                    ToUser_Id = User.Identity.GetUserId(),
                    Title = "ارسال جهت برررسی",
                    Text = "کاربر گرامی پرسشنامه شما جهت بررسی ویراستار ارسال گردید." + questionnaire.Name,
                    Type = (int) Enums.NotificationType.Success,
                    IsSeen = false
                };

                _context.Notifications.Add(newNotification);
            }


            _context.SaveChanges();
            return RedirectToAction("Index", "Questioner");
        }


    }
}