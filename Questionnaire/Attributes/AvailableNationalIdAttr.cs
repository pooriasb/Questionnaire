using QuestionnaireProject.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuestionnaireProject.Attributes
{
    public class AvailableNationalIdAttr : ValidationAttribute
    {

        public AvailableNationalIdAttr()
        {
            _context = new QuestionnaireEntities();

        }

        private readonly QuestionnaireEntities _context;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string nationalId = value as string;
            bool isRepeated = _context.AspNetUsers.Any(u => u.PhoneNumber == nationalId);
            if (isRepeated)
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }
           
            return null;
        }
    }
}