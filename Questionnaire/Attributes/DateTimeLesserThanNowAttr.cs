using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuestionnaireProject.Attributes
{
    public class DateTimeLesserThanNowAttr : ValidationAttribute
    {
        


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime d = Convert.ToDateTime(value);
            if (d > DateTime.Now)
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            return null;
        }        
    }
}