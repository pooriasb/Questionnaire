using QuestionnaireProject.Areas.Questioner.Core.IRepositories;
using QuestionnaireProject.Models;
using System.Web;

namespace QuestionnaireProject.Areas.Questioner.Persistence.Repositories
{

    public class RegisterRepository : IRegisterRepository
    {
        private readonly QuestionnaireEntities _context;

        public RegisterRepository(QuestionnaireEntities context)
        {
            _context = context;
        }
        public void InsertQuestionerProfile(string nationalId, HttpPostedFileBase nationalCardImage, string userId)
        {
            //var quetionerProfile = new QuestionerProfile();
            //quetionerProfile.User_Id = userId;
            //if (nationalCardImage != null)
            //{
            //    if (System.IO.Path.GetExtension(nationalCardImage.FileName)?.ToLower() == ".jpg")
            //    {
            //        if (nationalCardImage.ContentLength <= 2 * Math.Pow(1024, 2))
            //        {
            //            byte[] b = new byte[nationalCardImage.ContentLength];
            //            nationalCardImage.InputStream.Read(b, 0, nationalCardImage.ContentLength);
            //            quetionerProfile.NationalCardImage = b;
            //        }
            //    }
            //}
        }

        
    }
}