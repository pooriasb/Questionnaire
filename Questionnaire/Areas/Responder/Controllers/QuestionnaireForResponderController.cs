using Microsoft.AspNet.Identity;
using PagedList;
using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Areas.Responder.Models;
using QuestionnaireProject.Areas.Responder.Persistence;
using QuestionnaireProject.Extensions;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;

namespace QuestionnaireProject.Areas.Responder.Controllers
{
    [Authorize(Roles = "Responder")]
    public class QuestionnaireForResponderController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly ResponseRepository _response;
        private readonly QuestionnaireRepository _questionnaire;

        public QuestionnaireForResponderController()
        {
            _context = new QuestionnaireEntities();
            _response = new ResponseRepository(_context);
            _questionnaire = new QuestionnaireRepository(_context);
        }






        public bool HasCondition(string questionnaireId)
        {
            string userId = User.Identity.GetUserId();
            var cityId = User.Identity.GetCityId().AsInt();
            int? provinceId = _context.Cities.FirstOrDefault(c => c.Id == cityId)?.Province_Id;
            var sex = User.Identity.GetSexId().AsInt();
            var age = DateTime.Now.Year - User.Identity.GetBirthDate().AsDateTime().Year;
            var field = User.Identity.GetStudyFieldId().AsInt();
            //این پارامتر بر اساس صلاحیت پاسخ دهنده مقادیر پرسشنامه را نمایش میدهد.
            var questionnaire = _context.Conditions.Where(c => c.MaxAge > age &&
                                                                c.MinAge < age &&
                                                                (c.Sex == sex || c.Sex == 2) &&
                                                                (c.Field_Id == field || c.Field_Id == null) &&
                                                                (c.Provience_Id == provinceId || c.Provience_Id == null) &&
                                                                (c.City_Id == cityId || c.City_Id == null)).Select(q => new
                                                                {
                                                                    q.Questionnaire.Id,
                                                                    q.Questionnaire.Name,
                                                                    q.Questionnaire.Rate,
                                                                    QuestionsNumber = q.Questionnaire.Questions.Count,
                                                                    EachPersonMoney = q.Questionnaire.EachPersonMoneyPrice,
                                                                    SpecialType = q.Questionnaire.SpecialOffer.Type,
                                                                    ExpiryDate = q.Questionnaire.DateExpire,
                                                                    StudyField = q.StudyField.Name,
                                                                    AnsweredByPerson = q.Questionnaire.AnsweredByPerson,
                                                                    MaxNumberOfPeopleToAnswer = q.PeopleCount
                                                                }).FirstOrDefault(q => q.Id == questionnaireId);


            var userResponses = _context.Responses.Where(r => r.User_Id == userId && r.DateFinished != null).Select(r => r.Questionnaire_Id);
            if (questionnaire != null)
            {
                if (questionnaire.ExpiryDate > DateTime.Now && !userResponses.Contains(questionnaire.Id) &&
                    questionnaire.AnsweredByPerson < questionnaire.MaxNumberOfPeopleToAnswer)
                {
                    return true;
                }
            }
            return false;
        }


        public class AvailableQuestionnairesModel
        {
            public IPagedList<AvailableQuestionnaireDto> AvailableQuestionnaireDtos { get; set; }
        }


