using QuestionnaireProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuestionnaireProject.Attributes
{
    public class ListMinLengthDepQTypeAtt : ValidationAttribute
    {
        public ListMinLengthDepQTypeAtt(int minLength, Enums.QuestionType questionType)
        {
            //this.List = list;
            this.MinLength = minLength;
            this.QuestionType = questionType;

        }

        public int MinLength { get; private set; }
        public Enums.QuestionType QuestionType { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var questionType = validationContext.ObjectType.GetProperty("QuestionType");
            if (questionType != null)
            {
                var questionTypeValues = questionType.GetValue(validationContext.ObjectInstance, null);

                
                    if ((Enums.QuestionType)questionTypeValues == QuestionType)
                    {
                        var values = value as List<string>;
                        if (values?.Count(a => a != "") < MinLength)
                        {
                            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                
            }
            return null;
        }
    }
}


