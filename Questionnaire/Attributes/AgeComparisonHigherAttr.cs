using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using QuestionnaireProject.Models;

namespace QuestionnaireProject.Attributes
{
    public class AgeComparisonHigherAttr : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var minAge = validationContext.ObjectType.GetProperty("MinAge");
            if (minAge != null)
            {
                var minAgeValue = minAge.GetValue(validationContext.ObjectInstance, null) as int?;



                var maxAgeValue = value as int?;
                if (minAgeValue > maxAgeValue || minAgeValue == null || maxAgeValue==null)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                }


            }
            return null;
        }
    }
}