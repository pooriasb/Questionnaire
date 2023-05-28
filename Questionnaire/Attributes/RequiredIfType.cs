using QuestionnaireProject.Models;
using System.ComponentModel.DataAnnotations;

namespace QuestionnaireProject.Attributes
{
    public class RequiredIfType : ValidationAttribute
    {

        public RequiredIfType(Enums.MediaType mediaType)
        {
            this.MediaType = mediaType;

        }

        public Enums.MediaType MediaType { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
             var mediaType = validationContext.ObjectType.GetProperty("MediaType");
            if (mediaType != null)
            {
                var mediaTypeValues = mediaType.GetValue(validationContext.ObjectInstance, null);


                if ((Enums.MediaType) mediaTypeValues == MediaType)
                {
                    if (value == null)
                    {
                        return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                    }
                }

            }
            return null;
        }
    }
}