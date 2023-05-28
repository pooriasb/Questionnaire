using ClosedXML.Excel;
using Microsoft.AspNet.Identity;
using PagedList;
using QuestionnaireProject.Areas.Questioner.Models;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace QuestionnaireProject.Areas.Questioner.Controllers
{
    [Authorize(Roles = "Questioner")]
    public class ReportForQuestionerController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly PaymentRepository _payment;

        public ReportForQuestionerController()
        {
            _context = new QuestionnaireEntities();
            _payment = new PaymentRepository(_context);
        }

        // GET: Questioner/ReportForQuestioner
        public ActionResult Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var userId = User.Identity.GetUserId();
            var questionnaires = _context.Questionnaires.Where(q => q.User_Id == userId && q.Questions.Count >0).ToList();
            var model = questionnaires.ToPagedList(pageNumber, pageSize);
            return View(model);
        }


        public ActionResult Responders(string questionnaireId, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var userId = User.Identity.GetUserId();
            var responders = _context.Responses.Where(q => q.Questionnaire_Id == questionnaireId).ToList();
            var model = responders.ToPagedList(pageNumber, pageSize);
            return View(model);
        }


        public List<EachPersonReportModel> EachPersonReportCore(string questionnaireId, string userId)
        {
            //todo: check singleOrDefault try catch


            var questionJoinedResponses = _context.UsersResponses
                .Join(_context.Questions,
                    r => r.Question_Id,
                    q => q.Id,
                    (r, q) => new { r, q })
                .Where(u => u.r.Respons.User_Id == userId && u.r.Respons.Questionnaire_Id == questionnaireId)
                .Select(u => new
                {
                    u.q,
                    u.r
                });

            
            List<EachPersonReportModel> eachPersonReportModel = new List<EachPersonReportModel>();
            foreach (var t in questionJoinedResponses)
            {
                EachPersonReportModel dto;
                if (t.q.QuestionType == (int)Enums.QuestionType.Anatomical)
                {
                    dto = new EachPersonReportModel()
                    {
                        Question = t.q.Text,
                        Response = t.r.Response,
                        MediaType = (Enums.MediaType)t.q.MediaType,
                        QuestionType = (Enums.QuestionType)t.q.QuestionType
                    };
                }
                else
                {
                    dto = new EachPersonReportModel()
                    {
                        Question = t.q.Text,
                        Response = t.q.Answers.FirstOrDefault(a => a.Id == t.r.Response)?.AnswerText,
                        MediaType = (Enums.MediaType)t.q.MediaType,
                        QuestionType = (Enums.QuestionType)t.q.QuestionType
                    };
                }

                eachPersonReportModel.Add(dto);
            }
            

            return eachPersonReportModel;
        }

        public ActionResult EachPersonReport(int responseId, int? page)
        {
            //var model = new List<EachPersonReportModel>();
            //var response = _context.VBResponses.Select(r => new
            //{
            //  r.VBAnswer.Question.Questionnaire.Id
            //});


            //todo: try catch should be added
            var response = _context.Responses.SingleOrDefault(r => r.Id == responseId);


            if (response != null)
            {
                var userId = response.User_Id;
                var questionnaireId = response.Questionnaire_Id;
                var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                  var report =   _context.ResponderReports.Where(x => x.QuastionarID == questionnaireId && x.ResponderID == userId).FirstOrDefault();
                    if (report==null)
                    {
                        ViewBag.hasReport = "f";
                    }
                    else
                    {
                        ViewBag.hasReport = "t";

                    }


                    ViewBag.UserID = userId;
                    ViewBag.response = responseId;
                    ViewBag.PackID = questionnaireId;
                    ViewBag.UniqueCode = user.UniqueCode;
                    ViewBag.BirthDate = user.BirthDate;
                    ViewBag.UserCity = _context.Cities.FirstOrDefault(c => c.Id == user.CityId)?.Name;
                    ViewBag.UserField = _context.StudyFields.FirstOrDefault(p => p.Id == user.StudyFieldId)?.Name;
                    ViewBag.QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId)?.Name;
                }

                List<EachPersonReportModel> dtos = EachPersonReportCore(questionnaireId, userId);
                return View(dtos);
            }



            return View("Index");
        }
        [HttpGet]
        public ActionResult AddUserReport(string PackageID,string UserID, string response) {
            try
            {
                ResponderReport report = new ResponderReport() { QuastionarID = PackageID, ResponderID = UserID, ReportDate = DateTime.Now };
                _context.ResponderReports.Add(report);
                _context.SaveChanges();
                TempData["ok"] = "گزارش شما با موفقیت ثبت شد";
            }
            catch (Exception)
            {

                TempData["error"] = "در هنگام ثبت خطا مشکلی رخ داده است";

            }

            return RedirectToAction("EachPersonReport",new { responseId = response });
        }


        [HttpPost]
        public ActionResult DeletePersonReport(int responseId)
        {
            var response = _context.Responses.Find(responseId);
            var userId = User.Identity.GetUserId();
            if (response!=null)
            {
                var questionnaire = _context.Questionnaires.Find(response.Questionnaire_Id);
                if (questionnaire != null && questionnaire.User_Id == userId)
                {
                    questionnaire.AnsweredByPerson--;
                    _context.Responses.Remove(response);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("EachPersonReport", new { responseId = responseId});
        }

        //public ActionResult EachPersonReportToPrint(int responseId)
        //{
        //    var response = _context.Responses.FirstOrDefault(r => r.Id == responseId);
        //    //todo هنوز کامل نیس کاکو


        //    if (response != null)
        //    {
        //        var userId = response.User_Id;
        //        var questionnaireId = response.Questionnaire_Id;
        //        var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == userId);
        //        ViewBag.UserData = user;
        //        ViewBag.UserCity = _context.Cities.FirstOrDefault(c => c.Id == user.CityId).Name;
        //        ViewBag.UserField = _context.StudyFields.FirstOrDefault(p => p.Id == user.StudyFieldId).Name;
        //        ViewBag.QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId).Name;
        //        List<EachPersonReportModel> dtos = EachPersonReportCore(questionnaireId, userId);
        //        return View(dtos);
        //    }
        //    return View("Index");
        //}


        //todo: this action result should be changed because this should only return a file instead of returning a view
        public ActionResult EachPersonReportToExcel(int responseId)
        {

            //todo هنوز کامل نیس کاکو , مثکه نیازی هم نیس
            var response = _context.Responses.FirstOrDefault(r => r.Id == responseId);


            if (response != null)
            {
                var userId = response.User_Id;
                var questionnaireId = response.Questionnaire_Id;
                var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == userId);
                ViewBag.UserData = user;
                ViewBag.UserCity = _context.Cities.FirstOrDefault(c => c.Id == user.CityId).Name;
                ViewBag.UserField = _context.StudyFields.FirstOrDefault(p => p.Id == user.StudyFieldId).Name;
                ViewBag.QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId).Name;
                List<EachPersonReportModel> eachPersonReportModel = EachPersonReportCore(questionnaireId, userId);

            }
            return View("Index");
        }

        public List<EachQuestionnaireReportModel> EachQuestionnaireReportCore(string questionnaireId)
        {
            var responses = _context.UsersResponses.Where(r => r.Respons.Questionnaire_Id == questionnaireId).ToList();
            var questions = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId).ToList();
            var answers = _context.Answers.Where(a => a.Question.Questionnaire_Id == questionnaireId).ToList();


            List<EachQuestionnaireReportModel> modelTosend = new List<EachQuestionnaireReportModel>();

            foreach (var q in questions)
            {
                EachQuestionnaireReportModel mod = new EachQuestionnaireReportModel()
                {
                    Question = q.Text,
                    QuestionId = q.Id,
                    QuestionType = (Enums.QuestionType)q.QuestionType,
                    EachQuestionnaiireAnswerModels = new List<EachQuestionnaireAnswerModel>()
                };
                if (q.QuestionType == (int)Enums.QuestionType.Anatomical)
                {
                    foreach (var anatomicalResponse in responses.Where(r => r.Question_Id == q.Id))
                    {
                        EachQuestionnaireAnswerModel answer = new EachQuestionnaireAnswerModel()
                        {
                            Answer = anatomicalResponse.Response,
                            //Count = responses.Count(r => r.Response == a.Id)

                        };
                        mod.EachQuestionnaiireAnswerModels.Add(answer);
                    }
                }
                else
                {
                    foreach (var a in answers.Where(a => a.Question_Id == q.Id))
                    {
                        EachQuestionnaireAnswerModel answer = new EachQuestionnaireAnswerModel()
                        {
                            AnswerId = a.Id,
                            Answer = a.AnswerText,
                            Count = responses.Count(r => r.Response == a.Id)

                        };
                        mod.EachQuestionnaiireAnswerModels.Add(answer);
                    }
                }
                modelTosend.Add(mod);
            }

            return modelTosend;

        }
        public ActionResult EachQuestionnaierReport(string questionnaireId)
        {
            var respondersNumber = _context.Responses.Count(r => r.Questionnaire_Id == questionnaireId);

            ViewBag.responders = respondersNumber;
            ViewBag.QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId).Name;
            ViewBag.QuestionnaireId = questionnaireId;
            List<EachQuestionnaireReportModel> modelTosend = EachQuestionnaireReportCore(questionnaireId);

            return View(modelTosend);
        }

        public ActionResult EachQuestionnaierReportToPrint(string questionnaireId)
        {
            var respondersNumber = _context.Responses.Count(r => r.Questionnaire_Id == questionnaireId);

            ViewBag.responders = respondersNumber;
            ViewBag.QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId).Name;
            List<EachQuestionnaireReportModel> modelTosend = EachQuestionnaireReportCore(questionnaireId);


            return View(modelTosend);
        }


        public void EachQuestionnaireFullReportInExcel(string questionnaireId)
        {
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            var userId = User.Identity.GetUserId();
            if (questionnaire != null && questionnaire.User_Id == userId)
            {
                var responses = _context.Responses.Where(r => r.Questionnaire_Id == questionnaireId).ToList();
                var questions = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId && q.QuestionType != (int)Enums.QuestionType.Validation 
                && q.QuestionType != (int)Enums.QuestionType.BenchMarking
                ).OrderBy(q => q.QuestionType).ToList();
                var userResponses = _context.UsersResponses
                    .Where(u => u.Respons.Questionnaire_Id == questionnaireId)
                    .GroupJoin(_context.Answers,
                        r => r.Response,
                        a => a.Id,
                        (r, a) => new { r, a }).ToList();

                var responderDetails = _context.Responses
                    .Where(r => r.Questionnaire_Id == questionnaireId)
                    .Join(_context.AspNetUsers,
                        r => r.User_Id,
                        u => u.Id,
                        (r, u) => new { r, u }).ToList();

                var cities = _context.Cities.ToList();

                DataTable fullReport = new DataTable();
                fullReport.Columns.Add(new DataColumn("ردیف", typeof(string)));
                fullReport.Columns.Add(new DataColumn("کد شرکت کننده", typeof(string)));
                fullReport.Columns.Add(new DataColumn("تاریخ پاسخگویی", typeof(string)));
                fullReport.Columns.Add(new DataColumn("کد پاسخ دهنده", typeof(string)));
                fullReport.Columns.Add(new DataColumn("جنسیت", typeof(string)));
                fullReport.Columns.Add(new DataColumn("سن", typeof(int)));
                fullReport.Columns.Add(new DataColumn("شهر محل سکونت", typeof(string)));
                foreach (var question in questions)
                {
                    DataColumn newDataColumn = new DataColumn(question.Text, typeof(string));
                    fullReport.Columns.Add(newDataColumn);

                }

                int rowNumber = 1;

                foreach (var response in responses)
                {
                    DataRow row = fullReport.NewRow();
                    PersianCalendar pc = new PersianCalendar();
                    var responder = responderDetails.SingleOrDefault(r => r.u.Id == response.User_Id);
                    string gender;
                    if (responder?.u.Sex == 0)
                    {
                        gender = "زن";
                    }
                    else
                    {
                        gender = "مرد";
                    }

                    row[0] = rowNumber;
                    row[1] = responder?.u.UniqueCode;
                    if (response.DateFinished != null)
                    {
                        int year = pc.GetYear((DateTime)response.DateFinished);
                        int month = pc.GetMonth((DateTime)response.DateFinished);
                        int day = pc.GetDayOfMonth((DateTime)response.DateFinished);
                        int hour = pc.GetHour((DateTime)response.DateFinished);
                        int minute = pc.GetMinute((DateTime)response.DateFinished);
                        row[2] = $"{year}/{month}/{day} {hour}:{minute}";
                    }
                    else
                    {
                        row[2] = response.DateFinished;
                    }
                    row[3] = responder?.u.UniqueCode;
                    row[4] = gender;
                    row[5] = DateTime.Now.Year - responder.u.BirthDate.Year;
                    row[6] = cities.FirstOrDefault(c => c.Id == responder.u.CityId)?.Name;
                    int column = 7;
                    foreach (var question in questions)
                    {
                        if (question.QuestionType == (int)Enums.QuestionType.Anatomical)
                        {
                            row[column] = userResponses
                                .FirstOrDefault(u => u.r.Question_Id == question.Id &&
                                                     u.r.Response_Id == response.Id)
                                ?.r.Response;
                        }
                        else if (question.QuestionType == (int)Enums.QuestionType.MainWithPicture)
                        {
                            string answer = userResponses
                                .FirstOrDefault(u => u.r.Question_Id == question.Id &&
                                                     u.r.Response_Id == response.Id)
                                ?.a.FirstOrDefault()
                                ?.AnswerText;
                            if (answer != null) row[column] = $"گزینه {answer.Split('-')[0]}";
                        }
                        else
                        {
                            row[column] = userResponses
                                .FirstOrDefault(u => u.r.Question_Id == question.Id &&
                                                    u.r.Response_Id == response.Id)
                                ?.a.FirstOrDefault()
                                ?.AnswerText;
                        }

                        column++;

                    }

                    fullReport.Rows.Add(row);
                    rowNumber++;
                }

                using (XLWorkbook wb = new XLWorkbook() { RightToLeft = true, })
                {
                    wb.Worksheets.Add(fullReport, "گزارش کامل");

                    //wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
                    string myName = Server.UrlEncode("QuestionnairReport" + ".xlsx");
                    MemoryStream stream = GetStream(wb);// The method is defined below
                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition",
                        "attachment; filename=" + myName);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }

            }

        }

        public void EachQuestionnaierReportToExcel(string questionnaireId)
        {
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            var userId = User.Identity.GetUserId();
            if (questionnaire.Id!=null && questionnaire.User_Id == userId)
            {
                string QuestionnaireName = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId).Name;

                int respondersNumber = _context.Responses.Count(r => r.Questionnaire_Id == questionnaireId);

                List<EachQuestionnaireReportModel> modelTosend = EachQuestionnaireReportCore(questionnaireId);




                //DataTable validationTable = new DataTable();
                //validationTable.Columns.AddRange(new DataColumn[12]
                //{
                //new DataColumn("ردیف", typeof(int)),
                //new DataColumn("پرسش", typeof(string)),
                //new DataColumn("گزینه 1",typeof(decimal)),
                //new DataColumn("گزینه 2", typeof(decimal)),
                //new DataColumn("گزینه 3", typeof(decimal)),
                //new DataColumn("گزینه 4", typeof(decimal)),
                //new DataColumn("گزینه 5",typeof(decimal)),
                //new DataColumn("گزینه 6", typeof(decimal)),
                //new DataColumn("گزینه 7", typeof(decimal)),
                //new DataColumn("گزینه 8",typeof(decimal)),
                //new DataColumn("گزینه 9", typeof(decimal)),
                //new DataColumn("گزینه 10", typeof(decimal))
                //});

                int row = 1;


                //foreach (var question in modelTosend.Where(m => m.QuestionType == Enums.QuestionType.Validation))
                //{
                //    int column = 0;
                //    DataRow validationRow = validationTable.NewRow();
                //    validationRow[column] = row;
                //    column++;
                //    validationRow[column] = question.Question;
                //    foreach (var answer in question.EachQuestionnaiireAnswerModels)
                //    {
                //        column++;
                //        validationRow[column] = (decimal)answer.Count / respondersNumber * 100;
                //    }
                //    row++;
                //    validationTable.Rows.Add(validationRow);
                //}
                DataTable mainTable = new DataTable();
                mainTable.Columns.AddRange(new DataColumn[12]
                {
                new DataColumn("ردیف", typeof(int)),
                new DataColumn("پرسش", typeof(string)),
                new DataColumn("گزینه 1",typeof(decimal)),
                new DataColumn("گزینه 2", typeof(decimal)),
                new DataColumn("گزینه 3", typeof(decimal)),
                new DataColumn("گزینه 4", typeof(decimal)),
                new DataColumn("گزینه 5",typeof(decimal)),
                new DataColumn("گزینه 6", typeof(decimal)),
                new DataColumn("گزینه 7", typeof(decimal)),
                new DataColumn("گزینه 8",typeof(decimal)),
                new DataColumn("گزینه 9", typeof(decimal)),
                new DataColumn("گزینه 10", typeof(decimal))
                });

                row = 1;

                foreach (var question in modelTosend.Where(m => m.QuestionType == Enums.QuestionType.Main ||
                                                                m.QuestionType == Enums.QuestionType.MainWithPicture))
                {
                    int column = 0;
                    DataRow mainRow = mainTable.NewRow();
                    mainRow[column] = row;
                    column++;
                    mainRow[column] = question.Question;
                    foreach (var answer in question.EachQuestionnaiireAnswerModels)
                    {
                        column++;
                        mainRow[column] = (decimal)answer.Count / respondersNumber * 100;
                    }
                    row++;
                    mainTable.Rows.Add(mainRow);
                }

                DataTable anatomicalTable = new DataTable();
                anatomicalTable.Columns.AddRange(new DataColumn[3]
                {
                new DataColumn("ردیف", typeof(int)),
                new DataColumn("پرسش", typeof(string)),
                new DataColumn("پاسخ",typeof(string)),

                });


                row = 1;

                foreach (var question in modelTosend.Where(m => m.QuestionType == Enums.QuestionType.Anatomical))
                {


                    foreach (var answer in question.EachQuestionnaiireAnswerModels)
                    {
                        DataRow anatomicalRow = anatomicalTable.NewRow();
                        anatomicalRow[0] = row;
                        anatomicalRow[1] = question.Question;
                        anatomicalRow[2] = answer.Answer;
                        anatomicalTable.Rows.Add(anatomicalRow);
                        row++;

                    }



                }
                //dt.Rows.Add(1, "C Sharp corner", "United States");

                //DataRow dr = dt.NewRow();
                //dr[0] = 123;
                //dr[1] = "123";
                //dt.Rows.Add(dr);

                //dt.Rows.Add(2, "Suraj", "India");
                //dt.Rows.Add(3, "Test User", "France");
                //dt.Rows.Add(4, "Developer", "Russia");
                ////Exporting to Excel
                ////string folderPath = "C:\\Excel\\";
                ////if (!Directory.Exists(folderPath))
                ////{
                ////    Directory.CreateDirectory(folderPath);
                ////}
                ////Codes for the Closed XML
                using (XLWorkbook wb = new XLWorkbook() { RightToLeft = true, })
                {
                   // wb.Worksheets.Add(validationTable, "سوالات راستی آزمایی");
                    wb.Worksheets.Add(mainTable, "سوالات اصلی");
                    wb.Worksheets.Add(anatomicalTable, "سوالات تشریحی");

                    //wb.SaveAs(folderPath + "DataGridViewExport.xlsx");
                    string myName = Server.UrlEncode("QuestionnairReport" + ".xlsx");
                    MemoryStream stream = GetStream(wb);// The method is defined below
                    Response.Clear();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition",
                    "attachment; filename=" + myName);
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
        }

        public MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }

        public ActionResult PaymentReport(int? page)
        {

            int pageSize = 10;
            int pageNumber = (page ?? 1);


            var userId = User.Identity.GetUserId();
            var report = _context.Payments.Where(p => p.FromUser_Id == userId).OrderBy(p => p.PaymentDate);

            return View(report.ToPagedList(pageNumber, pageSize));
        }
    }


}