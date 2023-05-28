using QuestionnaireProject.Models;
using System;
using System.Linq;

namespace QuestionnaireProject.Persistence.Repositories
{
    interface IQuestionnaireRepository
    {
        Questionnaire GetSingle(string questionnaireId);
        IQueryable<Questionnaire> GetAll();
        void Create(Questionnaire questionnaire);
        void Delete(string questionnaireId);
        void Edit(string questionnaireId, Questionnaire questionnaire);


        //Status Change
        void ChangeStatus(string questionnaireId, Enums.RequestStatus status);
        void ChangeStatusDueToEdit(string questionnaireId);

        //Confirm Questionnaire
        void DateConfirmed(string questionnaireId, DateTime dateTimeNow); //adds confirmdate
        void CancelConfirm(string questionnaireId); //Makes date null
        bool IsConfirmed(string questionnaireId);

        //Payment Making
        void DatePaied(string questionnaireId, DateTime dateTimeNow);
        void CancelPaid(string questionnaireId); //make payment day null
        bool IsPaid(string questionnaireId);

        //Expire options
        void DateExprire(string questionnaireId, DateTime dateToExpireQuestionnaire);
        void IsExpire(string questionnaireId, bool isExpire);

        //Special
        void DateSpecial(string questionnaireId, DateTime dateSpecialExpire);
        void SpecialId(string questionnaireId, int specialId);
        void RemoveSpecialId(string questionnaireId); // make special Id null


        //Rating
        void Rate(string questionnaireId);  //calculate rate


        //Payment Details
        void AddPaymentDetail(string questionnaireId, decimal eachPerson, int sitePercent, decimal sitePrice, decimal totalPrice);


    }
}