        //todo add pagination. 
        public ActionResult AvailableQuestionnaires(int? page)
        {

            //todo check the conditions for the user. 
            //todo add pagination. 

            string userId = User.Identity.GetUserId();
            var cityId = User.Identity.GetCityId().AsInt();
            int? provinceId = _context.Cities.FirstOrDefault(c => c.Id == cityId)?.Province_Id;
            var sex = User.Identity.GetSexId().AsInt();
            var age = DateTime.Now.Year - User.Identity.GetBirthDate().AsDateTime().Year;
            var field = User.Identity.GetStudyFieldId().AsInt();

            //todo add the date constraint
            //todo cityId and sex are not completed. 
            var model = new AvailableQuestionnairesModel();

            //این پارامتر بر اساس صلاحیت پاسخ دهنده مقادیر پرسشنامه را نمایش میدهد.
            var questionnaires = _context.Conditions.Where(c => c.MaxAge > age &&
                                                                c.MinAge < age &&
                                                                (c.Sex == sex || c.Sex == 2) &&
                                                                (c.Field_Id == field || c.Field_Id == null) &&
                                                                (c.Provience_Id == provinceId || c.Provience_Id == null) &&
                                                                (c.City_Id == cityId || c.City_Id == null)).Select(q => new
                                                                {
                                                                    q.Questionnaire.Id,
                                                                    q.Questionnaire.Name,
                                                                    q.Questionnaire.Rate,
                                                                    QuestionsNumber = q.Questionnaire.Questions.Count,
                                                                    EachPersonMoney = q.Questionnaire.EachPersonMoneyPrice,
                                                                    SpecialType = q.Questionnaire.SpecialOffer.Type,
                                                                    ExpiryDate = q.Questionnaire.DateExpire,
                                                                    StudyField = q.StudyField.Name,
                                                                    AnsweredByPerson = q.Questionnaire.AnsweredByPerson,
                                                                    MaxNumberOfPeopleToAnswer = q.PeopleCount,
                                                                    QuestionnaireCategory = q.Questionnaire.QuestionnaireCategory.Name,
                                                                    q.Questionnaire.DateCreated
                                                                });

            var availableQuestionnaireDtos = new List<AvailableQuestionnaireDto>();

            var userResponses = _context.Responses.Where(r => r.User_Id == userId && r.DateFinished != null).Select(r => r.Questionnaire_Id);

            foreach (var q in questionnaires)
            {
                if (q.ExpiryDate > DateTime.Now && !userResponses.Contains(q.Id) && q.AnsweredByPerson < q.MaxNumberOfPeopleToAnswer)
                {
                    var availableQuestionnaire = new AvailableQuestionnaireDto()
                    {
                        PeapleCount = q.MaxNumberOfPeopleToAnswer - q.AnsweredByPerson,
                        DateToExpire = (q.DateCreated.Value.AddDays(30) - DateTime.Now).Days,
                        QuestionnaireId = q.Id,
                        QuestionnaireName = q.Name,
                        EachPersonMoney = q.EachPersonMoney,
                        QuestionsNumber = q.QuestionsNumber,
                        Rate = q.Rate,
                        Special = q.SpecialType,
                        StudyField = q.StudyField,
                        QuestionnaireCategory = q.QuestionnaireCategory

                    };
                    availableQuestionnaireDtos.Add(availableQuestionnaire);
                }
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            model.AvailableQuestionnaireDtos = availableQuestionnaireDtos.OrderByDescending(a => a.Special == 0).ToPagedList(pageNumber, pageSize);
            //model.AvailableQuestionnaireDtos = model.AvailableQuestionnaireDtos?.OrderByDescending(a => a.Special);
            return View(model);
        }

        public class QuestionModel
        {
            public string QuestionnaireId { get; set; }
            public string QuestionId { get; set; }
            public string Response { get; set; }
            public QuestionPack QuestionPack { get; set; }

        }
        public ActionResult UnFinishedQuestionnaire() {
            string userId = User.Identity.GetUserId();
           
            //ViewBag.Packages 
            var response = _context.Responses.Where(x => x.DateFinished == null && x.User_Id == userId).ToList().Select(x=>x.Questionnaire_Id).ToList();
            var packages = new List<Questionnaire>();
            foreach (string item in response)
            {
                Questionnaire model = _context.Questionnaires.Where(x => x.Id == item).SingleOrDefault();
                if (model!=null)
                {
                    packages.Add(model);
                }
            }
            ViewBag.Packages = packages;
            return View();
        }

        private void FirstLevelQuestionGenerator(string questionnaireId)
        {
            var userId = User.Identity.GetUserId();
            var questions = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId &&
                                                          q.QuestionType == (int)Enums.QuestionType.BenchMarking);
            var response = _response.CreateResponse(userId, questionnaireId);
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            if (questionnaire!=null)
            {
                questionnaire.AnsweredByPerson++;

                if (questions.Count() != 0)
                {

                    List<UsersRespons> userResponses = new List<UsersRespons>();
                    foreach (var q in questions)
                    {
                        UsersRespons newQuestionnairePackage = new UsersRespons()
                        {
                            Response_Id = response.Id,
                            Question_Id = q.Id,
                            User_Id = userId
                        };
                        userResponses.Add(newQuestionnairePackage);
                    }

                    _context.UsersResponses.AddRange(userResponses);


                }   
            }
            

            _context.SaveChanges();

        }

