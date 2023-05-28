using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionnaireProject.Persistence
{
    public class QuestionnaireRepository : IQuestionnaireRepository
    {
        public QuestionnaireEntities _context { get; set; }

        public QuestionnaireRepository(QuestionnaireEntities context)
        {
            _context = context;
        }
        public Questionnaire GetSingle(string questionnaireId)
        {
            var query = _context.Questionnaires.Find(questionnaireId);
            return query;
        }

        public async Task<Questionnaire> GetSingleAsync(string questionnaireId)
        {
            var query = await _context.Questionnaires.FindAsync(questionnaireId);
            return query;
        }

        public IQueryable<Questionnaire> GetAll()
        {
            return _context.Questionnaires;
        }

        public void Create(Questionnaire questionnaire)
        {
            throw new NotImplementedException();
        }

        public void Delete(string questionnaireId)
        {
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            if (questionnaire!=null)
            {
                _context.Questionnaires.Remove(questionnaire);
            }
        }

        public void Edit(string questionnaireId, Questionnaire questionnaire)
        {
            throw new NotImplementedException();
        }

        public void ChangeStatus(string questionnaireId, Enums.RequestStatus status)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.Status = (int) status;
        }

        public void ChangeStatusDueToEdit(string questionnaireId)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.DateConfirmed = null;
            questionnaire.Status = (int)Enums.RequestStatus.Created;
        }

        public void DateConfirmed(string questionnaireId, DateTime dateTimeNow)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.DateConfirmed = dateTimeNow;
        }

        public void CancelConfirm(string questionnaireId)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.DateConfirmed = null;
        }

        public bool IsConfirmed(string questionnaireId)
        {
            try
            {
                var confirmationResult = _context.Questionnaires.Single(q => q.Id == questionnaireId).DateConfirmed;
                if (confirmationResult != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        /// <summary>
        /// This method sets the date of the payment for the questionnaire and would set the status to paid.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="dateTimeNow"></param>
        public void DatePaied(string questionnaireId, DateTime dateTimeNow)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.DatePaid = dateTimeNow;
            questionnaire.Status = (int) Enums.RequestStatus.Paid;
        }

        public void CancelPaid(string questionnaireId)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.DateConfirmed = null;
        }

        public bool IsPaid(string questionnaireId)
        {
            try
            {
                var paymentResult = _context.Questionnaires.Single(q => q.Id == questionnaireId).DatePaid;
                if (paymentResult != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void DateExprire(string questionnaireId, DateTime dateToExpireQuestionnaire)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.DateExpire = dateToExpireQuestionnaire;
        }

        public void IsExpire(string questionnaireId, bool isExpire)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.IsExpired = isExpire;
        }

        public void DateSpecial(string questionnaireId, DateTime dateSpecialExpire)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.DateSpecialExpire = dateSpecialExpire;
        }

        public void SpecialId(string questionnaireId, int specialId)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.Special_Id = specialId;
        }

        public void RemoveSpecialId(string questionnaireId)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.Special_Id = null;
        }

        public void Rate(string questionnaireId)
        {
            var rates = _context.Responses.Where(r => r.Questionnaire_Id == questionnaireId).Select(r => r.Rate);
            if (!rates.Any())
            {
                int sum = (int) rates.Sum();
                var count = rates.Count();
                int average = (int) sum/count;
                var questionnaire = GetSingle(questionnaireId);
                questionnaire.Rate = average;
            }
            else
            {
                var questionnaire = GetSingle(questionnaireId);
                questionnaire.Special_Id = 0;
            }
          
        }

        public void AddPaymentDetail(string questionnaireId, decimal eachPerson, int sitePercent, decimal sitePrice, decimal totalPrice)
        {
            var questionnaire = GetSingle(questionnaireId);
            questionnaire.EachPersonMoneyPrice = eachPerson;
            questionnaire.SitePercentValue = sitePercent;
            questionnaire.SitePercentPrice = sitePrice;
            questionnaire.TotalPayment = totalPrice;

        }

        
    }
}