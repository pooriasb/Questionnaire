using Microsoft.AspNet.Identity;
using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Areas.Responder.Controllers;
using QuestionnaireProject.Areas.Responder.Persistence;
using QuestionnaireProject.Extensions;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;

namespace QuestionnaireProject.Controllers.API
{

    public class AndroidController : ApiController
    {
        private readonly QuestionnaireEntities _context;
        private readonly ResponseRepository _response;
        private readonly QuestionnaireRepository _questionnaire;

        public AndroidController()
        {
            _context = new QuestionnaireEntities();
            _response = new ResponseRepository(_context);
            _questionnaire = new QuestionnaireRepository(_context);
        }

        public bool IsInRole()
        {
            if (User.IsInRole("Responder") && User.Identity.IsAuthenticated)
            {
                return true;
            }
            return false;
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
        

        [System.Web.Http.HttpPost]
        public IHttpActionResult Questionnaires()
        {
            if (!IsInRole())
            {
                return Json("lf");
            }
            string userId = User.Identity.GetUserId();
            var notificationCount = _context.Notifications.Count(n => n.ToUser_Id == userId && n.IsSeen == false);
            var answeredCount = _context.Responses.Count(r => r.User_Id == userId);
            var money = _context.Responses.Where(r => r.DatePaid == null)
                .Sum(r => r.Questionnaire.EachPersonMoneyPrice) +
                        _context.AspNetUsers.Find(userId)?.MoneyBalance +
                        _context.MoneyToReferers.Where(m=>m.ToUser_Id == userId && m.DatePaid == null).Sum(m=>m.MoneyToUser);
            var cityId = User.Identity.GetCityId().AsInt();
            int? provinceId = _context.Cities.FirstOrDefault(c => c.Id == cityId)?.Province_Id;
            var sex = User.Identity.GetSexId().AsInt();
            var age = DateTime.Now.Year - User.Identity.GetBirthDate().AsDateTime().Year;
            var field = User.Identity.GetStudyFieldId().AsInt();


            var questionnairs = _context.Conditions.Where(c => c.MaxAge > age &&
                                                               c.MinAge < age &&
                                                               (c.Sex == sex || c.Sex == 2) &&
                                                               (c.Field_Id == field || c.Field_Id == null) &&
                                                               (c.Provience_Id == provinceId || c.Provience_Id == null) &&
                                                               (c.City_Id == cityId || c.City_Id == null));
            var questionnaire = _context.Questionnaires.Select(x => new
            {
                //name = x.Name,
                category = x.QuestionnaireCategory.Name,
                id = x.Id,
                specialId = x.SpecialOffer.Type ?? 10,
                dateExpire = x.DateExpire.ToString(),
                answeredByPerson = x.AnsweredByPerson ?? 0,
                rate = x.Rate ?? 0,
                eachPesronMoney = x.EachPersonMoneyPrice,
                benchMarkQuestionsAmount = x.Questions.Count(q => q.QuestionType == (int)Enums.QuestionType.BenchMarking),
                otherQuestionsAmount = x.Questions.Count(q => q.QuestionType != (int)Enums.QuestionType.BenchMarking),
            }).ToList();
            return Json(new { notificationCount, answeredCount, questionnaireCount = questionnaire.Count, money, questionnaire = questionnaire });
        }


        public class QuestionModel
        {
            public string QuestionnaireId { get; set; }
            public string QuestionId { get; set; }
            public string QuestionText { get; set; }
            public List<string> AnswersText { get; set; }
            public List<string> AnswerId { get; set; }
            public Enums.QuestionType QuestionType { get; set; }
            public string MediaUrl { get; set; }
            public Enums.MediaType MediaType { get; set; }
            public string CorrectAnswer { get; set; }

        }

        public class ResponseModel
        {
            public string QuestionnaireId { get; set; }
            //public string QuestionId { get; set; }
            public string Response { get; set; }
        }


        private void FirstLevelQuestionGenerator(string questionnaireId)
        {
            var userId = User.Identity.GetUserId();
            var questions = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId &&
                                                          q.QuestionType == (int)Enums.QuestionType.BenchMarking);
            var response = _response.CreateResponse(userId, questionnaireId);
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            if (questionnaire != null)
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

        public IHttpActionResult FirstLevelQuestionShowOneByOne([FromBody]Questionnaire model)
        {
            string userId = User.Identity.GetUserId();
            var responseInResponses =
                _context.Responses.SingleOrDefault(r => r.User_Id == userId && r.Questionnaire_Id == model.Id);
            if (responseInResponses == null)
            {
                FirstLevelQuestionGenerator(model.Id);
            }

            UsersRespons response = _context.UsersResponses.FirstOrDefault(r => r.Respons.Questionnaire_Id == model.Id && r.Response == null && r.Response_Id == responseInResponses.Id);
            if (response != null)
            {
                var question = _context.Questions.FirstOrDefault(q => q.Id == response.Question_Id &&
                                                                      q.QuestionType == (int)Enums.QuestionType.BenchMarking);
                if (question != null)
                {
                    QuestionModel questionModel = new QuestionModel()
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Text,
                        //CorrectAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect == true)?.Id,
                        //AnswersText = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => a.AnswerText).ToList(),
                        //AnswerId = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => a.Id).ToList(),
                        QuestionType = (Enums.QuestionType)question.QuestionType,
                        QuestionnaireId = model.Id
                    };
                    var Answers = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => new { AnswerText = a.AnswerText, AnswerId = a.Id } );
                    questionModel.QuestionnaireId = model.Id;

                    return Json(new { questionModel, Answers});

                }

            }

