using QuestionnaireProject.Models;
using QuestionnaireProject.Models.DTOS;
using System.Collections.Generic;
using System.Web;

namespace QuestionnaireProject.Areas.Editor.Models
{
    public class EditorReviewModel
    {
        public ReviewDto ReviewDto { get; set; }
        public EditorAssert EditorAssert { get; set; }
    }

    public class EditorAssert
    {
        public string AssertionComment { get; set; }       
    }

    public class QuestionPack
    {
        public string QuestionnaireId { get; set; }
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<string> AnswersText { get; set; }
        public List<string> AnswerId { get; set; }
        public Enums.QuestionType QuestionType { get; set; }
        public string MediaUrl { get; set; }
        public Enums.MediaType MediaType { get; set; }
        public string CorrectAnswer { get; set; }
        public HttpPostedFileBase Picture { get; set; }
        public string EditorComment { get; set; }
    }

    public class QuestionerInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public byte[] NationalCardImage { get; set; }
    }
    public class ReviewDto
    {
        public string QuestionnaireId { get; set; }
        public string QuestionnaireName { get; set; }
        public QuestionerInfo QuestionerInfo { get; set; }
        public ConditionDto ConditionDto { get; set; }
        public List<QuestionPack> QuestionPacks { get; set; }

    }
}