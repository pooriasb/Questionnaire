using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Questioner.Models
{
    public class RequestModel
    {
        public IEnumerable<RequestDto> RequestDtos { get; set; } 
    }

    public class RequestDto
    {
        public string QuestionnaireId { get; set; }
        public string QuestionnaireName { get; set; }
        public Enums.RequestStatus RequestStatus { get; set; }
        public DateTime UpdateDate { get; set; }
        public string EditorComment { get; set; }
    }
}