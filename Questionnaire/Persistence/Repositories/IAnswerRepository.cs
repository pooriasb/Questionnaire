using QuestionnaireProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace QuestionnaireProject.Persistence.Repositories
{
    interface IAnswerRepository
    {
        Answer Create(string questionId, string answer, bool isCorrect, int answerCount);
        IEnumerable<Answer> CreateAll(string questionId, IEnumerable<string> answers);
        Answer Edit(string questionId, string answer);
        Answer Get(string id);
        IQueryable<Answer> GetAllByQuestionId(string questionId);
        void Delete(string vbAnswerId);
        void DeleteAllByQuestionId(string questionId);
        int GetCorrectAnswerChoiceNumber(string questionId);
        int AnswersCount(IEnumerable<string> answers);
    }
}
