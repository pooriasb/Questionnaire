using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestionnaireProject.Persistence
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly QuestionnaireEntities _context;

        public PaymentRepository(QuestionnaireEntities context)
        {
            _context = context;
        }
        public void MakePayment(Enums.PaymentType paymentType,
            string fromUserId,
            string toUserId,
            DateTime dateTime,
            decimal value,
            string comment)
        {
            _context.Payments.Add(new Payment()
            {
                Id = Guid.NewGuid().ToString(),
                FromUser_Id = fromUserId,
                ToUser_Id = toUserId,
                Type = (int)paymentType,
                PaymentDate = DateTime.Now,
                Value = value,
                Comment = comment
            });
        }

        public int GetSitePercent()
        {
            int value = int.Parse(_context.Options.FirstOrDefault(o => o.Name == "SitePercent").Value);
            return value;
        }

        public int GetRefererPercent()
        {
            int value = int.Parse(_context.Options.FirstOrDefault(o => o.Name == "RefererPercent").Value);
            return value;
        }

        public int GetDiscountOccasionalValue()
        {

            int value = int.Parse(_context.Options.FirstOrDefault(o => o.Name == "DiscountOccasional").Value);
            return value;
        }

        public string GetDiscountOccasionalName()
        {
            string code = _context.Options.FirstOrDefault(o => o.Name == "DiscountOccasionalWord").Value;
            return code;
        }

        public int GetDiscountReferenceValue(string code)
        {
            int? value = _context.Discounts.FirstOrDefault(o => o.Id == code).SitePercent;
            if (value != null)
            {
                return (int)value;

            }
            return 10; //todo site percent to return...
        }

        public bool HasReferer(string newUserId)
        {
            var hasReferer = _context.RefererCharges.Any(r => r.NewClient_Id == newUserId && r.CountToPay > 0);
            return hasReferer;
        }

        public string GetRefererId(string newUserId)
        {
            var refererId = _context.RefererCharges.FirstOrDefault(r => r.NewClient_Id == newUserId && r.CountToPay > 0)?.Referer_Id;
            return refererId;
        }

        public MoneyToReferer PayMoneyToReferer(string questionnaireId, string refererUserId, int percentToReferer, int moneyAmountToReferer)
        {           

            var moneyToReferer = new MoneyToReferer()
            {
                Questionnaire_Id = questionnaireId,
                ToUser_Id = refererUserId,
                PercentToUser = percentToReferer,
                MoneyToUser = moneyAmountToReferer
            };
            return _context.MoneyToReferers.Add(moneyToReferer);
        }

        public bool CountDownRefererRemain(string refererId, string newUserId)
        {
            var refererChargeRowId = _context.RefererCharges.FirstOrDefault(r =>
                r.Referer_Id == refererId && r.NewClient_Id == newUserId && r.CountToPay > 0)?.Id;

            if (refererChargeRowId != null)
            {
                var charge = _context.RefererCharges.Find(refererChargeRowId);
                if (charge!=null)
                {
                    charge.CountToPay--;
                    return true;
                }
                
            }

            return false;

        }

        public List<string> GetDiscaountReferenceCode(string userId)
        {
            var listOfCodes = _context.Discounts.Where(d => d.User_Id == userId).Select(d => d.Id).ToList();
            return listOfCodes;
        }


        public void DeleteDiscountRefernceCode(string code)
        {
            var discount = _context.Discounts.Find(code);

            _context.Discounts.Remove(discount);

        }
    }
}