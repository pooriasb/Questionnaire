using Microsoft.AspNet.Identity;
using PagedList;
using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Areas.Questioner.Models;
using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;
using QuestionnaireProject.Models;
using QuestionnaireProject.Models.DTOS;
using QuestionnaireProject.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QuestionnaireProject.Areas.Questioner.Controllers
{
    [Authorize(Roles = "Questioner")]

    public class QuestionnaireForQuestionerController : Controller
    {
        private readonly QuestionnaireEntities _context;
        private readonly QuestionnaireRepository _questionnaire;
        private readonly PaymentRepository _payment;
        private readonly NotificationRepository _notification;
        private readonly QuestionRepositpry _question;
        private AnswerRepository _answer;
      
        public QuestionnaireForQuestionerController()
        {
            _context = new QuestionnaireEntities();
            _questionnaire = new QuestionnaireRepository(_context);
            _payment = new PaymentRepository(_context);
            _notification = new NotificationRepository(_context);
            _question = new QuestionRepositpry(_context);
            _answer = new AnswerRepository(_context);            

        }

        #region Create Edit Delete
        /// <summary>
        /// This dto prepares the needed data to pass to the view.
        /// </summary>
        /// <returns></returns>
        private QuestionnairesDto DTO()
        {
            var _userId = User.Identity.GetUserId();

            List<Province> provinces = _context.Provinces.ToList();
            //provinces.Insert(0, new Province() { Name = "همه" });

            List<City> cities = new List<City>();
            //cities.Insert(0, new City() { Name = "همه" });

            List<StudyField> studyFields = _context.StudyFields.ToList();
            //studyFields.Insert(0, new StudyField() { Name = "همه" });

            List <QuestionnaireCategory> questionnaireCategories = _context.QuestionnaireCategories.ToList();

            var dto = new QuestionnairesDto()
            {
                Questionnaires = _context.Questionnaires.Include(q => q.Questions).Where(q => q.User_Id == _userId &&
                                                                      (q.Status == (int)Enums.RequestStatus.Created ||
                                                                       q.Status == (int)Enums.RequestStatus.Accepted ||
                                                                       q.Status == (int)Enums.RequestStatus.Rejected))
                    .ToList(),
                Provinces = provinces,
                Cities = cities,
                StudyFields = studyFields,
                Categories = questionnaireCategories
            };
            return dto;
        }


        /// <summary>
        /// This ActionResult returns the data of questions and also here we can add or edit questionnaire options.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Index(QuestionnaireModel model)
        {
            model.QuestionnairesDto = DTO();
            if (model.NewQuestionnaireViewModel == null)
            {
                model.NewQuestionnaireViewModel = new NewQuestionnaireViewModel();
            }
            return View(model);
        }


        /// <summary>
        /// This ActionResult create or edit the fields of a questionnaire.
        /// depends on the questionnaireId it can decide whether to create the questionnaire or edit it.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult CreateQuestionnaire(QuestionnaireModel model)
        {
            if (!ModelState.IsValid)
            {

                model.QuestionnairesDto = DTO();
                ViewBag.Expanded = true;
                return View("Index", model);
            }

            var userId = User.Identity.GetUserId();
            var questionnaireCount = _questionnaire.GetAll().Count(q => q.User_Id == userId &&
                                                                        q.Status == (int)Enums.RequestStatus.Created);


            //todo control the number of open questionnaires.
            //todo just can change the questionnaire in status create accept and reject... and just his or her user Id
            if (questionnaireCount <= (int)Enums.Questionnaire.MaxQuestionnairesNumberForEachUser)
            {
                var questionniareId = model.NewQuestionnaireViewModel.QuestionnaireId;
                if (questionniareId == null)
                {
                    var saveModel = model.NewQuestionnaireViewModel;
                    var questionnaire = new Questionnaire()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = saveModel.Name,
                        DateCreated = DateTime.Now,
                        User_Id = User.Identity.GetUserId(),
                        Status = (int)Enums.RequestStatus.Created,
                        AnsweredByPerson = 0, //before any payment
                        Category_Id = saveModel.CategoryId,
                        RemainMoney = 0, //before any payment
                        Editor_Id = null
                    };

                    var condition = new Condition()
                    {
                        Provience_Id = saveModel.ProvinceId,
                        City_Id = saveModel.CityId,
                        Field_Id = saveModel.StudyFieldId,
                        MaxAge = saveModel.MaxAge,
                        MinAge = saveModel.MinAge,
                        PeopleCount = saveModel.PeopleCount,
                        Questionnaire_Id = questionnaire.Id,
                        Sex = saveModel.Sex
                    };
                    _context.Questionnaires.Add(questionnaire);
                    _context.Conditions.Add(condition);
                }
                if (questionniareId != null)
                {
                    var saveModel = model.NewQuestionnaireViewModel;
                    var questionnaire = _context.Questionnaires.Find(questionniareId);
                    if (questionnaire!=null)
                    {
                        questionnaire.Name = saveModel.Name;
                        questionnaire.Category_Id = saveModel.CategoryId;
                    }

                    var conditionId = _context.Conditions.Single(c => c.Questionnaire_Id == questionniareId).Id;
                    var condition = _context.Conditions.Find(conditionId);
                    if (condition != null)
                    {
                        condition.Provience_Id = saveModel.ProvinceId;
                        condition.City_Id = saveModel.CityId;
                        condition.Field_Id = saveModel.StudyFieldId;
                        condition.MaxAge = saveModel.MaxAge;
                        condition.MinAge = saveModel.MinAge;
                        condition.PeopleCount = saveModel.PeopleCount;
                        condition.Questionnaire.Name = saveModel.Name;
                        condition.Sex = saveModel.Sex;
                        //questionnaire.User_Id = User.Identity.GetUserId();
                    }
                }
            }
            else
            {
                return HttpNotFound();
            }





            _context.SaveChanges();
            return RedirectToAction("Index");



        }


        /// <summary>
        /// This ActionResult submit and edit the changes in the questionnaire applied by the questioner.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public ActionResult Edit(QuestionnaireModel model, string questionnaireId)
        {
            //todo control the userId, accept, create, reject
            //this actionResult returns a view which contains the data from db 
            //to edit the fields

            model.QuestionnairesDto = DTO();
            var conditionId = _context.Conditions.Single(c => c.Questionnaire_Id == questionnaireId).Id;
            var condition = _context.Conditions.Find(conditionId);
            var questionnaire = _context.Questionnaires.Find(questionnaireId);

            //this is a backward loading of data from conditions
            if (condition != null)
            {
                model.NewQuestionnaireViewModel = new NewQuestionnaireViewModel()
                {
                    Name = condition.Questionnaire.Name,
                    ProvinceId = condition.Provience_Id ?? 0,
                    CityId = condition.City_Id ?? 0,
                    StudyFieldId = condition.Field_Id ?? 0,
                    MaxAge = condition.MaxAge ?? 1,
                    MinAge = condition.MinAge ?? 99,
                    QuestionnaireId = questionnaireId,
                    PeopleCount = condition.PeopleCount ?? 1,
                    Sex = condition.Sex ?? 2,
                    CategoryId = questionnaire?.Category_Id

                };
                //model.NewQuestionnaireViewModel = questionnaireToEdit;
                ViewBag.Expanded = true;
                return View("Index", model);
            }


            return RedirectToAction("Index");



        }


        //todo check whether it is okay to be async or not

        /// <summary>
        /// This ActionResult Delete a record of the questionner.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public ActionResult Delete(string questionnaireId)
        {

            //todo control the userId, accept, create, reject

            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            if (questionnaire != null)
            {
                //todo add questionnaireId to Conditions Table to Remove the dependencies
                //var condition = _context.Conditions.Find(questionnaire.Condition_Id);
                //if (condition != null)
                //{
                //   _context.Conditions.Remove(condition);
                //}
                _context.Questionnaires.Remove(questionnaire);
                _context.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        #endregion
        #region Pricing and Payment
        public class QuestionnairePricingModel
        {
            public string QuestionnaireId { get; set; }
            public QuestionnairePricingDto QuestionnairePricingDto { get; set; }
            public QuestionnairePricingViewModel QuestionnairePricingViewModel { get; set; }
        }

        public class QuestionnairePricingDto
        {
            public string QuestionnaireName { get; set; }
            public int PeopleCount { get; set; }
            public int MultipleChoiceAmount { get; set; }
            public int WithPictureAmount { get; set; }
            public int WithFilmAmount { get; set; }
            public int AnatomicalAmount { get; set; }
            public int SitePercent { get; set; }
            public int RefererPercent { get; set; }
            public bool HasReferer { get; set; }

        }

        public class QuestionnairePricingViewModel
        {
            public string QuestionnaireId { get; set; }

            [Required(ErrorMessage = "وارد نمودن مبلغ واحد الزامی است.")]
            [Range(1, int.MaxValue, ErrorMessage = "تنها اعداد مثبت و بزرگتر از یک قابل قبولند.")]
            public int MultipleChoicePrice { get; set; }

            [Required(ErrorMessage = "وارد نمودن مبلغ واحد الزامی است.")]
            [Range(1, int.MaxValue, ErrorMessage = "تنها اعداد مثبت و بزرگتر از یک قابل قبولند.")]
            public int WithPicturePrice { get; set; }

            [Required(ErrorMessage = "وارد نمودن مبلغ واحد الزامی است.")]
            [Range(1, int.MaxValue, ErrorMessage = "تنها اعداد مثبت و بزرگتر از یک قابل قبولند.")]
            public int WithFilmPrice { get; set; }

            [Required(ErrorMessage = "وارد نمودن مبلغ واحد الزامی است.")]
            [Range(1, int.MaxValue, ErrorMessage = "تنها اعداد مثبت و بزرگتر از یک قابل قبولند.")]
            public int AnatomicalPrice { get; set; }

            [Required(ErrorMessage = "وارد نمودن مبلغ واحد الزامی است.")]
            [Range(1, int.MaxValue, ErrorMessage = "تنها اعداد مثبت و بزرگتر از یک قابل قبولند.")]
            public string DiscountCode { get; set; }
        }

        public ActionResult QuestionnairePricing(string questionnaireId)
        {
            var model = new QuestionnairePricingModel();
            model.QuestionnaireId = questionnaireId;
            var questionnaire = _context.Questionnaires.Where(q => q.Id == questionnaireId).Select(q => new
            {
                q.Id,
                q.Name,
                PeopleCount = q.Conditions.FirstOrDefault(cond => cond.Questionnaire_Id == q.Id).PeopleCount,
                MultipleChoiceCount = q.Questions.Count(c => c.MediaType == (int)Enums.MediaType.NoMedia &&
                                                             (c.QuestionType == (int)Enums.QuestionType.Main ||
                                                             c.QuestionType == (int)Enums.QuestionType.BenchMarking ||
                                                             c.QuestionType == (int)Enums.QuestionType.Validation)),
                WithPictureCount = q.Questions.Count(c => c.QuestionType == (int)Enums.QuestionType.MainWithPicture || c.MediaType == (int)Enums.MediaType.Picture),
                WithFilmCount = q.Questions.Count(c => c.MediaType == (int)Enums.MediaType.Film),
                AnatomicalCount = q.Questions.Count(c => c.QuestionType == (int)Enums.QuestionType.Anatomical &&
                                                         c.MediaType == (int)Enums.MediaType.NoMedia)

            }).Single();
            var options = _context.Options.ToList();
            model.QuestionnairePricingDto = new QuestionnairePricingDto()
            {
                AnatomicalAmount = questionnaire.AnatomicalCount,
                MultipleChoiceAmount = questionnaire.MultipleChoiceCount,
                WithFilmAmount = questionnaire.WithFilmCount,
                WithPictureAmount = questionnaire.WithPictureCount,
                SitePercent = int.Parse(options.FirstOrDefault(o => o.Name == "SitePercent")?.Value),
                QuestionnaireName = questionnaire.Name,
                PeopleCount = (int)questionnaire.PeopleCount
            };

            // If this User Had any referer...
            string userId = User.Identity.GetUserId();
            
            if (_payment.HasReferer(userId))
            {
                model.QuestionnairePricingDto.HasReferer = true;
                model.QuestionnairePricingDto.RefererPercent = int.Parse(options.FirstOrDefault(o => o.Name == "RefererPercent")?.Value);
            }
            model.QuestionnairePricingViewModel = new QuestionnairePricingViewModel();
            return View(model);
        }


        /// <summary>
        /// This ActionResult returns a json which evaluate the validity of a code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult DiscountCodeValidator(string code)
        {
            int sitePercent = _payment.GetSitePercent();
            var userId = User.Identity.GetUserId();
            bool result = false;

            var ocName = _payment.GetDiscountOccasionalName();
            var refCodes = _payment.GetDiscaountReferenceCode(userId);
            if (code == ocName && code != "0")
            {
                sitePercent = _payment.GetDiscountOccasionalValue();
                result = true;
            }
            else if (refCodes.Contains(code))
            {
                sitePercent = _payment.GetDiscountReferenceValue(code);
                result = true;
            }

            return Json(new { result, sitePercent });
        }

        public class QuestionPaymentDto
        {
            public decimal PriceToPay { get; set; }
            public string QuestionnaireId { get; set; }
        }


        //todo: remain money value?? change the status of the questionnaire after payment.
        //todo: dont recalculate if once the user has set the values.
        /// <summary>
        /// This ActionReslut recalculate the questionnaire final price which can be paid by the user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult QuestionnairePayment(QuestionnairePricingModel model)
        {
            var questionnaire = _context.Questionnaires.Where(q => q.Id == model.QuestionnaireId).Select(q => new
            {
                q.Id,
                q.Name,
                PeopleCount = q.Conditions.FirstOrDefault(cond => cond.Questionnaire_Id == q.Id).PeopleCount,
                MultipleChoiceCount = q.Questions.Count(c => c.MediaType == (int)Enums.MediaType.NoMedia &&
                                                             (c.QuestionType == (int)Enums.QuestionType.Main ||
                                                              c.QuestionType == (int)Enums.QuestionType.BenchMarking ||
                                                              c.QuestionType == (int)Enums.QuestionType.Validation)),
                WithPictureCount = q.Questions.Count(c => c.QuestionType == (int)Enums.QuestionType.MainWithPicture || c.MediaType == (int)Enums.MediaType.Picture),
                WithFilmCount = q.Questions.Count(c => c.MediaType == (int)Enums.MediaType.Film),
                AnatomicalCount = q.Questions.Count(c => c.QuestionType == (int)Enums.QuestionType.Anatomical &&
                                                         c.MediaType == (int)Enums.MediaType.NoMedia)

            }).FirstOrDefault();
            var questionnaireToChange = _context.Questionnaires.Find(model.QuestionnaireId);
            //questionnaireToChange.DatePaid = DateTime.Now;

            int peopleCount = (int)questionnaire.PeopleCount;
            int anatomicalPrice = model.QuestionnairePricingViewModel.AnatomicalPrice;
            int anatomicalValue = questionnaire.AnatomicalCount;
            int anatomicalTotal = anatomicalValue * anatomicalPrice;

            int multipleChoicePrice = model.QuestionnairePricingViewModel.MultipleChoicePrice;
            int multipleChoiceValue = questionnaire.MultipleChoiceCount;
            int multipleChoiceTotal = multipleChoiceValue * multipleChoicePrice;

            int withPicturePrice = model.QuestionnairePricingViewModel.WithPicturePrice;
            int withPictureValue = questionnaire.WithPictureCount;
            int withPictureTotal = withPicturePrice * withPictureValue;


            int withFilmPrice = model.QuestionnairePricingViewModel.WithFilmPrice;
            int withFilmValue = questionnaire.WithFilmCount;
            int withFilmTotal = withFilmPrice * withFilmValue;

            int sitePercent = _payment.GetSitePercent();
            int refererPercent = 0;
            int moneyAmountToReferer = 0;
            var userId = User.Identity.GetUserId();


            var ocName = _payment.GetDiscountOccasionalName();
            var refCodes = _payment.GetDiscaountReferenceCode(userId);
            if (model.QuestionnairePricingViewModel.DiscountCode == ocName && model.QuestionnairePricingViewModel.DiscountCode != "0")
            {
                sitePercent = _payment.GetDiscountOccasionalValue();

            }
            else if (refCodes.Contains(model.QuestionnairePricingViewModel.DiscountCode))
            {
                sitePercent = _payment.GetDiscountReferenceValue(model.QuestionnairePricingViewModel.DiscountCode);
                _payment.DeleteDiscountRefernceCode(model.QuestionnairePricingViewModel.DiscountCode);

                //todo use try catch...
                var discount = _context.Discounts.Single(d => d.Id == model.QuestionnairePricingViewModel.DiscountCode);

                _context.Discounts.Remove(discount);

            }

            if (_payment.HasReferer(userId))
            {
                var refererId = _payment.GetRefererId(userId);
                if (refererId!=null)
                {
                    var result = _payment.CountDownRefererRemain(refererId, userId);
                    if (result==true)
                    {
                        refererPercent = _payment.GetRefererPercent();
                        sitePercent = sitePercent - refererPercent;
                        moneyAmountToReferer = refererPercent * peopleCount * (anatomicalTotal + multipleChoiceTotal + withPictureTotal + withFilmTotal) / 100;
                        _payment.PayMoneyToReferer(questionnaire.Id, refererId, refererPercent, moneyAmountToReferer);
                    }
                }                
            }
            

            questionnaireToChange.SitePercentPrice = sitePercent;
            questionnaireToChange.EachPersonMoneyPrice = (anatomicalTotal + multipleChoiceTotal + withPictureTotal + withFilmTotal);
            questionnaireToChange.SitePercentValue = sitePercent * peopleCount * (int)questionnaireToChange.EachPersonMoneyPrice / 100;


            questionnaireToChange.TotalPayment = peopleCount * (int)questionnaireToChange.EachPersonMoneyPrice + (int)questionnaireToChange.SitePercentValue + (int) moneyAmountToReferer;
            questionnaireToChange.RemainMoney = peopleCount * (int)questionnaireToChange.EachPersonMoneyPrice;
            decimal priceToPay = (decimal)questionnaireToChange.TotalPayment;

            _context.SaveChanges();
            return RedirectToAction("PaymentConfirmation", new { questionnaireId = questionnaire.Id });


            //var dto = new QuestionPaymentDto()
            //{
            //    QuestionnaireId = model.QuestionnaireId,
            //    PriceToPay = priceToPay
            //};

            //return View(dto);
        }


        /// <summary>
        /// This ActionResult sets the payment value to pass to the bank site.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <returns></returns>
        public async Task<ActionResult> PaymentConfirmation(string questionnaireId)
        {
            //todo is there any validation???!!
            var questionnaire = await _questionnaire.GetSingleAsync(questionnaireId);            
            
            if (questionnaire?.TotalPayment != null)
            {
                int paymentValue = (int) questionnaire.TotalPayment;
                System.Net.ServicePointManager.Expect100Continue = false;
                ZarinpalSandbox.PaymentGatewayImplementationServicePortTypeClient zp = new ZarinpalSandbox.PaymentGatewayImplementationServicePortTypeClient();

                var result = await zp.PaymentRequestAsync("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX", 
                    paymentValue, 
                    "پرسشنامه", 
                    "info@questionnaire.ir", 
                    "09171112356",
                    "http://intelligence.sabooki.ir/Questioner/QuestionnaireForQuestioner/PaymentResult/" + questionnaireId
                    );



                if (result.Body.Status == 100)
                {
                    // Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority);
                    Response.Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + result.Body.Authority);
                }
                else
                {
                    ViewBag.Error = "Error : " + result.Body.Status;
                }
            }


            return RedirectToAction("Index");
        }

        public ActionResult PaymentResult(string id)
        {

            var questionnaire = _questionnaire.GetSingle(id);
            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
            {
                if (Request.QueryString["Status"].ToString().Equals("OK"))
                {
                    long RefID;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    ZarinpalSandbox.PaymentGatewayImplementationServicePortTypeClient zp = new ZarinpalSandbox.PaymentGatewayImplementationServicePortTypeClient();

                    int Status = zp.PaymentVerification("YOUR-ZARINPAL-MERCHANT-CODE", Request.QueryString["Authority"].ToString(), (int)questionnaire.TotalPayment, out RefID);

                    if (Status == 100)
                    {
                        var fromUserId = _questionnaire.GetSingle(id).User_Id;
                        var toUserId = User.Identity.GetUserId();
                        int DaysToExpire = int.Parse(_context.Options.FirstOrDefault(o => o.Name == "DaysToExpire").Value);
                        _questionnaire.DateExprire(id, DateTime.Now.AddDays(DaysToExpire));
                        _questionnaire.DatePaied(id, DateTime.Now);
                        _payment.MakePayment(Enums.PaymentType.QuestionerToSiteForQuestionnaire, fromUserId, toUserId, DateTime.Now, (decimal)questionnaire.TotalPayment, "پول بابت پرسشنامه پرداخت شد.");
                        _notification.Add(new Notification()
                        {
                            DateCreated = DateTime.Now,
                            FromUser_Id = null,
                            ToUser_Id = _questionnaire.GetSingle(id).User_Id,
                            Title = "اعلان پرداخت",
                            Text = "پرداخت شما با موفقیت صورت گرفت.",
                            Type = (int)Enums.NotificationType.Success
                        });
                        _context.SaveChanges();
                        ViewBag.IsSuccess = true;
                        ViewBag.RefId = RefID;
                        // Response.Write("Success!! RefId: " + RefID);
                    }
                    else
                    {
                        ViewBag.Status = Status;
                    }

                }
                else
                {
                    ViewBag.Error = "Error! Authority: " + Request.QueryString["Authority"].ToString() + " Status: " +
                                    Request.QueryString["Status"].ToString();
                    Response.Write("Error! Authority: " + Request.QueryString["Authority"].ToString() + " Status: " + Request.QueryString["Status"].ToString());
                }
            }
            else
            {
                Response.Write("Invalid Input");
            }
            return View();
        }


        public class InProccessDto
        {
            public string QuestionnaireId { get; set; }
            public string QuestionnaireName { get; set; }
            public DateTime? DateExpire { get; set; }
            public int RespoderAmount { get; set; }
            public SpecialOffer SpecialOffer { get; set; }


        }
        public class InProccessModel
        {
            public IPagedList<InProccessDto> InProccessDtos { get; set; }
        }
        #endregion
        /// <summary>
        /// This ActionReslut return the InProccess questionnaires.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult InProccessQuestionnaires(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var userId = User.Identity.GetUserId();
            var questionnaires = _context.Questionnaires.OrderBy(q => q.DateExpire)
                                        .Where(q => q.Status == (int)Enums.RequestStatus.Paid &&
                                                    q.AspNetUser.Id == userId);


            var model = new InProccessModel();
            var dtos = new List<InProccessDto>();
            foreach (var questionnaire in questionnaires)
            {
                var dto = new InProccessDto()
                {
                    QuestionnaireId = questionnaire.Id,
                    DateExpire = questionnaire.DateExpire,
                    //SpecialOffer =new SpecialOffer()
                    //{
                    //    Type = questionnaire.d
                    //},
                    RespoderAmount =  (int)questionnaire.AnsweredByPerson,
                    QuestionnaireName = questionnaire.Name,

                };
                dto.SpecialOffer = questionnaire.SpecialOffer;

                dtos.Add(dto);
            }

            model.InProccessDtos = dtos.ToPagedList(pageNumber, pageSize);
            return View(model);
        }
        #region Specials
        public class SpecialModel
        {
            public string QuestionnaireId { get; set; }
            public List<SpecialOffer> SpecialOffers { get; set; }
        }

        public ActionResult Specials(string questionnaireId)
        {
            var model = new SpecialModel();
            model.QuestionnaireId = questionnaireId;
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            if (questionnaire != null && questionnaire.User_Id == User.Identity.GetUserId())
            {
                if (questionnaire.Special_Id==null)
                {
                    model.SpecialOffers = _context.SpecialOffers.ToList();
                }
                else
                {
                    model.SpecialOffers = _context.SpecialOffers.Where(s=>s.Type == (int) Enums.Special.ExtraDays).ToList();
                }
            }
            
            return View(model);
        }

        public class MakeSpecialInvoiceModel
        {
            public string QuestionnaireId { get; set; }
            public string QuestionnaireName { get; set; }
            public SpecialOffer Special { get; set; }
        }

        /// <summary>
        /// This ActionResult creates an invoice to pay.    
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="specialId"></param>
        /// <returns></returns>
        public ActionResult MakeSpecialInvoice(string questionnaireId, int specialId)
        {
            //todo only paid and userId
            try
            {
                var model = new MakeSpecialInvoiceModel()
                {
                    QuestionnaireId = questionnaireId,
                    QuestionnaireName = _questionnaire.GetSingle(questionnaireId).Name,
                    Special = _context.SpecialOffers.First(s => s.Id == specialId)
                };

                return View(model);
            }
            catch (Exception e)
            {
                return HttpNotFound(e.Message);
            }

        }

        /// <summary>
        /// This ActionResult returns the payment finallization.
        /// </summary>
        /// <param name="questionnaireId"></param>
        /// <param name="specialId"></param>
        /// <returns></returns>
        public async Task<ActionResult> MakeSpecialPayment(string questionnaireId, int specialId)
        {
            var special = await _context.SpecialOffers.FirstOrDefaultAsync(s => s.Id == specialId);
           

            if (special != null)
            {
                int paymentValue = (int)special.Price;
                System.Net.ServicePointManager.Expect100Continue = false;
                ZarinpalSandbox.PaymentGatewayImplementationServicePortTypeClient zp =
                    new ZarinpalSandbox.PaymentGatewayImplementationServicePortTypeClient();
                
                var result = await zp.PaymentRequestAsync("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                    paymentValue,
                    "پرسشنامه",
                    "info@questionnaire.ir",
                    "09171112356",
                    "http://intelligence.sabooki.ir/Questioner/QuestionnaireForQuestioner/SpecialPaymentResult?questionnaireId=" + questionnaireId+"&SpecialId=" + specialId);


                if (result.Body.Status == 100)
                {
                    // Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority);
                    Response.Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + result.Body.Authority);
                }
                else
                {
                    ViewBag.Error = "Error : " + result.Body.Status;
                }
            }
            return RedirectToAction("Index");

        }

        public ActionResult SpecialPaymentResult(string questionnaireId, int specialId)
        {
            var questionnaire = _questionnaire.GetSingle(questionnaireId);
            var special = _context.SpecialOffers.FirstOrDefault(s => s.Id == specialId);

            if (Request.QueryString["Status"] != "" && Request.QueryString["Status"] != null && Request.QueryString["Authority"] != "" && Request.QueryString["Authority"] != null)
            {
                if (Request.QueryString["Status"].ToString().Equals("OK"))
                {
                    long RefID;
                    System.Net.ServicePointManager.Expect100Continue = false;
                    ZarinpalSandbox.PaymentGatewayImplementationServicePortTypeClient zp = new ZarinpalSandbox.PaymentGatewayImplementationServicePortTypeClient();

                    var Status = zp.PaymentVerification("YOUR-ZARINPAL-MERCHANT-CODE", Request.QueryString["Authority"].ToString(), (int)special.Price, out RefID);

                    if (Status == 100)
                    {
                        var fromUserId = _questionnaire.GetSingle(questionnaireId).User_Id;
                        var toUserId = User.Identity.GetUserId();
                        if (special.Type == (int)Enums.Special.ExtraDays)
                        {
                            //todo check for nulls when receives data from db
                            _questionnaire.DateExprire(questionnaireId, ((DateTime)_questionnaire.GetSingle(questionnaireId).DateExpire).AddDays(special.Days ?? 0));
                        }
                        else
                        {
                            _questionnaire.DateSpecial(questionnaireId, DateTime.Now.AddDays(special.Days ?? 0));
                            _questionnaire.SpecialId(questionnaireId, specialId);
                        }
                        _payment.MakePayment(Enums.PaymentType.QuestionerToSiteForSpecial, fromUserId, toUserId, DateTime.Now, (decimal)questionnaire.TotalPayment, "پول بابت پرسشنامه پرداخت شد.");
                        _notification.Add(new Notification()
                        {
                            DateCreated = DateTime.Now,
                            FromUser_Id = null,
                            ToUser_Id = User.Identity.GetUserId(),
                            Title = "پرداخت",
                            Text = "پرداخت بابت ویژه سازی با موفقیت صورت پذیرفت.",
                            Type = (int)Enums.NotificationType.Success
                        });
                        _context.SaveChanges();
                        ViewBag.IsSuccess = true;
                        ViewBag.RefId = RefID;
                        // Response.Write("Success!! RefId: " + RefID);
                    }
                    else
                    {
                        ViewBag.Status = Status;
                    }

                }
                else
                {
                    ViewBag.Error = "Error! Authority: " + Request.QueryString["Authority"].ToString() + " Status: " +
                                    Request.QueryString["Status"].ToString();
                    Response.Write("Error! Authority: " + Request.QueryString["Authority"].ToString() + " Status: " + Request.QueryString["Status"].ToString());
                }
            }
            else
            {
                Response.Write("Invalid Input");
            }
            return View();


        }

        #endregion



        public class FinishedQuestionnaireModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public DateTime? DateExpire { get; set; }
            public int? Rate { get; set; }
            public int UnFinishedCount { get; set; }
            public int AllResponders { get; set; }
           
        }


        /// <summary>
        /// This ActionResult return the finished questionnaires.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult FinishedQuestionnaires(int? page)
        {
            //todo check for isExpired
            //todo make it for users
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            List<FinishedQuestionnaireModel> dto = new List<FinishedQuestionnaireModel>();
            string userId = User.Identity.GetUserId();
            var questionnaires = _context.Questionnaires
                    .Where(q => q.User_Id == userId && q.DateExpire < DateTime.Now)
                    .Select(q => new
                    {
                        q.Id,
                        q.Name,
                        q.DateExpire,
                        q.Rate
                    }).ToList();


            foreach (var questionnaire in questionnaires)
            {
                dto.Add(new FinishedQuestionnaireModel()
                {
                    DateExpire = questionnaire.DateExpire,
                    Name = questionnaire.Name,
                    Id = questionnaire.Id,
                    Rate = questionnaire.Rate,

                    AllResponders = _context.Responses.Where(q => q.Questionnaire_Id == questionnaire.Id && q.IsBanned==false).ToList().Count
                   ,
                    UnFinishedCount=  _context.Responses.Where(q => q.Questionnaire_Id == questionnaire.Id&&q.DateFinished == null).ToList().Count
            });
            }
            var model = dto.ToPagedList(pageNumber, pageSize);
            return View(model);
        }

        public class ResponsesReportDto
        {
            public int ResponseId { get; set; }
            public string ResponderId { get; set; }
            public string ResponderName { get; set; }
            public string ResponderLastName { get; set; }
            public DateTime? AnsweredDateTime { get; set; }
            public int? Score { get; set; }
        }
        public class DetailedReportModel
        {
            public string QuestionnaireId { get; set; }
            public string QuestionnaireName { get; set; }

            public IPagedList<ResponsesReportDto> ResponsesReportDtos { get; set; }
        }
        public ActionResult DetailedReport(string questionnaireId, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var model = new DetailedReportModel();
            var questionnaire = _questionnaire.GetSingle(questionnaireId);
            model.QuestionnaireName = questionnaire.Name;
            model.QuestionnaireId = questionnaire.Id;
            var responses = _context.Responses.Where(r => r.Questionnaire_Id == questionnaire.Id);
            if (responses.Count() != 0)
            {
                var dtos = new List<ResponsesReportDto>();

                foreach (var response in responses)
                {
                    var dto = new ResponsesReportDto()
                    {
                        ResponseId = response.Id,
                        ResponderId = response.User_Id,
                        ResponderName = response.AspNetUser.FirstName,
                        ResponderLastName = response.AspNetUser.LastName,
                        AnsweredDateTime = response.DateFinished,
                        Score = response.Score
                    };
                    dtos.Add(dto);
                }
                model.ResponsesReportDtos = dtos.ToPagedList(pageNumber, pageSize);

            }


            return View(model);
        }


        public ActionResult QuestionnaireFullView(string questionnaireId)
        {
            //this action result returns the all information about the questionnaire and also it can 
            // show the all relevant questioins.
            var questionnaire = _context.Questionnaires.FirstOrDefault(q => q.Id == questionnaireId);
            var questionnairePlus = _context.Questionnaires.Where(q => q.Id == questionnaireId).Select(x => new
            {
                
                x.AspNetUser.NationalCardImage,
                x.AspNetUser.National_Id,
                x.AspNetUser.FirstName,
                x.AspNetUser.LastName,
                x.EachPersonMoneyPrice,
                Condition = x.Conditions.Select(y => new
                {
                    y.MaxAge,
                    y.MinAge,
                    CityName = y.City.Name,
                    ProvinceName = y.Province.Name,
                    y.PeopleCount,
                    FieldName = y.StudyField.Name
                }).FirstOrDefault()

            }).FirstOrDefault();


            if (questionnairePlus != null && questionnaire != null)
            {
                var dto = new ReviewDto()
                {
                    QuestionnaireId = questionnaireId,
                    QuestionnaireName = questionnaire.Name,

                    // Added user info to ReviewDto
                    QuestionerInfo = new QuestionerInfo()
                    {
                        FirstName = questionnairePlus.FirstName,
                        LastName = questionnairePlus.LastName,
                        NationalCode = questionnairePlus.National_Id,
                        NationalCardImage = questionnairePlus.NationalCardImage
                    },
                    QuestionPacks = new List<QuestionPack>()

                };

                // Set the condtionDto to send to View
                dto.ConditionDto = new ConditionDto()
                {
                    Field = questionnairePlus.Condition.FieldName,
                    PeopleCount = questionnairePlus.Condition.PeopleCount,
                    EachPersonMoney = questionnairePlus.EachPersonMoneyPrice,
                    MinAge = questionnairePlus.Condition.MinAge,
                    MaxAge = questionnairePlus.Condition.MaxAge,
                    City = questionnairePlus.Condition.CityName,
                    Provience = questionnairePlus.Condition.ProvinceName

                };
                var questions = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId);
                foreach (var question in questions)
                {
                    var questionPack = new QuestionPack()
                    {
                        QuestionId = question.Id,
                        QuestionType = (Enums.QuestionType)question.QuestionType,
                        QuestionText = question.Text,
                        MediaType = (Enums.MediaType)question.MediaType,
                        EditorComment = question.EditorComment

                    };

                    if (questionPack.MediaType == Enums.MediaType.Film ||
                        questionPack.MediaType == Enums.MediaType.Picture)
                    {
                        questionPack.MediaUrl = _context.Medias.FirstOrDefault(m => m.Question_Id == question.Id).Url;
                    }

                    if (questionPack.QuestionType == Enums.QuestionType.Main ||
                        questionPack.QuestionType == Enums.QuestionType.MainWithPicture)
                    {
                        var answers = _context.Answers.Where(a => a.Question_Id == questionPack.QuestionId)
                            .Select(a => a.AnswerText).ToList();
                        questionPack.AnswersText = answers;
                    }

                    if (questionPack.QuestionType == Enums.QuestionType.BenchMarking ||
                        questionPack.QuestionType == Enums.QuestionType.Validation)
                    {
                        var answers = _context.Answers.Where(a => a.Question_Id == questionPack.QuestionId)
                            .Select(x => new
                            {
                                x.Id,
                                x.AnswerText,
                                x.IsCorrect
                            });
                        questionPack.AnswersText = answers.Select(a => a.AnswerText).ToList();
                        questionPack.AnswerId = answers.Select(a => a.Id).ToList();
                        questionPack.CorrectAnswer = answers.FirstOrDefault(a => a.IsCorrect == true).Id;

                    }

                    dto.QuestionPacks.Add(questionPack);
                }

                var model = new EditorReviewModel { ReviewDto = dto };
                return View(model);
            }

            return View("Index");
        }

        public ActionResult CopyQuestionnaire(string questionnaireId)
        {
            var questionnaire = _context.Questionnaires.Find(questionnaireId);
            var condition = _context.Conditions.FirstOrDefault(c => c.Questionnaire_Id == questionnaireId);
            var userId = User.Identity.GetUserId();
            if (questionnaire!= null && questionnaire.User_Id == userId && condition != null)
            {
                var newQuestionnaire = new Questionnaire()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = questionnaire.Name + "(کپی پرسشنامه: جهت تغییر ویرایش نمایید.)",
                    DateCreated = DateTime.Now,
                    User_Id = userId,
                    Status = (int) Enums.RequestStatus.Created,
                    AnsweredByPerson = 0, //before any payment
                    Category_Id = questionnaire.Category_Id,
                    RemainMoney = 0, //before any payment
                    Editor_Id = null

                };

                var newCondition = new Condition()
                {
                    Provience_Id = condition.Provience_Id,
                    City_Id = condition.City_Id,
                    Field_Id = condition.Field_Id,
                    MaxAge = condition.MaxAge,
                    MinAge = condition.MinAge,
                    PeopleCount = condition.PeopleCount,
                    Questionnaire_Id = newQuestionnaire.Id,
                    Sex = condition.Sex
                };

                _context.Questionnaires.Add(newQuestionnaire);
                _context.Conditions.Add(newCondition);
                var questions = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId).OrderBy(q=>q.Id).ToList();
                var answers = _context.Answers.Where(q => q.Question.Questionnaire_Id == questionnaireId).ToList();
                var medias = _context.Medias.Where(q => q.Question.Questionnaire_Id == questionnaireId).ToList();
                var questionCount = 1;
                foreach (var question in questions)
                {

                    var newQuestion = new Question()
                    {
                        Questionnaire_Id = newQuestionnaire.Id,
                        Id = questionCount.ToString().PadLeft(4, '0') + "_" + Guid.NewGuid(),
                        Text = question.Text,
                        QuestionType = question.QuestionType,
                        MediaType = question.MediaType
                    };
                    _context.Questions.Add(newQuestion);
                    if (question.MediaType != (int)Enums.MediaType.NoMedia)
                    {
                        _context.Medias.Add(new Media()
                        {
                            Question_Id = newQuestion.Id,
                            Url=medias.FirstOrDefault(m=>m.Question_Id == question.Id)?.Url
                        });
                    }

                    if (question.QuestionType != (int)Enums.QuestionType.Anatomical)
                    {
                        var answerCount = 1;
                        foreach (var answer in answers.OrderBy(a => a.Id).Where(q=>q.Question_Id == question.Id))
                        {
                            if (answer.IsCorrect == null)
                            {
                                answer.IsCorrect = false;
                            }

                            _answer.Create(newQuestion.Id, answer.AnswerText, (bool)answer.IsCorrect, answerCount);
                            answerCount++;
                        }

                    }

                    questionCount++;
                }

                _context.SaveChanges();

            }

            return RedirectToAction("Index");
        }




        public bool AddToArchive(string questionnairId) {

            try
            {
                var pachakge = _context.Questionnaires.Find(questionnairId);
                if (pachakge != null)
                {
                    IEnumerable<Question> questions = pachakge.Questions.ToList(); // find package questions
                                                                                   //  var responses = _context.Responses.Where(x => x.Questionnaire_Id == questionnairId).ToList() ;
                    List<UsersRespons> responsesToDetele = new List<UsersRespons>();//empty list for user responses that we want to delete
                    foreach (var item in questions)
                    {
                        IEnumerable<UsersRespons> response = _context.UsersResponses.Where(x => x.Question_Id == item.Id).ToList(); // find user responses for a question
                        responsesToDetele.AddRange(response);
                    }
                    //////////////
                    _context.UsersResponses.RemoveRange(responsesToDetele);// remove a range of user responses of package questions
                    _context.Questions.RemoveRange(questions); // remove a range of package questions
                    pachakge.Status = 9;
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;

                }
            }
            catch (Exception)
            {

                return false;
            }
      



           
        }

        public ActionResult Archive() {
            string userid = User.Identity.GetUserId();
         //   var res = _context.Questionnaires.ToList().Where(x => x.DateExpire?.AddDays(30) > DateTime.Now && x.Status == 9).ToList();
            var res = _context.Questionnaires.ToList().Where(x => x.DateExpire?.AddDays(30) > DateTime.Now && x.User_Id==userid).ToList();
            foreach (var item in res)
            {
                AddToArchive(item.Id);
            }
                ViewBag.Packages = res;
          
            return View();
        }





    }


}
