using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuestionnaireProject.Attributes
{
    public class PictureMaxLengthAttr : ValidationAttribute
    {
        public PictureMaxLengthAttr(int pictureMaxLength)
        {
            this.PictureMaxLength = pictureMaxLength;

        }

        public int PictureMaxLength { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var picture = value as HttpPostedFileBase;
            if (picture != null)
            {
                var pictureMaxLength = picture.ContentLength;
                if (pictureMaxLength > PictureMaxLength)
                {
                    return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));

                }
            }
            return null;
        }
    }
}