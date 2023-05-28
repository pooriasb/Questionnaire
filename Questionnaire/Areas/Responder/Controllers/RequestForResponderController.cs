using Microsoft.AspNet.Identity;
using QuestionnaireProject.Areas.Responder.Models;
using QuestionnaireProject.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Responder.Controllers
{
    [Authorize(Roles = "Responder")]

    public class RequestForResponderController : Controller
    {
        private readonly QuestionnaireEntities _context;

        public RequestForResponderController()
        {
            _context = new QuestionnaireEntities();
        }

        public ActionResult Index()
        {

            var userId = User.Identity.GetUserId();

            var answeredQuestionnaires = _context.Responses.Where(r => r.User_Id == userId && r.DateFinished != null && r.DatePaid == null && r.IsBanned == false)
                .Select(r => new
                {
                    EachPersonPrice = r.Questionnaire.EachPersonMoneyPrice.Value
                });

            var refererMoney = _context.MoneyToReferers.Where(m => m.ToUser_Id == userId && m.DatePaid == null).Select(m => new
            {
                m.MoneyToUser.Value
            });
            var model = new RequestForResponderViewModel();


            //if (answeredQuestionnaires.Count() != 0)
            //{
            model = new RequestForResponderViewModel()
            {
                CardId = _context.AspNetUsers.FirstOrDefault(r => r.Id == userId).BankId,
                MoneyForAnswers = (decimal?)answeredQuestionnaires.Sum(x => (decimal?)x.EachPersonPrice) ?? 0,
                MoneyForReferer = (decimal?)refererMoney.Sum(x => (decimal?)x.Value) ?? 0,
                MoneyInWallet = _context.AspNetUsers.FirstOrDefault(a => a.Id == userId)?.MoneyBalance,
                ResponderRequestLeastValue =
                    decimal.Parse(_context.Options.FirstOrDefault(o => o.Name == "ResponderRequestLeastValue").Value),
                HasPaymentRequest = _context.AspNetUsers.FirstOrDefault(r => r.Id == userId).HasPaymentRequest
            };

            //}



            return View(model);
        }

        public ActionResult PaymentRequest(RequestForResponderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            var userId = User.Identity.GetUserId();
            var user = _context.AspNetUsers.Find(userId);
            if (user!=null && user.HasPaymentRequest != true)
            {
                var money = _context.Responses.Where(r => r.User_Id == userId && r.DatePaid == null && r.IsBanned == false)
                                .Sum(x => (decimal?)x.Questionnaire.EachPersonMoneyPrice) ?? 0;
                var minMoney = decimal.Parse(_context.Options.FirstOrDefault(o => o.Name == "ResponderRequestLeastValue").Value);
                if (money >= minMoney)
                {
                    var responder = _context.AspNetUsers.FirstOrDefault(r => r.Id == userId);
                    AccumulateAllMoney();
                    if (responder.HasPaymentRequest != null)
                    {
                        responder.BankId = model.CardId;
                        responder.HasPaymentRequest = true;
                    }
                    _context.SaveChanges();
                }
            }
            

            return RedirectToAction("Index");

        }

        private void AccumulateAllMoney()
        {
            var userId = User.Identity.GetUserId();
            var user = _context.AspNetUsers.Find(userId);
            if (user != null)
            {
                var responses =
                    _context.Responses.Where(r => r.User_Id == userId && r.DateFinished != null && r.IsBanned == false && r.DatePaid == null);
                var refererMoney = _context.MoneyToReferers.Where(r => r.ToUser_Id == userId && r.DatePaid == null);

                decimal responseMoneySum = 0;
                foreach (var response in responses)
                {
                    responseMoneySum += response.Questionnaire.EachPersonMoneyPrice ?? 0;
                    response.DatePaid = DateTime.Now;
                }

                decimal refererMoneySum = 0;
                foreach (var rm in refererMoney)
                {
                    refererMoneySum += rm.MoneyToUser ?? 0;
                    rm.DatePaid = DateTime.Now;
                }

                user.MoneyBalance += (responseMoneySum + refererMoneySum);

                _context.SaveChanges();
            }

        }
    }
}