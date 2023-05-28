using PagedList;
using System;
using System.ComponentModel.DataAnnotations;

namespace QuestionnaireProject.Areas.Editor.Models
{
    public class EditorQuestionnaireModel
    {
        public QuestionnaireDto QuestionnaireDto { get; set; }
    }

    public class QuestionnaireDto
    {
        public IPagedList<QuestionnairePack> QuestionnairePacks { get; set; }
    }

    public class QuestionnairePack
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public int? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime? DateExpire { get; set; }
        public int? SpecialId { get; set; }
        public decimal? EachPersonMoneyPrice { get; set; }
        public decimal? SitePercentValue { get; set; }
        public decimal? SitePercentPrice { get; set; }
        public decimal? TotalPayment { get; set; }
        public decimal? Rate { get; set; }

        

    }
}