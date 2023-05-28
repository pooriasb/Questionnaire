using Microsoft.AspNet.Identity;
using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Editor.Controllers
{
    [Authorize(Roles = "Editor")]
    public partial class RequestForEditorController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly NotificationRepository _notfication;
        private readonly PaymentRepository _payment;

        public RequestForEditorController()
        {
            _context = new QuestionnaireEntities();
            _notfication = new NotificationRepository(_context);
            _payment = new PaymentRepository(_context);
        }

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// this action result shows the list of requested payment from users.
        /// </summary>
        /// <returns></returns>
        public ActionResult RequestFromResponder()
        {
            var model = new List<RequsetForPaymentDto>();
            var hasRequest = _context.AspNetUsers.Where(r => r.HasPaymentRequest == true).Select(r => new
            {
                r.FirstName,
                r.LastName,
                r.UserName,
                r.BankId,
                r.Id,
                MoneyToPay = r.MoneyBalance

            });

            var responses = _context.Responses;

            foreach (var item in hasRequest)
            {
                var request = new RequsetForPaymentDto()
                {
                    BankId = item.BankId,
                    // MoneyToPay = responses.Where(r => r.User_Id == item.Id && r.DatePaid == null && r.IsBanned == false).Select(r => r.Questionnaire.EachPersonMoneyPrice).Sum(),
                    MoneyToPay = item.MoneyToPay,
                    UserName = item.UserName,
                    UserNameAndLastName = item.FirstName + " " + item.LastName

                };
                model.Add(request);
            }
            return View(model);
        }


        /// <summary>
        /// detailed report of payment to a user.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ActionResult RequestFromResponderReport(string username)
        {
            try
            {
                var userId = _context.AspNetUsers.Single(u => u.UserName == username).Id;

                var hasRequest = _context.AspNetUsers.Single(r => r.Id == userId).HasPaymentRequest;
                if (hasRequest == true)
                {
                    var responder = _context.AspNetUsers.Where(r => r.Id == userId).Select(r => new
                    {
                        r.FirstName,
                        r.LastName,
                        r.UserName,
                        r.BankId,
                        r.Id,
                        r.MoneyBalance,
                        MoneyToPayForAnswer = r.Responses.Where(resp => resp.User_Id == r.Id && resp.IsBanned == false),
                        MoneyToPayForReferer = r.MoneyToReferers.Where(m=>m.DatePaid == null && m.ToUser_Id == r.Id)

                    }).Single();

                    var responses = _context.Responses.Where(r => r.User_Id == userId && r.IsBanned == false);

                    var model = new RequestFromResponderReportModel()
                    {                                               
                        UserName = responder.UserName,
                        UserNameAndLastName = responder.FirstName + " " + responder.LastName,
                        MoneyToPay = responses.Where(r => r.DatePaid == null)
                            .Select(r => r.Questionnaire.EachPersonMoneyPrice).Sum(),
                        BankId = responder.BankId,
                        ReportDtos = new List<ReportDto>(),
                    };

                    foreach (var response in responses)
                    {
                        var report = new ReportDto()
                        {
                            QuestionnaireName = response.Questionnaire.Name,
                            QuestionnairePrice = response.Questionnaire.EachPersonMoneyPrice,
                            AnsweredDate = response.DateFinished,
                            PaidDate = response.DatePaid
                        };
                        model.ReportDtos.Add(report);
                    }
                    return View(model);

                }

                return RedirectToAction("RequestFromResponder");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public ActionResult ConfirmPaymentToResponder(string username)
        {
            try
            {
                var userId = _context.AspNetUsers.Single(u => u.UserName == username).Id;
                var responder = _context.AspNetUsers.Single(r => r.HasPaymentRequest == true && r.Id == userId);
                responder.HasPaymentRequest = false;
                var responses = _context.Responses.Where(r => r.User_Id == userId && r.DatePaid == null && r.IsBanned == false);
                // var moneyToPay = responses.Where(r=>r.DatePaid == null && r.IsBanned == false).Select(r => r.Questionnaire.EachPersonMoneyPrice).Sum();
                //foreach (var response in responses)
                //{
                //    response.DatePaid = DateTime.Now;
                //}

                var user = _context.AspNetUsers.Find(userId);
                if (user!=null)
                {
                    decimal moneyToPay = user.MoneyBalance;
                    var editorUserId = User.Identity.GetUserId();
                    _payment.MakePayment((int)Enums.PaymentType.AdminToResponder, editorUserId, userId, DateTime.Now, (decimal)moneyToPay, "پرداخت بابت پاسخگویی کاربر به پرسشنامه");
                    _notfication.Add(new Notification()
                    {
                        DateCreated = DateTime.Now,
                        FromUser_Id = editorUserId,
                        ToUser_Id = userId,
                        Title = "پرداخت",
                        Text = $"مبلغ {moneyToPay} تومان به حساب بانکی شما پرداخت گردید.",
                        Type = (int)Enums.NotificationType.Success
                    });

                    user.MoneyBalance = 0;

                    _context.SaveChanges();
                }
                

                
                return RedirectToAction("RequestFromResponder");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        //public ActionResult ReceivedRequests()
        //{
        //    //todo complete this to show the requested questionnaires
        //    return View();
        //}

        
        //public ActionResult SendRejectToQuestioner()
        //{
        //    return View();
        //}

        //public ActionResult SendAcceptToQuestioner()
        //{
        //    return View();
        //}

    }

    
}