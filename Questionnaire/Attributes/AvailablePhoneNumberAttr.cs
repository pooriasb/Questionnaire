using QuestionnaireProject.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuestionnaireProject.Attributes
{
    public class AvailablePhoneNumberAttr : ValidationAttribute
    {

        public AvailablePhoneNumberAttr()
        {
            _context = new QuestionnaireEntities();

        }

        private readonly QuestionnaireEntities _context;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string phoneNumber = value as string;
            bool isRepeated = _context.AspNetUsers.Any(u => u.PhoneNumber == phoneNumber);
            if (isRepeated)
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
           
            return null;
        }
    }
}