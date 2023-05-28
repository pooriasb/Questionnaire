using QuestionnaireProject.Areas.Editor.Models;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Questioner.Models
{
    public class QuestionModel
    {
        public string QuestionnaireName { get; set; }
        public QuestionPack QuestionPackViewModel { get; set; }
        public List<QuestionPack> QuestionPacksDto { get; set; }


    }
    
}