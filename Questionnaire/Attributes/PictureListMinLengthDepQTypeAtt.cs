using QuestionnaireProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuestionnaireProject.Attributes
{
    public class PictureListMinLengthDepQTypeAtt : ValidationAttribute
    {
        public PictureListMinLengthDepQTypeAtt(int minLength, Enums.QuestionType questionType)
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
                        var values = value as List<HttpPostedFileBase>;
                        if (values?.Count(a => a != null) < MinLength)
                        {
                            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                
            }
            return null;
        }
    }
}


