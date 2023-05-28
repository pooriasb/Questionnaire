using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Web;

namespace QuestionnaireProject.Areas.Questioner.Core.Attributes
{
    public class FileExtensionAttr : ValidationAttribute
    {

        private readonly string _fileExtension;
        public FileExtensionAttr(string fileExtension)
        {
            _fileExtension = fileExtension;
        }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file == null)
                return false;

            string extension = Path.GetExtension(file.FileName)?.ToLower();

            if (extension != null && extension.Contains(_fileExtension))
            {
                return true;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return base.FormatErrorMessage(_fileExtension);
        }


    }
}