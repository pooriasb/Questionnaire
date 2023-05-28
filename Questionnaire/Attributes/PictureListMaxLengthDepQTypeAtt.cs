﻿using QuestionnaireProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuestionnaireProject.Attributes
{
    public class PictureListMaxLengthDepQTypeAtt : ValidationAttribute
    {
        public PictureListMaxLengthDepQTypeAtt(int maxLength, Enums.QuestionType questionType)
        {
            //this.List = list;
            this.MaxLength = maxLength;
            this.QuestionType = questionType;

        }

        public int MaxLength { get; private set; }
        public Enums.QuestionType QuestionType { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var questionType = validationContext.ObjectType.GetProperty("QuestionType");
            if (questionType != null)
            {
                var questionTypeValues = questionType.GetValue(validationContext.ObjectInstance, null);


                if ((Enums.QuestionType) questionTypeValues == QuestionType)
                {
                    var values = value as List<HttpPostedFileBase>;
                    if (values?.Count(a => a != null) > MaxLength)
                    {
                        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                    }
                }

            }
            return null;
        }
    }
}


