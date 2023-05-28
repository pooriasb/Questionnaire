using Microsoft.AspNet.Identity;
using PagedList;
using QuestionnaireProject.Areas.Responder.Models;
using QuestionnaireProject.Extensions;
using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;

namespace QuestionnaireProject.Areas.Responder.Controllers
{
    [Authorize(Roles = "Responder")]
    public class ResponderController : Controller
    {
        public readonly QuestionnaireEntities _context;
        public ResponderController()
        {
            _context = new QuestionnaireEntities();
        }

        
        public ActionResult Index(int? page)
        {
            var userId = User.Identity.GetUserId();
            var answeredQuestionnaires =  _context.Responses.Where(r => r.User_Id == userId && r.DatePaid == null && r.IsBanned==false)
                .Select(r => new
                {
                    EachPersonPrice = r.Questionnaire.EachPersonMoneyPrice.Value
                }).ToList();

            //todo add pagination. 

            var cityId = User.Identity.GetCityId().AsInt();
            int? provinceId = _context.Cities.FirstOrDefault(c => c.Id == cityId)?.Province_Id;
            var sex = User.Identity.GetSexId().AsInt();
            var age = DateTime.Now.Year - User.Identity.GetBirthDate().AsDateTime().Year;
            var field = User.Identity.GetStudyFieldId().AsInt();

            //todo add the date constraint

            //این پارامتر بر اساس صلاحیت پاسخ دهنده مقادیر پرسشنامه را نمایش میدهد.
            //var questionnaires = _context.Conditions.Where(c => c.MaxAge > age &&
            //                                                    c.MinAge < age &&
            //                                                    (c.Sex == sex || c.Sex == 2) &&
            //                                                    (c.Field_Id == field || c.Field_Id == null) &&
            //                                                    (c.Provience_Id == provinceId || c.Provience_Id == null) &&
            //                                                    (c.City_Id == cityId || c.City_Id == null)).Select(q => new
            //                                                    {
            //                                                        q.Questionnaire.Id,
            //                                                        q.Questionnaire.Name,
            //                                                        q.Questionnaire.Rate,
            //                                                        QuestionsNumber = q.Questionnaire.Questions.Count,
            //                                                        EachPersonMoney = q.Questionnaire.EachPersonMoneyPrice,
            //                                                        q.Questionnaire.Special_Id,
            //                                                        ExpiryDate = q.Questionnaire.DateExpire
            //                                                    });

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
            var model = new ResponderIndexDto()
            {
                AvailableQuestionnaireDtos = availableQuestionnaireDtos.OrderByDescending(a => a.Special == 0||a.Special==1).ToPagedList(pageNumber, pageSize),
            AvailableQuestionnaires =
                     _context.Responses.Where(x => x.DateFinished == null && x.User_Id == userId).Count(), // taghir karde be na tamam ha
               // AnsweredQuestionnaires = _context.Responses.Count(r => r.User_Id == userId),
                AnsweredQuestionnaires = availableQuestionnaireDtos.Count(),// taghir karde be porsesh name haye vajede sharayet
                MoneyToReceive = _context.AspNetUsers.Single(q => q.Id == userId).MoneyBalance//answeredQuestionnaires.Select(x=>x.EachPersonPrice).Sum()
            };
            Session["AvailableCount"] = model.AnsweredQuestionnaires;
            Session["UnFinished"] = model.AvailableQuestionnaires;



            return View(model);
        }


    }
}