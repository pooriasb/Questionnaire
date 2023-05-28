﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuestionnaireProject.Attributes
{
    public class ListMinLengthAttr: ValidationAttribute
    {
        public ListMinLengthAttr(int minLength)
        {
            //this.List = list;
            this.MinLength = minLength;
        }

        //public List<string> List { get; private set; }
        public int MinLength { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var values = value as List<string>;
            if (values?.Count(a => a != "") < MinLength)
            {
                return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            }

            //var properties = this.PropertyNames.Select(validationContext.ObjectType.GetProperty);
            //var values = properties.Select(p => p.GetValue(validationContext.ObjectInstance, null)).OfType<string>();
            //var totalLength = values.Sum(x => x.Length) + Convert.ToString(value).Length;
            //if (totalLength < this.MinLength)
            //{
            //    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
            //}
            return null;
        }
    }
}