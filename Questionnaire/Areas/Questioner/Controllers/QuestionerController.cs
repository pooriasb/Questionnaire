using Microsoft.AspNet.Identity;
using QuestionnaireProject.Areas.Questioner.Models;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

// This controller provides all the needed codes and functions which should be used by the EDITOR

namespace QuestionnaireProject.Areas.Questioner.Controllers
{
    [Authorize(Roles = "Questioner, Editor")]
    public class QuestionerController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly QuestionnaireRepository _questionnaire;

        public QuestionerController()
        {
            _context = new QuestionnaireEntities();
            _questionnaire = new QuestionnaireRepository(_context);
        }
       
        // GET: Questioner
        public ActionResult Index()
        {
            try
            {
                Task.Run(()=>WalletUpdater());

                var userId = User.Identity.GetUserId();
                QuestionerLandingPageDto model = new QuestionerLandingPageDto()
                {
                    SentRequestsCount = _questionnaire.GetAll().Count(q => q.User_Id == userId &&
                                                                           q.Status == (int)Enums.RequestStatus.Pending),
                    AcceptedRequestsCount = _questionnaire.GetAll().Count(q=>q.User_Id == userId && 
                                                                             q.Status == (int) Enums.RequestStatus.Accepted),
                    NotPaidQuestionnairesCount = _questionnaire.GetAll().Count(q => q.User_Id == userId &&
                                                                                       q.Status == (int)Enums.RequestStatus.Accepted),
                    FinishedQuestionnairesCount = _questionnaire.GetAll().Count(q => q.User_Id == userId &&
                                                                                     q.Status == (int)Enums.RequestStatus.Finished),
                    InProcessQuestionnairesCount = _questionnaire.GetAll().Count(q => q.User_Id == userId &&
                                                                                      q.Status == (int)Enums.RequestStatus.Paid),
                    RejectedRequsetsCount = _questionnaire.GetAll().Count(q => q.User_Id == userId &&
                                                                               q.Status == (int)Enums.RequestStatus.Rejected),
                    UploadedFilesCount = 0,
                    Notifications = _context.Notifications.Where(n => n.ToUser_Id == userId).ToList(),
                    MoneyBalance = _context.AspNetUsers.Single(q => q.Id == userId).MoneyBalance

                };
                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpException(404, "Some description");
            }
           
        }

        public void WalletUpdater()
        {
            var context = new QuestionnaireEntities();
            var userId = User.Identity.GetUserId();
            List<Questionnaire> questionnaires = context.Questionnaires.Where(q => q.User_Id == userId &&
                                                                                          q.DateExpire < DateTime.Now).ToList();
            var questionerProfile = context.AspNetUsers.FirstOrDefault(q => q.Id == userId);
            foreach (var q in questionnaires)
            {
                q.Status = (int)Enums.RequestStatus.Finished;                
                q.IsExpired = true;
                if (q.RemainMoney != null)
                    if (questionerProfile != null)
                        questionerProfile.MoneyBalance += (decimal) q.RemainMoney;
                q.RemainMoney = 0;
                context.SaveChanges();


            }

        }
    }
}