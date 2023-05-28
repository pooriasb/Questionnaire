using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace QuestionnaireProject.Areas.Questioner.Controllers
{
    //Currently this controller is unuseful but it would become uesful in near future.
    public class UploadFileForQuestionerController : Controller
    {
        public class FileUploadModel
        {
            [DataType(DataType.Upload)]
            [Display(Name = "فایل خود را آپلود نمایید")]
            [Required(ErrorMessage = "لطفا فایل اکسلی را جهت آپلود انتخاب نمایید.")]
            public string File { get; set; }

        }

        // GET: UploadFileForQuestioner
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult UploadToServer(HttpPostedFileBase file)
        //{
            
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {

        //            if (file != null)
        //            {
        //                //todo: check this
        //                string extension = Path.GetExtension(file.FileName);
        //                if (extension != null && extension.ToLower()==".xlsx")
        //                {
        //                    string path = Path.Combine(Server.MapPath("~/Uploads/QuestionnaireFiles"), Guid.NewGuid().ToString().Replace("-","") + extension);
        //                    file.SaveAs(path);
        //                }
        //                else
        //                {
        //                    ViewBag.FileError = "لطفا فایل اکسل را برای آپلود انتخاب کنید";
        //                    return View("Index");
        //                }
                       

        //            }
        //            ViewBag.FileStatus = "فایل شما آپلود گردید.";
        //        }
        //        catch (Exception)
        //        {

        //            ViewBag.FileError = "در آپلود فایل شما خطایی رخ داده است";
        //        }

        //    }
        //    return View("Index");

        //}

        
    }
}