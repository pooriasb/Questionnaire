using System.ComponentModel.DataAnnotations;

namespace QuestionnaireProject.Areas.Editor.Models
{
    public class EditorDto
    {
        public int RecievedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int AcceptedRequests { get; set; }
        public int RecievedFiles { get; set; }

        public int NotSubmitedQuestionnaires { get; set; }
        public int NotPaidQuestionnaires { get; set; }
        public int PaidQuestionnaires { get; set; }
        public int FinishedQuestionnaires { get; set; }
    }
    public class SiteContentModel {
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string SiteTitle { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]

        public string SiteBody1 { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]

        public string SiteBody2 { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string Sitepurpose { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string SiteAbout { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string SV1 { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string SV2 { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string SV3 { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string SV1Title { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string SV2Title { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string SV3Title { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string Address { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string Email { get; set; }
        [Required(ErrorMessage = "ورود این گزینه الزامی است")]
        public string Tell { get; set; }
    }
    public class CategoryModel {

        public int? id { get; set; }
        public string Name { get; set; }
    }
}