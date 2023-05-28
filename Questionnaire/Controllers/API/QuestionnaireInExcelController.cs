using Microsoft.AspNet.Identity;
using QuestionnaireProject.Models;
using QuestionnaireProject.Services;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace QuestionnaireProject.Controllers.API
{
    public class QuestionnaireInExcelController : ApiController
    {
        private readonly QuestionnaireEntities _context;
        private ServerResponseModel _serverResponse;

        public QuestionnaireInExcelController()
        {
            _context = new QuestionnaireEntities();
            _serverResponse = new ServerResponseModel();
        }

        [Serializable]
        public class ExcelInput
        {
            public string QuestionnaireId { get; set; }
            public HttpPostedFile File { get; set; }
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult Create()
        {
            var questionnaireId = HttpContext.Current.Request["QuestionnaireIdForExcel"];
            var file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
            int fileSize = file.ContentLength;
            string fileName = file.FileName;
            string mimeType = file.ContentType;
            System.IO.Stream fileContent = file.InputStream;
           
                //todo: check this
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower() == ".xlsx" && fileSize>0)
                {
                    string a = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/QuestionnaireFiles");
                    string newFileName = Guid.NewGuid().ToString().Replace("-", "") + extension;
                    string path =
                        Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/QuestionnaireFiles"),
                            newFileName);
                    file.SaveAs(path);
                    int questionnaierCount = _context.Questions.Count(q => q.Questionnaire_Id == questionnaireId);
                    if (questionnaierCount != 0)
                    {
                        return Json(new ServerResponseModel()
                        {
                            Response = false,
                            Message =
                                "در صورت تمایل به ایجاد پرسشنامه در اکسل بایستی قبلا سوالی در پرسشنامه ثبت نشده باشد."
                        });

                    }

                    var userId = User.Identity.GetUserId();
                    ExcelToDbService excelToDb = new ExcelToDbService();
                    var result = excelToDb.SaveExcelToDb(userId, 0, questionnaireId, newFileName);
                    return Json(result);

                }

                return Json(new ServerResponseModel()
                {
                    Message = "دقت نمایید که فایل انتخابی شما بایستی حتما دارای پسوند xlsx باشد.",
                    Response = true
                });



           
            




        }

    }
}
