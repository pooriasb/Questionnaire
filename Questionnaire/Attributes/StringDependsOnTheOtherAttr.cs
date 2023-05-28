using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuestionnaireProject.Attributes
{
    public class StringDependsOnTheOtherAttr : ValidationAttribute
    {
        public StringDependsOnTheOtherAttr(string otherVariable)
        {
            //this.List = list;
            this.OtherVariable = otherVariable;

        }

        public string OtherVariable { get; private set; }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var secondString = validationContext.ObjectType.GetProperty(OtherVariable);
            if (secondString != null)
            {
                var secondStringValue = secondString.GetValue(validationContext.ObjectInstance, null);

                if (secondStringValue == null)
                {
                    if (value as string == null)
                    {
                        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));

                    }
                }



            }

            
            return null;
        }
    }
}
