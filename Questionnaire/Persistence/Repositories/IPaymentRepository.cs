using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;

namespace QuestionnaireProject.Persistence.Repositories
{
    interface IPaymentRepository
    {

        void MakePayment(Enums.PaymentType paymentType,
            string fromUserId,
            string toUserId,
            DateTime dateTime,
            decimal value,
            string comment);

        int GetSitePercent();

        int GetDiscountOccasionalValue();

        string GetDiscountOccasionalName();

        int GetDiscountReferenceValue(string code);

        List<string> GetDiscaountReferenceCode(string userId);


        void DeleteDiscountRefernceCode(string code);






    }
}
