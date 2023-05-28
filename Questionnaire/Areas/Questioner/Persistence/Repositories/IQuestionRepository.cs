using QuestionnaireProject.Models;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Questioner.Services
{
    interface IQuestionRepository
    {
        Question Create(string questionnaireId, string text, Enums.MediaType mediaType,Enums.QuestionType questionType);
        Question Edit(string questionId, string text, Enums.MediaType mediaType, Enums.QuestionType questionType);
        Question Get(string questionId);
        IEnumerable<Question> GetAll(string questionnaireId);
        void Delete(string questionId);
    }
}
