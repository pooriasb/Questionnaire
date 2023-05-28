using QuestionnaireProject.Models.DTOS;
using QuestionnaireProject.Models.ViewModels;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Questioner.Models
{
    public class QuestionForQuestionerModel
    {
        public string QuestionnaireId { get; set; }
        public string QuestionnaireName { get; set; }
        public VBQuestionPackViewModel VbQuestionPackViewModel { get; set; }
        public IEnumerable<FullQuestionPackDto> FullQuestionPackDtos { get; set; }
        public MainQuestionPackViewModel MainQuestionPackViewModel { get; set; }
    }
}