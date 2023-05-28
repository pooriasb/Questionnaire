using System.ComponentModel.DataAnnotations;

namespace QuestionnaireProject.Areas.Responder.Models
{
    public class RequestForResponderViewModel
    {
        public decimal? MoneyForAnswers { get; set; }
        public decimal? MoneyForReferer { get; set; }
        public decimal? MoneyInWallet { get; set; }
        public decimal ResponderRequestLeastValue { get; set; }

        [MinLength(16, ErrorMessage = "حداقل بایستی 16 رقم وارد نمایید.")]
        public string CardId { get; set; }
        public bool? HasPaymentRequest { get; set; }

    }
}