        public ActionResult FirstLevelQuestionShowOneByOne(string questionnaireId)
        {
            string userId = User.Identity.GetUserId();
            var responseInResponses =
                _context.Responses.SingleOrDefault(r => r.User_Id == userId && r.Questionnaire_Id == questionnaireId);
            if (responseInResponses == null)
            {
                FirstLevelQuestionGenerator(questionnaireId);
                responseInResponses =
                _context.Responses.SingleOrDefault(r => r.User_Id == userId && r.Questionnaire_Id == questionnaireId);
            }
            UsersRespons response = _context.UsersResponses.FirstOrDefault(r =>
              r.Respons.Questionnaire_Id == questionnaireId && r.Response == null &&
              r.Response_Id == responseInResponses.Id);
          

            if (response != null)
            {
                var question = _context.Questions.FirstOrDefault(q => q.Id == response.Question_Id &&
                                                                      q.QuestionType == (int)Enums.QuestionType.BenchMarking);
                if (question != null)
                {
                    QuestionModel questionModel = new QuestionModel();
                    var questionPack = new QuestionPack()
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Text,
                        CorrectAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect == true)?.Id,
                        AnswersText = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => a.AnswerText).ToList(),
                        AnswerId = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => a.Id).ToList(),
                        QuestionType = (Enums.QuestionType)question.QuestionType,
                        QuestionnaireId = questionnaireId

                    };
                    questionModel.QuestionPack = questionPack;
                    questionModel.QuestionnaireId = questionnaireId;
                    questionModel.QuestionId = question.Id;
                    TempData["QuestionModel"] = questionModel;
                    TempData["ResponseId"] = response.Id;

                    return View(questionModel);

                }

            }

            return RedirectToAction("FirstLevelResponseAssertion", new { questionnaireId = questionnaireId });



        }

        public ActionResult FirstLevelResponseStoreOneByOne(QuestionModel model)
        {

            if (TempData["QuestionModel"] is QuestionModel && TempData["ResponseId"] is long)
            {
                QuestionModel question = TempData["QuestionModel"] as QuestionModel;// get question model for question pack
                long? responseId = TempData["ResponseId"] as long?;
                if (question.QuestionPack.QuestionType == Enums.QuestionType.BenchMarking)// check if the question is benchMarking
                {
                    if (question.QuestionPack.AnswerId.Contains(model.Response))//check if user response is in our responses
                    {
                        UsersRespons response = _context.UsersResponses.Find(responseId);// find response for set true 
                        if (response != null)
                        {
                            response.Response = model.Response;
                            response.IsCorrect = question.QuestionPack.CorrectAnswer == model.Response;// set User Answer True Or False
                        }
                        //TODO: delete respose and rediret to some where if is correct answer is false
                    }
                }


                _context.SaveChanges();
            }

            return RedirectToAction("FirstLevelQuestionShowOneByOne", new { questionnaireId = model.QuestionnaireId });
        }

        public ActionResult FirstLevelResponseAssertion(string questionnaireId)
        {
            string userId = User.Identity.GetUserId();

            var benchmarking = _context.UsersResponses.Where(r => r.Respons.Questionnaire_Id == questionnaireId && r.Respons.User_Id == userId)
                .Join(_context.Questions,
                    responses => responses.Question_Id,
                    questions => questions.Id,
                    (responses, questions) => new { responses, questions })
                .Where(q => q.questions.QuestionType == 0);
            int benchmarkingTrueCount = benchmarking.Count(b => b.responses.IsCorrect == true);
            int benchmarkingTotalCount = benchmarking.Count();
            if (benchmarkingTotalCount == benchmarkingTrueCount)
            {

                return RedirectToAction("SecondLevelQuestionShowOneByOne", new { questionnaireId = questionnaireId });
            }
            else
            {
                Respons userResponse = _context.Responses.SingleOrDefault(r => r.User_Id == userId &&
                                                                               r.Questionnaire_Id == questionnaireId);

                if (userResponse != null)
                {
                    userResponse.DateFinished = DateTime.Now;
                    userResponse.IsBanned = true;
                    // todo : questioner peaple count -- 
                    // delete user responses
                    if (DeleteUserResposesAndQuestionnairCount(userResponse.Id)) {

                        TempData["suspend"] = "شما لایق پاسخ گویی به این پرسشنامه نمیباشید";
                    }
                    else
                    {
                        TempData["suspend"] = "خطایی رخ داده است";
                    }

                }

                _context.SaveChanges();
            }
            //return View("AvailableQuestionnaires");
            return RedirectToAction("AvailableQuestionnaires");
        }
        private bool DeleteUserResposesAndQuestionnairCount(long id = -1) {
            try
            {
                if (id != -1)
                {
                    _context.Questionnaires.Find(_context.Responses.Find(id).Questionnaire_Id).AnsweredByPerson--;
                    List<UsersRespons> ResponsesToDelete = _context.UsersResponses.Where(x => x.Response_Id == id).ToList();
                    _context.UsersResponses.RemoveRange(ResponsesToDelete);

                    _context.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {

                return false;
            }
         
           
        }
        private void SecondLevelQuestionGenerator(string questionnaireId)
        {
            var userId = User.Identity.GetUserId();
            var questions = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId && q.QuestionType != (int)Enums.QuestionType.BenchMarking);

            //todo try catch for the following singleOrDefault
            var response =
                _context.Responses.SingleOrDefault(r => r.User_Id == userId && r.Questionnaire_Id == questionnaireId);
            if (response != null)
            {
                List<UsersRespons> userResponses = new List<UsersRespons>();
                foreach (var q in questions)
                {
                    UsersRespons newQuestionnairePackage = new UsersRespons()
                    {
                        Response_Id = response.Id,
                        Question_Id = q.Id,
                        User_Id = userId
                    };
                    userResponses.Add(newQuestionnairePackage);
                }


                var rand = new Random();

                var adminCount = 0;
                if (userResponses.Count <5)
                    adminCount = 2;
                else
               adminCount =    userResponses.Count() / 5;


                var chancey = _context.AdminQuestions.Include(a => a.AdminAnswers).ToList().OrderBy(a => rand.Next()).Take(adminCount).ToList();
                foreach (var c in chancey)
                {
                    UsersRespons newQuestionnairePackage = new UsersRespons()
                    {
                        Response_Id = response.Id,
                        Question_Id = c.Id,
                        User_Id = userId
                    };
                    userResponses.Insert(rand.Next(userResponses.Count()), newQuestionnairePackage);
                }

                _context.UsersResponses.AddRange(userResponses);
                _context.SaveChanges();
            }

        }

        public ActionResult SecondLevelQuestionShowOneByOne(string questionnaireId)
        {
            string userId = User.Identity.GetUserId();
            var checkResponseInUserResponsesCount = _context.UsersResponses.Count(r =>
                r.Respons.User_Id == userId && r.Respons.Questionnaire_Id == questionnaireId);
            var questionsInQuestionnaireCount = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId);
            if (questionsInQuestionnaireCount > checkResponseInUserResponsesCount)
            {
                SecondLevelQuestionGenerator(questionnaireId);
            }
            //else
            //{
            //    return HttpNotFound();
            //}

            UsersRespons response = _context.UsersResponses.FirstOrDefault(r => r.Respons.Questionnaire_Id == questionnaireId && r.Response == null && r.Respons.User_Id == userId);
            if (response != null)
            {
                var questionAdmin = _context.AdminQuestions.FirstOrDefault(q => q.Id == response.Question_Id);
                if (questionAdmin != null)
                {
                    QuestionModel questionModel = new QuestionModel();
                    var questionPack = new QuestionPack()
                    {
                        QuestionId = questionAdmin.Id,
                        QuestionText = questionAdmin.Text,
                        CorrectAnswer = questionAdmin.AdminAnswers.FirstOrDefault(a => a.IsCorrect == true)?.Id,
                        AnswersText = questionAdmin.AdminAnswers.Where(a => a.AdminQuestion_Id == questionAdmin.Id).Select(a => a.AnswerText).ToList(),
                        AnswerId = questionAdmin.AdminAnswers.Where(a => a.AdminQuestion_Id == questionAdmin.Id).Select(a => a.Id).ToList(),
                        QuestionType = Enums.QuestionType.SiteBenchMarking,
                        QuestionnaireId = questionnaireId

                    };
                    questionModel.QuestionPack = questionPack;
                    questionModel.QuestionnaireId = questionnaireId;
                    questionModel.QuestionId = questionAdmin.Id;
                    TempData["QuestionModel"] = questionModel;
                    TempData["ResponseId"] = response.Id;

                    return View(questionModel);
                }

                // if that wasn't an admin question.
                var question = _context.Questions.FirstOrDefault(q => q.Id == response.Question_Id);
                if (question != null)
                {
                    QuestionModel questionModel = new QuestionModel();
                    var questionPack = new QuestionPack()
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Text,
                        CorrectAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect == true)?.Id,
                        AnswersText = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => a.AnswerText).ToList(),
                        AnswerId = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => a.Id).ToList(),
                        QuestionType = (Enums.QuestionType)question.QuestionType,
                        QuestionnaireId = questionnaireId, 
                        MediaType = (Enums.MediaType)question.MediaType

                    };

                    if (questionPack.MediaType == Enums.MediaType.Picture)
                    {
                        questionPack.MediaUrl = question.Medias.FirstOrDefault(m => m.Question_Id == question.Id)?.Url;
                    }else if (questionPack.MediaType == Enums.MediaType.Film)
                    {
                        questionPack.MediaUrl = question.Medias.FirstOrDefault(m => m.Question_Id == question.Id)?.Url;
                    }
                    questionModel.QuestionPack = questionPack;
                    questionModel.QuestionnaireId = questionnaireId;
                    questionModel.QuestionId = question.Id;
                    TempData["QuestionModel"] = questionModel;
                    TempData["ResponseId"] = response.Id;

                    return View(questionModel);

                }

            }

            return RedirectToAction("SecondLevelResponseAssertion", new { questionnaireId = questionnaireId });


        }

        public ActionResult SecondLevelResponseStoreOneByOne(QuestionModel model)
        {
            if (TempData["QuestionModel"] is QuestionModel && TempData["ResponseId"] is long)
            {
                QuestionModel question = TempData["QuestionModel"] as QuestionModel;
                long? responseId = TempData["ResponseId"] as long?;
                if (question.QuestionPack.QuestionType == Enums.QuestionType.Anatomical)
                {
                    UsersRespons response = _context.UsersResponses.Find(responseId);
                    if (response != null)
                    {
                        response.Response = model.Response;
                        response.IsCorrect = null;
                    }

                }
                else if (question.QuestionPack.QuestionType == Enums.QuestionType.Main ||
                         question.QuestionPack.QuestionType == Enums.QuestionType.MainWithPicture)
                {
                    if (question.QuestionPack.AnswerId.Contains(model.Response))
                    {
                        UsersRespons response = _context.UsersResponses.Find(responseId);
                        if (response != null)
                        {
                            response.Response = model.Response;
                            response.IsCorrect = null;
                        }
                    }
                }
                else if (question.QuestionPack.QuestionType == Enums.QuestionType.BenchMarking ||
                         question.QuestionPack.QuestionType == Enums.QuestionType.Validation)
                {
                    if (question.QuestionPack.AnswerId.Contains(model.Response))
                    {

                        UsersRespons response = _context.UsersResponses.Find(responseId);
                        if (response != null)
                        {
                            response.Response = model.Response;
                            response.IsCorrect = question.QuestionPack.CorrectAnswer == model.Response;
                            if (response.IsCorrect == false)
                            {

                                int Deleteid = response.Response_Id.Value;
                                Respons responseToSetBannrd = _context.Responses.Find(Deleteid);
                                    if (DeleteUserResposesAndQuestionnairCount(Deleteid))
                                    {
                                    responseToSetBannrd.DateFinished = DateTime.Now;
                                    responseToSetBannrd.IsBanned = true;
                                    _context.SaveChanges();
                                        TempData["suspend"] = "شما لایق پاسخ گویی به این پرسشنامه نمیباشید";
                                    }
                                    else
                                    {
                                        TempData["suspend"] = "خطایی رخ داده است";
                                    }
                                   return RedirectToAction("AvailableQuestionnaires");


                            }
                        }

                    }
                }
                else if (question.QuestionPack.QuestionType == Enums.QuestionType.SiteBenchMarking)
                {
                    if (question.QuestionPack.AnswerId.Contains(model.Response))
                    {

                        UsersRespons response = _context.UsersResponses.Find(responseId);
                        if (response != null)
                        {
                            response.Response = model.Response;
                            response.IsCorrect = question.QuestionPack.CorrectAnswer == model.Response;
                        }

                    }
                }

                _context.SaveChanges();
            }

            return RedirectToAction("SecondLevelQuestionShowOneByOne", new { questionnaireId = model.QuestionnaireId });
        }
        public ActionResult SecondLevelResponseAssertion(string questionnaireId)
        {
            var userId = User.Identity.GetUserId();

            var nullResponses = _context.UsersResponses.Any(r => r.Respons.Questionnaire_Id == questionnaireId &&
                                                                 r.Respons.User_Id == userId &&
                                                                 r.Respons == null);
            if (nullResponses)
            {
                return RedirectToAction("SecondLevelQuestionShowOneByOne", new { questionnaireId = questionnaireId });

            }
            //continue to rating
            
            var validation = _context.UsersResponses.Where(r => r.Respons.Questionnaire_Id == questionnaireId)
                .Join(_context.Questions,
                    response => response.Question_Id,
                    question => question.Id,
                    (response, question) => new { response, question }).Where(q => q.question.QuestionType == 1).ToList();

            int validationTrueCount = validation.Count(v => v.response.IsCorrect == true);
            int validationCount = validation.Count();

            if (validationCount != 0)
            {
                int score = Convert.ToInt32(((decimal)validationTrueCount / validationCount) * 100);
                _response.SetValidationScore(score, questionnaireId, userId);
            }

            var admin = _context.UsersResponses.Where(r => r.Respons.Questionnaire_Id == questionnaireId)
                .Join(_context.AdminQuestions,
                    adminResponse => adminResponse.Question_Id,
                    adminQuestion => adminQuestion.Id,
                    (adminResponse, adminQuestion) => new { adminResponse, adminQuestion });
            var adminTrue = admin.Count(y => y.adminResponse.IsCorrect == true);
            var adminTotal = admin.Count();
            //todo try catch if has any other record
            Respons userResponse = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == questionnaireId &&
                                                                           r.User_Id == userId);
            var questionnaire = _context.Questionnaires.Find(questionnaireId);

            if (userResponse != null && questionnaire!=null)
            {
                if (adminTrue == adminTotal)
                {
                    userResponse.DateFinished = DateTime.Now;
                    userResponse.IsBanned = false;
                    questionnaire.RemainMoney -= questionnaire.EachPersonMoneyPrice;
                  //  _context.AspNetUsers.Find(userId).MoneyBalance += questionnaire.EachPersonMoneyPrice.Value;
                }
                else
                {
                    //HACK : I think we must delete responses here
                    userResponse.DateFinished = DateTime.Now;
                    userResponse.IsBanned = true;
                  //  questionnaire.AnsweredByPerson--;
                    if (DeleteUserResposesAndQuestionnairCount(userResponse.Id))
                    {
                        _context.SaveChanges();
                        TempData["suspend"] = "شما در سوالات راستی آزمایی ادمین شکست خوردید.";
                        return RedirectToAction("AvailableQuestionnaires");

                    }
                    else
                    {
                        TempData["suspend"] = "خطایی رخ داده است";
                    }
                }

                
            }

            _context.SaveChanges();

            return RedirectToAction("QuestionnaireRating", new{questionnaireId});
        }

        public ActionResult QuestionnaireRating()
        {

            //Task.Run();
            //todo show the money and the rating
            return View();
        }

        public void QuestionnaireRateSetter(string questionnaireId)
        {
            var rate = _context.Responses.Where(q => q.Questionnaire_Id == questionnaireId && q.Rate != null).Select(r => r.Rate).Average();
            double questionnaireRate = Math.Ceiling(rate ?? 0);
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            if (questionnaire != null)
            {
                questionnaire.Rate = (int)questionnaireRate;
                _context.SaveChanges();

            }


        }

        public ActionResult RatingConfirm(int? rate, string questionnaireId)
        {
            var userId = User.Identity.GetUserId();
            var checkResponse = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == questionnaireId &&
                                                                        r.User_Id == userId);
            int rateTodb = rate ?? 0;
            _response.SetRate(userId, questionnaireId, rateTodb);
            _context.SaveChanges();
            Task.Run(() => QuestionnaireRateSetter(questionnaireId));
            return RedirectToAction("AvailableQuestionnaires");
        }






        //public ActionResult BenchMarkingQuestion(string questionnaireId)
        //{

        //    try
        //    {
        //        if (!HasCondition(questionnaireId))
        //        {
        //            //بررسی میکند که شرایط پاسخگویی به پرسش را دارد.
        //            return HttpNotFound();
        //        }

        //        var userId = User.Identity.GetUserId();
        //        var checkResponse = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == questionnaireId &&
        //                                                                    r.User_Id == userId);

        //        if (checkResponse != null)
        //        {
        //            if (checkResponse.IsBanned == true || checkResponse.DateFinished != null)
        //            {
        //                //بررسی میکند که در صورت پاسخ دادن قبلی به این پرسشنامه و یا بن شدن اررور برگرداند
        //                // در غیر اینصورت به صفحه پرسشهای اصلی هدایت میشود.
        //                //todo 404 error
        //                return RedirectToAction("AvailableQuestionnaires");
        //            }
        //            if (checkResponse.IsBanned == false)
        //            {
        //                TempData["questionnaireId"] = questionnaireId;
        //                return RedirectToAction("MainQuestions");
        //            }


        //        }
        //        else
        //        {



        //            if (TempData["questionnaireId"] != null)
        //            {
        //                questionnaireId = TempData["questionnaireId"] as string;
        //            }

        //            // this is due to when there is no benchMarking question and would be redirected to the main questions
        //            var serverVm = _response.FromServer(questionnaireId).Where(q => q.QuestionType == Enums.QuestionType.BenchMarking).ToList();
        //            if (serverVm.Count == 0)
        //            {
        //                _response.CreateResponse(userId, questionnaireId);
        //                _context.SaveChanges();
        //                TempData["questionnaireId"] = questionnaireId;
        //                return RedirectToAction("MainQuestions");
        //            }
        //            /////////////////

        //            var query = _context.Questions
        //                .Where(q =>
        //                    q.Questionnaire_Id == questionnaireId &&
        //                    q.QuestionType == (int)Enums.QuestionType.BenchMarking)
        //                .Select(x => new
        //                {
        //                    x.Id,
        //                    x.Text,
        //                    x.VBAnswers,
        //                });


        //            var dto = new List<ResponderQuestionPackDto>();
        //            //var vm = new List<ResponderQuestionPackViewModel>();
        //            foreach (var q in query)
        //            {
        //                var quesitonPackVm = new ResponderQuestionPackViewModel();
        //                var questionPack = new ResponderQuestionPackDto()
        //                {
        //                    AnswersText = q.VBAnswers.Where(x => x.Question_Id == q.Id).Select(a => a.Answer).ToList(),
        //                    AnswerIds = q.VBAnswers.Where(x => x.Question_Id == q.Id).Select(a => a.Id).ToList(),
        //                    QuestionId = q.Id,
        //                    QuestionText = q.Text
        //                };
        //                dto.Add(questionPack);
        //                //vm.Add(quesitonPackVm);
        //            }

        //            var model = new BenchMarkingQuestionModel()
        //            {
        //                ResponderQuestionPackDtos = dto,
        //                QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId)?.Name,
        //                QuestionnaireId = questionnaireId
        //            };

        //            if (TempData["modelBenchMarking"] is BenchMarkingQuestionModel returnModel)
        //            {
        //                model.ResponderQuestionPackViewModels = returnModel.ResponderQuestionPackViewModels;

        //            }

        //            return View(model);
        //        }

        //        return RedirectToAction("AvailableQuestionnaires");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        //خطایی رخ داده است هدایت به صفحه ارور
        //        throw;
        //    }




        //}


        //[HttpPost]
        //public ActionResult BenchMarkingEvaluation(BenchMarkingQuestionModel model, string questionnaireId)
        //{
        //    if (!HasCondition(questionnaireId))
        //    {
        //        //بررسی میکند که شرایط پاسخگویی به پرسش را دارد.
        //        return RedirectToAction("AvailableQuestionnaires", "QuestionnaireForResponder");
        //    }
        //    var userId = User.Identity.GetUserId();
        //    var checkResponse = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == questionnaireId &&
        //                                                                r.User_Id == userId);

        //    //if (checkResponse != null)
        //    //{
        //    //    if (checkResponse.IsBanned == true || checkResponse.DateFinished != null)
        //    //    {

        //    //        //todo 404 error
        //    //        return HttpNotFound();
        //    //    }
        //    //    if (checkResponse.IsBanned == false)
        //    //    {
        //    //        return RedirectToAction("MainQuestions", new { questionnaireId });
        //    //    }


        //    //}

        //    TempData["modelBenchMarking"] = model;
        //    TempData["questionnaireId"] = questionnaireId;
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["Error"] = "پاسخ به تمامی سوالات اجباری میباشد.";
        //        return RedirectToAction("BenchMarkingQuestion");
        //    }
        //    //todo evaluation of admin questions and validation questions
        //    var serverVm = _response.FromServer(model.QuestionnaireId).Where(q => q.QuestionType == Enums.QuestionType.BenchMarking).ToList();
        //    var userVm = model.ResponderQuestionPackViewModels;
        //    if (serverVm.Count == 0)
        //    {
        //        _response.CreateResponse(userId, questionnaireId);
        //        _context.SaveChanges();
        //        TempData["questionnaireId"] = questionnaireId;
        //        return RedirectToAction("MainQuestions");
        //    }
        //    if (serverVm.Count == userVm.Count)
        //    {
        //        var validatorModel = new List<ResponderQuesionPackValidatorModel>();
        //        foreach (var pair in userVm.Zip(serverVm, Tuple.Create))
        //        {
        //            var newValidatorModel = new ResponderQuesionPackValidatorModel();

        //            newValidatorModel.QuestionPackServerViewModel = pair.Item2;
        //            newValidatorModel.ResponderQuestionPackViewModel = pair.Item1;

        //            validatorModel.Add(newValidatorModel);
        //        }

        //        bool mrEvaluator = _response.MrEvaluator(validatorModel);

        //        if (mrEvaluator)
        //        {
        //            var bmEvaluator = _response.BenchMarkingEvaluation(validatorModel);
        //            if (bmEvaluator)
        //            {
        //                _response.CreateResponse(userId, questionnaireId);
        //                _response.MrCreator(userId, validatorModel);
        //                _context.SaveChanges();
        //                TempData["questionnaireId"] = questionnaireId;
        //                return RedirectToAction("MainQuestions");

        //            }
        //            else
        //            {
        //                //در صورت غلط بودن پاسخگویی رکوردی در دیتابیس ذخیره میشود که شامل تاریخ پاسخ گویی 
        //                // و بن نمودن یوزر جهت پاسخگویی مجدد به پرسش است.
        //                _context.Responses.Add(new Respons()
        //                {
        //                    DateFinished = DateTime.Now,
        //                    IsBanned = true,
        //                    User_Id = userId,
        //                    Questionnaire_Id = questionnaireId,
        //                });
        //                _context.SaveChanges();
        //            }

        //            return RedirectToAction("AvailableQuestionnaires", "QuestionnaireForResponder");
        //        }

        //        return RedirectToAction("AvailableQuestionnaires", "QuestionnaireForResponder");
        //        //todo make a final decision...
        //    }

        //    return RedirectToAction("AvailableQuestionnaires", "QuestionnaireForResponder");



        //}




        //public ActionResult MainQuestions(string questionnaireId)
        //{
        //    if (questionnaireId == null)
        //    {
        //        questionnaireId = TempData["questionnaireId"] as string;
        //    }
        //    var userId = User.Identity.GetUserId();
        //    var checkResponse = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == questionnaireId &&
        //                                                                r.User_Id == userId);

        //    if (checkResponse != null)
        //    {
        //        if (checkResponse.IsBanned == true || checkResponse.DateFinished != null)
        //        {

        //            //todo 404 error
        //            return HttpNotFound();
        //        }
        //    }
        //    else
        //    {
        //        return HttpNotFound();
        //    }




        //    var returnModel = TempData["modelMain"] as BenchMarkingQuestionModel;


        //    if (TempData["questionnaireId"] != null)
        //    {
        //        questionnaireId = TempData["questionnaireId"] as string;
        //    }


        //    //todo evaluation
        //    var query = _context.Questions
        //        .Where(q =>
        //            q.Questionnaire_Id == questionnaireId &&
        //            q.QuestionType != (int)Enums.QuestionType.BenchMarking)
        //        .Select(x => new
        //        {
        //            x.Id,
        //            x.Text,
        //            x.QuestionType,
        //            x.VBAnswers,
        //            x.MultipleChoiceAnswers,
        //            x.Medias,
        //            x.MediaType
        //        });


        //    var dto = new List<ResponderQuestionPackDto>();
        //    //var vm = new List<ResponderQuestionPackViewModel>();
        //    foreach (var q in query)
        //    {
        //        if (q.QuestionType == (int)Enums.QuestionType.Main ||
        //            q.QuestionType == (int)Enums.QuestionType.Anatomical ||
        //            q.QuestionType == (int)Enums.QuestionType.MainWithPicture)
        //        {
        //            var questionPackVm = new ResponderQuestionPackViewModel();
        //            var questionPack = new ResponderQuestionPackDto()
        //            {
        //                QuestionId = q.Id,
        //                QuestionType = (Enums.QuestionType)q.QuestionType,
        //                QuestionText = q.Text,
        //                AnswersText = q.MultipleChoiceAnswers.Where(x => x.Question_Id == q.Id).Select(a => a.Answer).ToList(),
        //                AnswerIds = q.MultipleChoiceAnswers.Where(x => x.Question_Id == q.Id).Select(a => a.Id).ToList(),
        //                MediaType = (Enums.MediaType)q.MediaType,
        //            };

        //            if (q.MediaType != (int)Enums.MediaType.NoMedia)
        //            {
        //                questionPack.MediaUrl = q.Medias.SingleOrDefault(m => m.Question_Id == q.Id)?.Url;
        //            }
        //            dto.Add(questionPack);
        //            //vm.Add(questionPackVm);
        //        }
        //        else if (q.QuestionType == (int)Enums.QuestionType.Validation)
        //        {
        //            var questionPackVm = new ResponderQuestionPackViewModel();
        //            var questionPack = new ResponderQuestionPackDto()
        //            {
        //                QuestionId = q.Id,
        //                QuestionType = (Enums.QuestionType)q.QuestionType,
        //                QuestionText = q.Text,
        //                AnswersText = q.VBAnswers.Where(x => x.Question_Id == q.Id).Select(a => a.Answer).ToList(),
        //                AnswerIds = q.VBAnswers.Where(x => x.Question_Id == q.Id).Select(a => a.Id).ToList(),
        //            };

        //            dto.Add(questionPack);
        //            //vm.Add(questionPackVm);
        //        }


        //    }


        //    //this part adds admin questions to the dto and disperse it randomly in questions
        //    var rand = new Random();
        //    var adminCount = dto.Count() / 5;
        //    var chancey = _context.AdminQuestions.Include(a => a.AdminAnswers).ToList().OrderBy(a => rand.Next()).Take(adminCount).ToList();

        //    for (int i = 0; i < adminCount; i++)
        //    {
        //        var item = new ResponderQuestionPackDto()
        //        {
        //            QuestionId = chancey[i].Id,
        //            QuestionText = chancey[i].Text,
        //            AnswerIds = chancey[i].AdminAnswers.Select(a => a.Id).ToList(),
        //            AnswersText = chancey[i].AdminAnswers.Select(a => a.AnswerText).ToList(),

        //        };
        //        dto.Insert(rand.Next(dto.Count()), item);
        //        //vm.Add(new ResponderQuestionPackViewModel());

        //    }

        //    // end of random question dispersion in questionnaire randomly
        //    var model = new MainQuestionPackModel()
        //    {
        //        ResponderQuestionPackDtos = dto,
        //        QuestionnaireName = _questionnaire.GetSingle(questionnaireId).Name,
        //        QuestionnaireId = questionnaireId
        //    };

        //    if (returnModel != null)
        //    {
        //        model.ResponderQuestionPackViewModels = returnModel.ResponderQuestionPackViewModels;

        //    }

        //    return View(model);



        //}


        //[HttpPost]
        //public ActionResult MainQuestionsEvaluation(string questionnaireId, MainQuestionPackModel model)
        //{
        //    if (!HasCondition(questionnaireId))
        //    {
        //        //بررسی میکند که شرایط پاسخگویی به پرسش را دارد.
        //        return HttpNotFound();
        //    }
        //    var userId = User.Identity.GetUserId();
        //    var checkResponse = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == questionnaireId &&
        //                                                                r.User_Id == userId);

        //    //if (checkResponse != null)
        //    //{
        //    //    if (checkResponse.IsBanned == true || checkResponse.DateFinished != null)
        //    //    {

        //    //        //todo 404 error
        //    //        return HttpNotFound();
        //    //    }
        //    //    if (checkResponse.IsBanned == false)
        //    //    {
        //    //        return RedirectToAction("MainQuestions", new { questionnaireId });
        //    //    }


        //    //}

        //    TempData["modelMain"] = model;
        //    TempData["questionnaireId"] = questionnaireId;
        //    if (!ModelState.IsValid)
        //    {
        //        TempData["Error"] = "پاسخ به تمامی سوالات اجباری میباشد.";
        //        return RedirectToAction("MainQuestions");
        //    }



        //    //this method checks the received admin questions from server and extract the intersects
        //    var adminQvm = _context.AdminQuestions.Select(a => a.Id)
        //        .Intersect(model.ResponderQuestionPackViewModels.Select(r => r.QuestionId)).ToList();

        //    // this method find all admin answers in db and put them in a variable.
        //    var adminVm = _context.AdminQuestions.Where(a => adminQvm.Contains(a.Id)).ToList();
        //    var userAdminVm = model.ResponderQuestionPackViewModels.Where(r => adminVm.Any(e => r.QuestionId.Contains(e.Id))).ToList();


        //    //this if checks the answers correctness
        //    foreach (var uavm in userAdminVm)
        //    {
        //        var adminQuesion = adminVm.FirstOrDefault(a => a.Id == uavm.QuestionId);
        //        if (adminQuesion != null)
        //        {
        //            if (adminQuesion.AdminAnswers.FirstOrDefault(a => a.IsCorrect == true)?.Id != uavm.Answer)
        //            {
        //                _response.BanUser(userId, questionnaireId);
        //            }
        //        }


        //    }




        //    var serverVm = _response.FromServer(model.QuestionnaireId).Where(q => q.QuestionType != Enums.QuestionType.BenchMarking).ToList();
        //    var userVm = model.ResponderQuestionPackViewModels.Where(r => !adminVm.Any(e => r.QuestionId.Contains(e.Id))).ToList();
        //    if (serverVm.Count == userVm.Count)
        //    {
        //        var validatorModel = new List<ResponderQuesionPackValidatorModel>();
        //        foreach (var pair in userVm.Zip(serverVm, Tuple.Create))
        //        {
        //            var newValidatorModel =
        //                new ResponderQuesionPackValidatorModel
        //                {
        //                    QuestionPackServerViewModel = pair.Item2,
        //                    ResponderQuestionPackViewModel = pair.Item1
        //                };


        //            validatorModel.Add(newValidatorModel);
        //        }

        //        //this line evaluate that user sends valid data.
        //        var mrEvaluator = _response.MrEvaluator(validatorModel);

        //        if (mrEvaluator)
        //        {
        //            var score = _response.ValidationEvaluation(validatorModel);
        //            _response.SetValidationScore(score, questionnaireId, userId);
        //            _response.MrCreator(userId, validatorModel);
        //            var response = _context.Responses.Single(r => r.Questionnaire_Id == questionnaireId &&
        //                                                          r.User_Id == userId);
        //            response.DateFinished = DateTime.Now;

        //            var questionnaire = _context.Questionnaires.Find(questionnaireId);
        //            questionnaire.AnsweredByPerson++;

        //            questionnaire.Rate = 0;
        //            questionnaire.AnsweredByPerson = 0;

        //            //decrease the remain amount of money.
        //            questionnaire.RemainMoney -= questionnaire.EachPersonMoneyPrice;
        //            _context.SaveChanges();
        //            return RedirectToAction("QuestionnaireRating", new { questionnaireId = questionnaireId });

        //        }

        //    }

        //    return RedirectToAction("QuestionnaireRating", new { questionnaireId = questionnaireId });
        //}









        public class AnsweredQuestionDto
        {
            public string QuestionnaireName { get; set; }
            public DateTime? DateFinished { get; set; }
            public DateTime? DatePaid { get; set; }
            public decimal? ReceivedMoney { get; set; }
        }

        public class AnsweredQuestionsModel
        {
            public IPagedList<AnsweredQuestionDto> AnsweredQuestionDtos { get; set; }
        }

        public ActionResult AnsweredQuesionnaires(int? page ,int filter = 0)
        {
            //todo evaluate condition for the user;
            ViewBag.filter = filter;
            var userId = User.Identity.GetUserId();
            var responses = _context.Responses.Include(r => r.Questionnaire)
                .Where(r => r.User_Id == userId)
                .Select(r => new
                {
                    r.Questionnaire.QuestionnaireCategory.Name,
                    r.DateFinished,
                    r.DatePaid,
                    Money = r.Questionnaire.EachPersonMoneyPrice
                });

            var model = new AnsweredQuestionsModel();
            var answeredQuestionDtos = new List<AnsweredQuestionDto>();

            foreach (var r in responses)
            {
                var dto = new AnsweredQuestionDto()
                {
                    QuestionnaireName = r.Name,
                    DatePaid = r.DatePaid,
                    DateFinished = r.DateFinished,
                    ReceivedMoney = r.Money
                };
                answeredQuestionDtos.Add(dto);
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            model.AnsweredQuestionDtos = answeredQuestionDtos.ToPagedList(pageNumber, pageSize);
            return View(model);
        }


    }

}