            return Json(new{result=true,message="مرحله اول به پایان رسید."});



        }

        public IHttpActionResult FirstLevelResponseStoreOneByOne([FromBody]ResponseModel model)
        {
            var userId = User.Identity.GetUserId();
            var response = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == model.QuestionnaireId && r.User_Id == userId);
           
            
            var userResponse = _context.UsersResponses.FirstOrDefault(r => r.Respons.Questionnaire_Id == model.QuestionnaireId && r.Response == null && r.Response_Id == response.Id);
            var question = _context.Questions.SingleOrDefault(q => q.Id == userResponse.Question_Id);
            var answers =
                _context.Answers.Where(a => a.Question_Id == question.Id).ToList();
            if (question!=null && userResponse!=null && response!=null && userId!=null && response.User_Id == userId/*TempData["QuestionModel"] is QuestionnaireForResponderController.QuestionModel && TempData["ResponseId"] is long*/)
            {
                
                if (question.QuestionType == (int) Enums.QuestionType.BenchMarking)
                {
                    if (answers.Select(a=>a.Id).Contains(model.Response))
                    {

                        UsersRespons userResponseToSave = _context.UsersResponses.Find(userResponse.Id);
                        if (userResponseToSave != null)
                        {
                            userResponseToSave.Response = model.Response;
                            if (answers.FirstOrDefault(a => a.IsCorrect == true)?.Id == model.Response)
                            {
                                userResponseToSave.IsCorrect =true;
                            }
                            
                        }
                        _context.SaveChanges();
                        return Json(new { result=true, message = "پاسخ ذخیره شد. مرحله اول" });

                    }
                }


                
            }

