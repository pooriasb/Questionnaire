using Microsoft.AspNet.Identity;
using PagedList;
using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Models;
using QuestionnaireProject.Models.DTOS;
using QuestionnaireProject.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Editor.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "Editor")]
    public class QuestionnaireForEditorController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly NotificationRepository _notification;

        public QuestionnaireForEditorController()
        {
            _context = new QuestionnaireEntities();
            _notification = new NotificationRepository(_context);
        }
        

        /// <summary>
        /// This ActionResult shows a complete report of questionnaires.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="search"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult Index(int? page, string search, int? status, int? selft)
        {
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search;
            var userId = User.Identity.GetUserId();

            IQueryable<Questionnaire> questionnaires = _context.Questionnaires.Where(q=>q.Editor_Id == userId|| q.Editor_Id == null).OrderByDescending(q => q.DateCreated);
            //var model = new EditorQuestionnaireModel();
            if (status!=null)
            {
                questionnaires = questionnaires.Where(q => q.Status == status);
            }

            if (search != null)
            {
                questionnaires = questionnaires.Where(q => q.Name.Contains(search));

            }
            if (selft != null)
            {
                string editorID = User.Identity.GetUserId().ToString();
                questionnaires = questionnaires.Where(q => q.Editor_Id ==editorID );

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
                    UserId = questionnaire.Editor_Id,
                    Rate = questionnaire.Rate
                };
                dtos.Add(dto);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var model = dtos.ToPagedList(pageNumber, pageSize);
            if (selft != null)
            ViewBag.HeadTitle = "در اختیار من";
            else
                ViewBag.HeadTitle = SetHeadTitle(status);

            return View(model);
        }

        private string SetHeadTitle(int? status) {
            switch (status)
            {
                case 0:return "ارسال نشده";
                case 1: return "رد شده";
                case 2: return "در انتظار تایید ناظر";
                case 3: return "تایید شده (پرداخت نشده)";
                case 4: return "در حال اجرا";
                case 5: return "پایان یافته";
                default:return "همه";
            }
            
        }

        //todo: is the questions can be editable or not and also can editor delete the questions?
        /// <summary>
        /// This ActionResult shows a complete details of a questionnaire.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public ActionResult Questionnaire(string questionnaireId)
        {
            var userId = User.Identity.GetUserId();
            //this action result returns the all information about the questionnaire and also it can 
            // show the all relevant questioins.
            var questionnaire = _context.Questionnaires.Find(questionnaireId);            
            
            var questionnairePlus = _context.Questionnaires.Where(q => q.Id == questionnaireId).Select(x => new
            {
                x.AspNetUser.NationalCardImage,
                x.AspNetUser.National_Id,
                x.AspNetUser.FirstName,
                x.AspNetUser.LastName,
                x.EachPersonMoneyPrice,
                Condition = x.Conditions.Select(y => new
                {
                    y.MaxAge,
                    y.MinAge,
                    CityName = y.City.Name,
                    ProvinceName = y.Province.Name,
                    y.PeopleCount,
                    FieldName = y.StudyField.Name
                }).FirstOrDefault()

            }).FirstOrDefault();
            

            if (questionnairePlus != null && questionnaire != null)
            {
                questionnaire.Editor_Id = userId;
                _context.SaveChanges();
                var dto = new ReviewDto()
                {
                    QuestionnaireId = questionnaireId,
                    QuestionnaireName = questionnaire.Name,

                    // Added user info to ReviewDto
                    QuestionerInfo = new QuestionerInfo()
                    {
                        FirstName = questionnairePlus.FirstName,
                        LastName = questionnairePlus.LastName,
                        NationalCode = questionnairePlus.National_Id,
                        NationalCardImage = questionnairePlus.NationalCardImage
                    },
                    QuestionPacks = new List<QuestionPack>()

                };

                // Set the condtionDto to send to View
                dto.ConditionDto = new ConditionDto()
                {
                    Field = questionnairePlus.Condition.FieldName,
                    PeopleCount = questionnairePlus.Condition.PeopleCount,
                    EachPersonMoney = questionnairePlus.EachPersonMoneyPrice,
                    MinAge = questionnairePlus.Condition.MinAge,
                    MaxAge = questionnairePlus.Condition.MaxAge,
                    City = questionnairePlus.Condition.CityName,
                    Provience = questionnairePlus.Condition.ProvinceName

                };
                var questions = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId);
                foreach (var question in questions)
                {
                    var questionPack = new QuestionPack()
                    {
                        QuestionId = question.Id,
                        QuestionType = (Enums.QuestionType)question.QuestionType,
                        QuestionText = question.Text,
                        MediaType = (Enums.MediaType)question.MediaType,
                        EditorComment = question.EditorComment

                    };

                    if (questionPack.MediaType == Enums.MediaType.Film ||
                        questionPack.MediaType == Enums.MediaType.Picture)
                    {
                        questionPack.MediaUrl = _context.Medias.FirstOrDefault(m => m.Question_Id == question.Id).Url;
                    }

                    if (questionPack.QuestionType == Enums.QuestionType.Main ||
                        questionPack.QuestionType == Enums.QuestionType.MainWithPicture)
                    {
                        var answers = _context.Answers.Where(a => a.Question_Id == questionPack.QuestionId)
                            .Select(a => a.AnswerText).ToList();
                        questionPack.AnswersText = answers;
                    }

                    if (questionPack.QuestionType == Enums.QuestionType.BenchMarking ||
                        questionPack.QuestionType == Enums.QuestionType.Validation)
                    {
                        var answers = _context.Answers.Where(a => a.Question_Id == questionPack.QuestionId)
                            .Select(x => new
                            {
                                x.Id,
                                x.AnswerText,
                                x.IsCorrect
                            });
                        questionPack.AnswersText = answers.Select(a => a.AnswerText).ToList();
                        questionPack.AnswerId = answers.Select(a => a.Id).ToList();
                        questionPack.CorrectAnswer = answers.FirstOrDefault(a => a.IsCorrect == true).Id;

                    }

                    dto.QuestionPacks.Add(questionPack);
                }

                var model = new EditorReviewModel { ReviewDto = dto };
                return View(model);
            }

            return View("Index");


        }

        [System.Web.Mvc.HttpPost]
        public ActionResult SetCommentForQuestion([FromBody]string questionId, [FromBody]string comment)
        {           
            var question = _context.Questions.Find(questionId);
            if (question!=null)
            {
                question.EditorComment = comment == "" ? null : comment;
                _context.SaveChanges();
            }

            return Json(new{questionId, comment});
        }


        /// <summary>
        /// This ActionResult deletes the questionnaire.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public ActionResult Delete(string questionnaireId)
        {
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            if (questionnaire != null)
            {
                _context.Questionnaires.Remove(questionnaire);
                _context.SaveChanges();

            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// This ActionResult submit the editors assertion to the questioner.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult QuestionnaireAssertion(string questionnaireId, EditorReviewModel model)
        {
            try
            {
               
                if (Request.Form["Accept"] != null)
                {
                    var questionnaire = _context.Questionnaires
                        .Single(q => q.Id == questionnaireId);
                    if (questionnaire != null)
                    {

                        // new notificaiton for the accepted request to the user.
                        _notification.Add(new Notification()
                        {
                            DateCreated = DateTime.Now,
                            FromUser_Id = User.Identity.GetUserId(),
                            ToUser_Id = questionnaire.User_Id,
                            Title = $"پرسشنامه {questionnaire.Name} تایید گردید.",
                            Type = (int) Enums.NotificationType.Success,
                            Text = model.EditorAssert.AssertionComment,
                            IsSeen = false

                        });
                        
                        //change the status in context.
                        questionnaire.Status = (int)Enums.RequestStatus.Accepted;
                        questionnaire.DateConfirmed = DateTime.Now;
                        //questionnaire.DateExpire = DateTime.Now.AddDays(7);
                        _context.Entry<Questionnaire>(questionnaire).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
                else if (Request.Form["Reject"] != null)
                {
                    var questionnaire = _context.Questionnaires
                        .SingleOrDefault(q => q.Id == questionnaireId);
                    if (questionnaire != null)
                    {

                        // new notificaiton for the accepted request to the user.

                        _notification.Add(new Notification()
                        {
                            DateCreated = DateTime.Now,
                            FromUser_Id = User.Identity.GetUserId(),
                            ToUser_Id = questionnaire.User_Id,
                            Title = $"پرسشنامه {questionnaire.Name} رد گردید.",
                            Type = (int)Enums.NotificationType.Danger,
                            Text = model.EditorAssert.AssertionComment,
                            IsSeen = false

                        });  
                        
                        // the following lines add the new changes of status to the context.
                        questionnaire.Status = (int)Enums.RequestStatus.Rejected;
                        questionnaire.DateConfirmed = null;
                        questionnaire.DateExpire = null;
                        _context.Entry<Questionnaire>(questionnaire).State = System.Data.Entity.EntityState.Modified;

                        _context.SaveChanges();
                    }
                }


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        
        public ActionResult Fohshes()
        {
            var fohshes = new List<string> { "عکس", "حرومزاده", "چند", "دیوث"};
            return Json(fohshes);
        }

       


       


    }
}