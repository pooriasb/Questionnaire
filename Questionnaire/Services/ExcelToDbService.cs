using ClosedXML.Excel;
using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence;
using System;
using System.Linq;
using AnswerRepository = QuestionnaireProject.Areas.Questioner.Persistence.AnswerRepository;

//using Excel = Microsoft.Office.Interop.Excel;



namespace QuestionnaireProject.Services
{
    public class ExcelToDbService
    {
        private readonly QuestionnaireEntities _context;
        private readonly QuestionRepositpry _question;
        private readonly AnswerRepository _answers;
        //private readonly MultipleChoiceAnswerRepository _multiplechoiceAnswer;
        private readonly NotificationRepository _notification;


        public class ExcelToDbResult
        {
            public bool Result { get; set; }
            public string Message { get; set; }
        }
        public ExcelToDbService()
        {
            _context = new QuestionnaireEntities();
            _question = new QuestionRepositpry(_context);
            _answers = new AnswerRepository(_context);
            //_multiplechoiceAnswer = new MultipleChoiceAnswerRepository(_context);
            _notification = new NotificationRepository(_context);

        }

        public ExcelToDbResult SaveExcelToDb(string userId, int questionIndex, string questionnaireId, string fileName)
        {
            //Excel.Application xlApp = new Excel.Application();
            //Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/QuestionnaireFiles/"+fileName));
            //Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            //Excel.Range xlRange = xlWorksheet.UsedRange;            
            var workbook = new XLWorkbook(System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads/QuestionnaireFiles/" + fileName));
            var ws1 = workbook.Worksheet(1);
            int rowCount = ws1.RowsUsed().Count();
            int colCount = ws1.ColumnCount();

            if (rowCount > 101)
            {
                return new ExcelToDbResult()
                {
                    Message = "تعداد ردیفها بیش از حد مجاز میباشد. حداکثر مجاز به وارد نمودن 100 سوال در این بخش میباشید.",
                    Result = false
                };
            }

            //var questionnaire = new Questionnaire()
            //{
            //    Id = questionnaireId,
            //    Name = "پرهام تستی اکسلیییی",
            //    DateCreated = DateTime.Now,
            //    User_Id = userId,
            //    Status = (int)Enums.RequestStatus.Created
            //};
            //_context.Questionnaires.Add(questionnaire);

            //var condition = new Condition()
            //{
            //    Provience_Id = 1,
            //    City_Id = 1,
            //    Field_Id = 1,
            //    MaxAge = 99,
            //    MinAge = 1,
            //    PeopleCount = 123,
            //    Questionnaire_Id = questionnaireId,
            //    Sex = 1
            //};
            //_context.Conditions.Add(condition);
            var x = ws1.Row(1).Cell(1).GetValue<string>();
            for (int i = 1; i <= rowCount; i++)
            {
                if (ws1.Row(i).Cell(1).GetValue<string>() == "L")
                {
                    int answerCount = 0;
                    for (int j = 4; j < 20; j++)
                    {
                        if (ws1.Row(i).Cell(j).GetValue<string>() != "")
                        {
                            answerCount++;
                        }                        
                    }

                    if (answerCount < (int)Enums.BenchMarkingQuestion.MinAnswersNumber ||
                        answerCount > (int)Enums.BenchMarkingQuestion.MaxAnswersNumber)
                    {
                        return new ExcelToDbResult()
                        {
                            Result = false,
                            Message = $"تعداد پاسخ داده شده در سوال ردیف {i} خارج از حد مجاز است.",                            
                        };
                    }
                    var question = ws1.Row(i).Cell(2).GetValue<string>();
                    bool hasCorrectAnswer = false;
                    var newQuestion = new Question()
                    {
                        Questionnaire_Id = questionnaireId,
                        Id = (i + questionIndex).ToString().PadLeft(4, '0') + "_" + Guid.NewGuid(),
                        Text = question,
                        QuestionType = (int)Enums.QuestionType.BenchMarking,
                        MediaType = (int)Enums.MediaType.NoMedia,
                    };
                    _context.Questions.Add(newQuestion); var correctAnswer = int.Parse(ws1.Row(i).Cell(3).GetValue<string>());
                    answerCount = 1;
                    for (int j = 4; j < 13; j++)
                    {
                        if (ws1.Row(i).Cell(j).GetValue<string>() != "")
                        {
                            var answer = (ws1.Row(i).Cell(j).GetValue<string>());
                            if (j - 3 == correctAnswer)
                            {
                                hasCorrectAnswer = true;
                                _answers.Create(newQuestion.Id, answer, true, answerCount);
                            }
                            else
                            {
                                _answers.Create(newQuestion.Id, answer, false, answerCount);
                            }
                        }

                        answerCount++;
                    }
                    if (!hasCorrectAnswer)
                    {
                        return new ExcelToDbResult()
                        {
                            Result = false,
                            Message = $"پاسخ صحیح در پرسش ردیف {i} اشتباه وارد شده است.",
                        };
                    }


                }
                else if (ws1.Row(i).Cell(1).GetValue<string>() == "R")
                {
                    int answerCount = 0;
                    for (int j = 4; j < 20; j++)
                    {
                        if (ws1.Row(i).Cell(j).GetValue<string>() != "")
                        {
                            answerCount++;
                        }
                    }

                    if (answerCount < (int)Enums.ValidationQuestion.MinAnswersNumber ||
                        answerCount > (int)Enums.ValidationQuestion.MaxAnswersNumber)
                    {
                        return new ExcelToDbResult()
                        {
                            Result = false,
                            Message = $"تعداد پاسخ داده شده در سوال ردیف {i} کمتر از حد مجاز است.",
                        };
                    }

                    var question = ws1.Row(i).Cell(2).GetValue<string>();
                    bool hasCorrectAnswer = false;
                    var newQuestion = new Question()
                    {
                        Questionnaire_Id = questionnaireId,
                        Id = i.ToString().PadLeft(4, '0') + "_" + Guid.NewGuid(),
                        Text = question,
                        QuestionType = (int)Enums.QuestionType.Validation,
                        MediaType = (int)Enums.MediaType.NoMedia,
                    };
                    _context.Questions.Add(newQuestion);
                    var correctAnswer = int.Parse(ws1.Row(i).Cell(3).GetValue<string>());
                    answerCount = 0;
                    for (int j = 4; j < 13; j++)
                    {
                        if (ws1.Row(i).Cell(j).GetValue<string>() != "")
                        {
                            var answer = (ws1.Row(i).Cell(j).GetValue<string>());
                            if (j - 3 == correctAnswer)
                            {
                                hasCorrectAnswer = true;
                                _answers.Create(newQuestion.Id, answer, true, answerCount);
                            }
                            else
                            {
                                _answers.Create(newQuestion.Id, answer, false, answerCount);
                            }
                        }
                        answerCount++;
                    }
                    if (!hasCorrectAnswer)
                    {
                        return new ExcelToDbResult()
                        {
                            Result = false,
                            Message = $"پاسخ صحیح در پرسش ردیف {i} اشتباه وارد شده است.",
                        };
                    }
                }
                else if (ws1.Row(i).Cell(1).GetValue<string>() == "T")
                {
                    var id = Guid.NewGuid().ToString();
                    var question = ws1.Row(i).Cell(2).GetValue<string>();
                    var newQuestion = new Question()
                    {
                        Questionnaire_Id = questionnaireId,
                        Id = i.ToString().PadLeft(4, '0') + "_" + Guid.NewGuid(),
                        Text = question,
                        QuestionType = (int)Enums.QuestionType.Anatomical,
                        MediaType = (int)Enums.MediaType.NoMedia,
                    };
                    _context.Questions.Add(newQuestion);
                }
                else if (ws1.Row(i).Cell(1).GetValue<string>() == "C")
                {
                    int answerCount = 0;
                    for (int j = 4; j < 20; j++)
                    {
                        if (ws1.Row(i).Cell(j).GetValue<string>() != "")
                        {
                            answerCount++;
                        }
                    }

                    if (answerCount < (int)Enums.MainQuestion.MinAnswersNumber ||
                        answerCount > (int)Enums.MainQuestion.MaxAnswersNumber)
                    {
                        return new ExcelToDbResult()
                        {
                            Result = false,
                            Message = $"تعداد پاسخ داده شده در سوال ردیف {i} کمتر از حد مجاز است.",
                        };
                    }
                    var question = ws1.Row(i).Cell(2).GetValue<string>();
                    var newQuestion = new Question()
                    {
                        Questionnaire_Id = questionnaireId,
                        Id = i.ToString().PadLeft(4, '0') + "_" + Guid.NewGuid(),
                        Text = question,
                        QuestionType = (int)Enums.QuestionType.Main,
                        MediaType = (int)Enums.MediaType.NoMedia,
                    };
                    _context.Questions.Add(newQuestion);
                    answerCount = 1;
                    for (int j = 4; j < 13; j++)
                    {
                        if (ws1.Row(i).Cell(j).GetValue<string>() != "")
                        {
                            var answer = (ws1.Row(i).Cell(j).GetValue<string>());
                            _answers.Create(newQuestion.Id, answer, false, answerCount);
                        }
                        answerCount++;
                    }
                }
            }



            _context.SaveChanges();
            _notification.Add(new Notification()
            {
                FromUser_Id = null,
                ToUser_Id = userId,
                DateCreated = DateTime.Now,
                Title = "ثبت پرسشنامه از فایل اکسل",
                Text = "پرسشنامه شما با موفقیت از طریق فایل اکسل اضافه گردید. در صورت علاقه میتوانید آنرا ویرایش نمایید.",
                Type = (int)Enums.NotificationType.Warning
            });

            //GC.Collect();
            //GC.WaitForPendingFinalizers();

            //Marshal.ReleaseComObject(xlRange);
            //Marshal.ReleaseComObject(xlWorksheet);

            //xlWorkbook.Close();
            //Marshal.ReleaseComObject(xlWorkbook);

            //xlApp.Quit();
            //Marshal.ReleaseComObject(xlApp);
            return new ExcelToDbResult()
            {
                Result = true,
                Message = "پرسشنامه شما با موفقیت ثبت گردید."
            };
        }

    }
}