            return Json(new{ result = false, message ="پاسخ ذخیره نشد. مرحله اول"});
        }

        public IHttpActionResult FirstLevelResponseAssertion([FromBody]Questionnaire model)
        {
            string userId = User.Identity.GetUserId();

            var benchmarking = _context.UsersResponses.Where(r => r.Respons.Questionnaire_Id == model.Id && r.Respons.User_Id == userId)
                .Join(_context.Questions,
                    responses => responses.Question_Id,
                    questions => questions.Id,
                    (responses, questions) => new { responses, questions })
                .Where(q => q.questions.QuestionType == 0);
            int benchmarkingTrueCount = benchmarking.Count(b => b.responses.IsCorrect == true);
            int benchmarkingTotalCount = benchmarking.Count();
            if (benchmarkingTotalCount == benchmarkingTrueCount)
            {

                return Json(new{result=true,message="تمامی پاسخ های مرحله اول صحیح است."});
            }
            else
            {
                Respons userResponse = _context.Responses.SingleOrDefault(r => r.User_Id == userId &&
                                                                               r.Questionnaire_Id == model.Id);

                if (userResponse != null)
                {
                    userResponse.DateFinished = DateTime.Now;
                    userResponse.IsBanned = true;
                }

                _context.SaveChanges();
            }
            return Json(new {result=false, message = "یکی یا چند تا از پاسخ های مرحله اول غلط است." });
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
                var adminCount = userResponses.Count() / 5;
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

        public IHttpActionResult SecondLevelQuestionShowOneByOne([FromBody]Questionnaire model)
        {
            string userId = User.Identity.GetUserId();
            var checkResponseInUserResponsesCount = _context.UsersResponses.Count(r =>
                r.Respons.User_Id == userId && r.Respons.Questionnaire_Id == model.Id);
            var questionsInQuestionnaireCount = _context.Questions.Count(q => q.Questionnaire_Id == model.Id);
            if (questionsInQuestionnaireCount > checkResponseInUserResponsesCount)
            {
                SecondLevelQuestionGenerator(model.Id);
            }
            //else
            //{
            //    return Json(new { message = "در سیستم خطایی روی داده است." });
            //}

            UsersRespons response = _context.UsersResponses.FirstOrDefault(r => r.Respons.Questionnaire_Id == model.Id && r.Response == null && r.Respons.User_Id == userId);
            if (response != null)
            {
                var questionAdmin = _context.AdminQuestions.FirstOrDefault(q => q.Id == response.Question_Id);
                if (questionAdmin != null)
                {
                    QuestionnaireForResponderController.QuestionModel questionModel = new QuestionnaireForResponderController.QuestionModel();
                    var questionPack = new QuestionPack()
                    {
                        QuestionId = questionAdmin.Id,
                        QuestionText = questionAdmin.Text,
                        //CorrectAnswer = questionAdmin.AdminAnswers.FirstOrDefault(a => a.IsCorrect == true)?.Id,
                        AnswersText = questionAdmin.AdminAnswers.Where(a => a.AdminQuestion_Id == questionAdmin.Id).Select(a => a.AnswerText).ToList(),
                        AnswerId = questionAdmin.AdminAnswers.Where(a => a.AdminQuestion_Id == questionAdmin.Id).Select(a => a.Id).ToList(),
                        QuestionType = Enums.QuestionType.SiteBenchMarking,
                        QuestionnaireId = model.Id

                    };
                    questionModel.QuestionPack = questionPack;
                    questionModel.QuestionnaireId = model.Id;
                    questionModel.QuestionId = questionAdmin.Id;
                    //TempData["QuestionModel"] = questionModel;
                    //TempData["ResponseId"] = response.Id;

                    return Json(questionModel);
                }

                // if that wasn't an admin question.
                var question = _context.Questions.FirstOrDefault(q => q.Id == response.Question_Id);
                if (question != null)
                {
                    QuestionnaireForResponderController.QuestionModel questionModel = new QuestionnaireForResponderController.QuestionModel();
                    var questionPack = new QuestionPack()
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Text,
                        //CorrectAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect == true)?.Id,
                        //AnswersText = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => a.AnswerText).ToList(),
                        //AnswerId = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => a.Id).ToList(),
                        QuestionType = (Enums.QuestionType)question.QuestionType,
                        QuestionnaireId = model.Id,
                        MediaType = (Enums.MediaType)question.MediaType

                    };

                    if (questionPack.MediaType == Enums.MediaType.Picture)
                    {
                        questionPack.MediaUrl = question.Medias.FirstOrDefault(m => m.Question_Id == question.Id)?.Url;
                    }
                    else if (questionPack.MediaType == Enums.MediaType.Film)
                    {
                        questionPack.MediaUrl = question.Medias.FirstOrDefault(m => m.Question_Id == question.Id)?.Url;
                    }
                    questionModel.QuestionPack = questionPack;
                    questionModel.QuestionnaireId = model.Id;
                    questionModel.QuestionId = question.Id;

                    var Answers = question.Answers.Where(a => a.Question_Id == question.Id).Select(a => new { AnswerText = a.AnswerText, AnswerId = a.Id } );
                    //TempData["QuestionModel"] = questionModel;
                    //TempData["ResponseId"] = response.Id;

                    return Json(new { questionModel, Answers });

                }

            }

            return Json(new {result=true, message = "پرسشهای مرحله دوم تمام شد. ارزیابی" });

        }

        public IHttpActionResult SecondLevelResponseStoreOneByOne([FromBody]ResponseModel model)
        {            
            var userId = User.Identity.GetUserId();
            var response = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == model.QuestionnaireId && r.User_Id == userId);


            var userResponse = _context.UsersResponses.FirstOrDefault(r => r.Respons.Questionnaire_Id == model.QuestionnaireId && r.Response == null && r.Response_Id == response.Id);
            var question = _context.Questions.SingleOrDefault(q => q.Id == userResponse.Question_Id);
            List<Answer> answers = null;
            if (question!=null && question.QuestionType != (int)Enums.QuestionType.Anatomical)
            {
                answers =
                    _context.Answers.Where(a => a.Question_Id == question.Id).ToList();
            }
            

            if (response!=null && userId!=null && userResponse!=null && response.User_Id==userId)
            {                
                if (question!=null && question.QuestionType == (int)Enums.QuestionType.Anatomical)
                {
                    UsersRespons newUserResponse = _context.UsersResponses.Find(userResponse.Id);
                    if (newUserResponse != null)
                    {
                        newUserResponse.Response = model.Response;
                        newUserResponse.IsCorrect = null;
                        _context.SaveChanges();
                        return Json(new { result = true, message = "موفق در ثبت پاسخ" });
                    }

                }
                else if (question != null && (question.QuestionType == (int)Enums.QuestionType.Main ||
                         question.QuestionType == (int)Enums.QuestionType.MainWithPicture))
                {
                    if (answers != null && answers.Select(a=>a.Id).Contains(model.Response))
                    {
                        UsersRespons newUserResponse = _context.UsersResponses.Find(userResponse.Id);
                        if (newUserResponse != null)
                        {
                            newUserResponse.Response = model.Response;
                            newUserResponse.IsCorrect = null;
                            _context.SaveChanges();
                            return Json(new { result = true, message = "موفق در ثبت پاسخ" });
                        }
                    }
                }
                else if (question != null && (question.QuestionType == (int)Enums.QuestionType.BenchMarking ||
                         question.QuestionType == (int)Enums.QuestionType.Validation))
                {
                    if (answers!=null && answers.Select(a=>a.Id).Contains(model.Response))
                    {

                        UsersRespons newUserResponse = _context.UsersResponses.Find(userResponse.Id);
                        if (newUserResponse != null)
                        {
                            newUserResponse.Response = model.Response;
                            if (answers.FirstOrDefault(a => a.IsCorrect == true)?.Id == model.Response)
                            {
                                newUserResponse.IsCorrect = true;
                                _context.SaveChanges();
                                return Json(new { result = true, message = "موفق در ثبت پاسخ" });
                            }
                        }

                    }
                }
                else if (question==null)
                {
                    var adminAnswers = _context.AdminAnswers.Where(a => a.AdminQuestion_Id == userResponse.Question_Id);
                    if (adminAnswers.Select(a=>a.Id).Contains(model.Response))
                    {

                        UsersRespons newUserResponse = _context.UsersResponses.Find(userResponse.Id);
                        if (newUserResponse != null)
                        {
                            newUserResponse.Response = model.Response;
                            if (adminAnswers.FirstOrDefault(a => a.IsCorrect == true)?.Id == model.Response)
                            {
                                newUserResponse.IsCorrect = true;
                                _context.SaveChanges();
                                return Json(new { result = true, message = "موفق در ثبت پاسخ" });

                            }
                        }

                    }
                }

            }

            return Json(new {result=false, message = " مشکل در تبت پاسخ"});
        }
        public IHttpActionResult SecondLevelResponseAssertion([FromBody]Questionnaire model)
        {
            var userId = User.Identity.GetUserId();

            var nullResponses = _context.UsersResponses.Any(r => r.Respons.Questionnaire_Id == model.Id &&
                                                                 r.Respons.User_Id == userId &&
                                                                 r.Response == null);
            if (nullResponses)
            {
                return Json(new { result=false, message = "سوال بعدی مرحله دوم رو صدا بزن" });
            }
            //continue to rating

            var notNullResponses = _context.UsersResponses.Count(r => r.Respons.Questionnaire_Id == model.Id &&
                                                                 r.Respons.User_Id == userId &&
                                                                 r.Response != null);

            var countQuestionnaireQustions = _context.Questions.Count(q => q.Questionnaire_Id == model.Id);

            if (countQuestionnaireQustions <= notNullResponses)
            {
                var validation = _context.UsersResponses.Where(r => r.Respons.Questionnaire_Id == model.Id)
                .Join(_context.Questions,
                    response => response.Question_Id,
                    question => question.Id,
                    (response, question) => new { response, question }).Where(q => q.question.QuestionType == 1).ToList();

                int validationTrueCount = validation.Count(v => v.response.IsCorrect == true);
                int validationCount = validation.Count();

                if (validationCount != 0)
                {
                    int score = Convert.ToInt32(((decimal)validationTrueCount / validationCount) * 100);
                    _response.SetValidationScore(score, model.Id, userId);
                }

                var admin = _context.UsersResponses.Where(r => r.Respons.Questionnaire_Id == model.Id)
                    .Join(_context.AdminQuestions,
                        adminResponse => adminResponse.Question_Id,
                        adminQuestion => adminQuestion.Id,
                        (adminResponse, adminQuestion) => new { adminResponse, adminQuestion });
                var adminTrue = admin.Count(y => y.adminResponse.IsCorrect == true);
                var adminTotal = admin.Count();
                //todo try catch if has any other record
                Respons userResponse = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == model.Id &&
                                                                               r.User_Id == userId);
                var questionnaire = _context.Questionnaires.Find(model.Id);

                if (userResponse != null && questionnaire != null)
                {
                    if (adminTrue == adminTotal)
                    {
                        userResponse.DateFinished = DateTime.Now;
                        userResponse.IsBanned = false;
                        questionnaire.RemainMoney -= questionnaire.EachPersonMoneyPrice;
                    }
                    else
                    {
                        userResponse.DateFinished = DateTime.Now;
                        userResponse.IsBanned = true;
                        questionnaire.AnsweredByPerson++;
                    }


                }
                _context.SaveChanges();

                return Json(new { result = true,message = "برو برای امتیاز دهی سوال..." });
            }


            return Json(new { result = false, message = "سوال بعدی مرحله دوم رو صدا بزن" });



        }
        

        private void QuestionnaireRateSetter(string questionnaireId)
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

        public IHttpActionResult RatingConfirm([FromBody]Questionnaire model)
        {
            var userId = User.Identity.GetUserId();
            var checkResponse = _context.Responses.SingleOrDefault(r => r.Questionnaire_Id == model.Id &&
                                                                        r.User_Id == userId);
            int rateTodb = model.Rate ?? 0;
            _response.SetRate(userId, model.Id, rateTodb);
            _context.SaveChanges();
            Task.Run(() => QuestionnaireRateSetter(model.Id));
            return Json(new {result=true, message = "پایان نظرسنجی" });

        }




        //[Serializable]
        //public class BenchMarkingQuestionDto
        //{
        //    public string QuestionnaireId { get; set; }
        //    public string QuestionnaireName { get; set; }
        //    public List<ResponderQuestionPackDto> ResponderQuestionPackDtos { get; set; }

        //}

        

        //[System.Web.Http.HttpPost]
        //public IHttpActionResult BenchMark([FromBody]Questionnaire questionnaire)
        //{

        //    if (!HasCondition(questionnaire.Id))
        //    {
        //        return Json(new { result = 0, message = "شما صلاحیت پاسخگویی به پرسشنامه را ندارید." });
        //    }

        //    var query = _context.Questions
        //        .Where(q =>
        //            q.Questionnaire_Id == questionnaire.Id &&
        //            q.QuestionType == (int)Enums.QuestionType.BenchMarking)
        //        .Select(x => new
        //        {
        //            questionId = x.Id,
        //            questionText = x.Text,
        //            answers = x.Answers.Select(v => new
        //            {
        //                id = v.Id,
        //                text = v.AnswerText
        //            }),
        //        }).ToList();

        //    return Json(new { questions = query });
        //}

        //[Serializable]
        //public class BenchMarkingQuestionViewModel
        //{
        //    public string QuestionnaireId { get; set; }
        //    public List<ResponderQuestionPackViewModel> Responses { get; set; }

        //}

        //public IHttpActionResult BenchMarkEvaluation(BenchMarkingQuestionViewModel model)
        //{
        //    if (!HasCondition(model.QuestionnaireId))
        //    {
        //        return Json(new { result = 0, message = "شما صلاحیت پاسخگویی به پرسشنامه را ندارید." });
        //    }
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var userId = User.Identity.GetUserId();
        //        var serverVm = _response.FromServer(model.QuestionnaireId).Where(q => q.QuestionType == Enums.QuestionType.BenchMarking).ToList();
        //        var userVm = model.Responses;
        //        if (serverVm.Count == userVm.Count)
        //        {
        //            var validatorModel = new List<ResponderQuesionPackValidatorModel>();
        //            foreach (var pair in userVm.Zip(serverVm, Tuple.Create))
        //            {
        //                var newValidatorModel = new ResponderQuesionPackValidatorModel();

        //                newValidatorModel.QuestionPackServerViewModel = pair.Item2;
        //                newValidatorModel.ResponderQuestionPackViewModel = pair.Item1;

        //                validatorModel.Add(newValidatorModel);
        //            }

        //            var mrEvaluator = _response.MrEvaluator(validatorModel);

        //            if (mrEvaluator)
        //            {
        //                var bmEvaluator = _response.BenchMarkingEvaluation(validatorModel);
        //                if (bmEvaluator)
        //                {
        //                    _response.CreateResponse(userId, model.QuestionnaireId);
        //                    _response.MrCreator(userId, validatorModel);
        //                    _context.SaveChanges();
        //                    return Json(new { result = 1, message = "لیاقت سنجی بدون غلط و با موفقیت پاسخ داده شد." });

        //                }

        //                return Json(new { result = 0, message = "لیاقت سنجی غلط پاسخ داده شد." });
        //            }

        //            //todo make a final decision...
        //        }
        //        return Json(new { result = 0, message = "خطایی رخ داده است." });


        //    }

        //    return Json(new { result = 0 });

        //}



        //[Serializable]
        //public class MainQuestionPackDto
        //{
        //    public string QuestionnaireId { get; set; }
        //    public string QuestionnaireName { get; set; }
        //    public List<ResponderQuestionPackDto> ResponderQuestionPackDtos { get; set; }
        //    //public List<ResponderQuestionPackViewModel> ResponderQuestionPackViewModels { get; set; }
        //}

        //public IHttpActionResult Main([FromBody] Questionnaire questionnaire)
        //{
        //    if (!HasCondition(questionnaire.Id))
        //    {
        //        return Json(new { result = 0, message = "شما صلاحیت پاسخگویی به پرسشنامه را ندارید." });
        //    }

        //    //todo evaluation
        //    var query = _context.Questions
        //        .Where(q =>
        //            q.Questionnaire_Id == questionnaire.Id &&
        //            q.QuestionType != (int)Enums.QuestionType.BenchMarking)
        //        .Select(x => new
        //        {
        //            x.Questionnaire_Id,
        //            x.Id,
        //            x.Text,
        //            x.QuestionType,
        //            MultipleChoiceAnswers = x.Answers.Select(m => new
        //            {
        //                m.Id,
        //                m.AnswerText
        //            }),
        //            VBAnswers = x.Answers.Select(v => new
        //            {
        //                v.Id,
        //                v.AnswerText,
        //            }),
        //            Medias = x.Medias.Select(m => new
        //            {
        //                m.Url
        //            }),
        //            x.MediaType
        //        });

        //    return Json(new { questions = query });
        //}

        //[Serializable]
        //public class MainQuestionPackViewModel
        //{
        //    public string QuestionnaireId { get; set; }
        //    public string QuestionnaireName { get; set; }
        //    //public List<ResponderQuestionPackDto> ResponderQuestionPackDtos { get; set; }
        //    public List<ResponderQuestionPackViewModel> ResponderQuestionPackViewModels { get; set; }
        //}

        //public IHttpActionResult MainEvaluation([FromBody]MainQuestionPackViewModel model)
        //{
        //    if (!HasCondition(model.QuestionnaireId))
        //    {
        //        return Json(new { result = 0, message = "شما صلاحیت پاسخگویی به پرسشنامه را ندارید." });
        //    }
        //    var userId = User.Identity.GetUserId();
        //    var serverVm = _response.FromServer(model.QuestionnaireId).Where(q => q.QuestionType != Enums.QuestionType.BenchMarking).ToList();
        //    var userVm = model.ResponderQuestionPackViewModels;
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
        //            _response.SetValidationScore(score, model.QuestionnaireId, userId);
        //            _response.MrCreator(userId, validatorModel);
        //            _response.Finished(userId, model.QuestionnaireId);
        //            _context.SaveChanges();
        //            return Json(new { result = true, message = "پرسشنامه با موفقیت ذخیره شد." });



        //        }

        //    }

        //    return Json(new { result = false, message = "خطایی رخ داده است." });
        //}


        //[System.Web.Http.HttpPost]
        //public IHttpActionResult Rate([FromBody]Questionnaire questionnaire)
        //{
        //    try
        //    {
        //        //todo response and rating finalize and decision to pay money
        //        var userId = User.Identity.GetUserId();
        //        _response.SetRate(userId, questionnaire.Id, questionnaire.Rate);
        //        _context.SaveChanges();
        //        return Json(new { result = true, message = "با موفقیت ذخیره شد." });
        //    }
        //    catch (Exception e)
        //    {
        //        var userId = User.Identity.GetUserId();

        //        return Json(new { result = false, message = "خطایی رخ داده است. بیش از یک داده موجود است." });
        //    }

        //}



        [System.Web.Http.HttpPost]
        public IHttpActionResult Payments()
        {
            var userId = User.Identity.GetUserId();
            var query = _context.Responses
                .Where(r => /*r.User_Id == userId &&*/
                                                      r.DatePaid != null &&
                                                      r.IsBanned != true)
                .Select(r => new
                {
                    QuestionnaireName = r.Questionnaire.Name,
                    r.DatePaid,
                    QuestionnairePrice = r.Questionnaire.EachPersonMoneyPrice,
                    r.DateFinished

                });
            return Json(new { Payments = query });
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult MoneyBalance()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var query = _context.Responses
                    .Where(r => r.User_Id == userId &&
                        r.DatePaid == null &&
                        r.IsBanned != true)
                    .Select(r => r.Questionnaire.EachPersonMoneyPrice).Sum();
                return Json(new { result=true, MoneyBalance = query ?? 0 });
            }
            catch (Exception e)
            {
                return Json(new { result = false, message = "خطایی رخ داده است" });

            }

        }

        [Serializable]
        public class Questionnaire
        {
            public string Id { get; set; }
            public int? Rate { get; set; }
            public string CardNumber { get; set; }

        }


        [System.Web.Http.HttpPost]
        public IHttpActionResult SendPaymentRequest(Questionnaire questionnaire)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var responder = _context.AspNetUsers.Single(r => r.Id == userId);
                responder.HasPaymentRequest = true;
                responder.BankId = questionnaire.CardNumber;
                _context.SaveChanges();
                return Json(new { result = true, message = "درخواست با موفقیت ثبت گردید." });
            }
            catch (Exception e)
            {
                var userId = User.Identity.GetUserId();
                return Json(new { result = false, message = "چنین کاربری به عنوان پاسخ دهنده ثبت نشده است." });

            }

        }


        [System.Web.Http.HttpPost]
        public IHttpActionResult Responses()
        {
            var userId = User.Identity.GetUserId();
            var query = _context.Responses
                .Where(r => r.User_Id == userId &&
                            r.DateFinished != null &&
                            r.IsBanned != true)
                .Select(r => new
                {
                    QuestionnaireName = r.Questionnaire.Name,
                    QuestionnairePrice = r.Questionnaire.EachPersonMoneyPrice,
                    r.DateFinished

                });
            return Json(new {responses=query});
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult Notifications()
        {
            var userId = User.Identity.GetUserId();
            var query = _context.Notifications
                .Where(n => n.ToUser_Id == userId)
                .Select(n => new
                {
                    //n.FromUser_Id,
                    //n.ToUser_Id,  
                    n.DateCreated,
                    n.Title,
                    n.Text,
                    n.Type,
                    n.IsSeen

                });
            return Json(new { Notifications = query });
        }

    }
}
