using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace QuestionnaireProject.Attributes
{
    public class ListMaxLengthAttr: ValidationAttribute
    {
        public ListMaxLengthAttr(int maxLength)
        {
            //this.List = list;
            this.MaxLength = maxLength;
        }

        //public List<string> List { get; private set; }
        public int MaxLength { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var values = value as List<string>;
            if (values?.Count(a => a != "")>MaxLength)
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