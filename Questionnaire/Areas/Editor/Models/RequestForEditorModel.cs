using System;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Editor.Models
{
    public class RequsetForPaymentDto
    {
        public string UserNameAndLastName { get; set; }
        public string UserName { get; set; }
        public string BankId { get; set; }
        public decimal? MoneyToPay { get; set; }
    }

    public class RequestFromResponderReportModel
    {
        public string UserNameAndLastName { get; set; }
        public string UserName { get; set; }
        public string BankId { get; set; }
        public decimal? MoneyToPay { get; set; }
        public List<ReportDto> ReportDtos { get; set; }
    }

    public class ReportDto
    {
        public string QuestionnaireName { get; set; }
        public decimal? QuestionnairePrice { get; set; }
        public DateTime? AnsweredDate { get; set; }
        public DateTime? PaidDate { get; set; }
    }


}