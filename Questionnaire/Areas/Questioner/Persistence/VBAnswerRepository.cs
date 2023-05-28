using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;

namespace QuestionnaireProject.Areas.Questioner.Persistence
{
    public class VBAnswerRepository : IVBAnswerRepository
    {
        //private readonly QuestionnaireEntities _context;

        //public VBAnswerRepository(QuestionnaireEntities context)
        //{
        //    _context = context;
        //}
        //public VBAnswer Create(string questionId, string answer, bool isCorrect, int answerCount)
        //{
        //    VBAnswer newAnswer = new VBAnswer()
        //    {
        //        Id = answerCount.ToString().PadLeft(3, '0')+"_"+Guid.NewGuid().ToString(),
        //        Answer = answer,
        //        Question_Id = questionId,
        //        IsCorrect = isCorrect
        //    };

        //    return _context.VBAnswers.Add(newAnswer);

        //}

        //public IEnumerable<VBAnswer> CreateAll(string questionId, IEnumerable<string> answers)
        //{
        //    List<VBAnswer> newAnswers = new List<VBAnswer>();



        //    foreach (var answer in answers)
        //    {


        //        if (answer != "")
        //        {
        //            VBAnswer newAnswer = new VBAnswer()
        //            {
        //                Answer = answer,
        //                Question_Id = questionId,
        //            };
        //            newAnswers.Add(newAnswer);
        //        }
        //    }
        //    _context.VBAnswers.AddRange(newAnswers);
        //    return newAnswers;
        //}

        //public VBAnswer Edit(string questionId, string answer)
        //{
        //    throw new NotImplementedException();
        //}

        //public VBAnswer Get(string id)
        //{
        //    throw new NotImplementedException();
        //}




        //public void Delete(string vbAnswerId)
        //{
        //    //todo error handler
        //    var answerToDelete = _context.VBAnswers.Find(vbAnswerId);
        //    _context.VBAnswers.Remove(answerToDelete);

        //}

        //public void DeleteAllByQuestionId(string questionId)
        //{
        //    var answerIds = GetAllByQuestionId(questionId);

        //    foreach (var answerId in answerIds)
        //    {
        //        Delete(answerId.Id);
        //    }
        //}

        //public int GetCorrectAnswerChoiceNumber(string questionId)
        //{
        //    var answers = _context.VBAnswers.Where(a => a.Question_Id == questionId);
        //    int correctAnswerChoiceNumber = 1;

        //    foreach (var answer in answers)
        //    {
        //        if (answer.IsCorrect == true)
        //        {
        //            return correctAnswerChoiceNumber;
        //        }
        //        correctAnswerChoiceNumber++;
        //    }

        //    return correctAnswerChoiceNumber;
        //}

        //public int AnswersCount(IEnumerable<string> answers)
        //{
        //    int counter = 0;
        //    foreach (var answer in answers)
        //    {
        //        if (answer != "")
        //        {
        //            counter++;
        //        }
        //    }

        //    return counter;
        //}

        //public IQueryable<Answer> GetAllByQuestionId(string questionId)
        //{
        //    return _context.Answers.Where(a => a.Question_Id == questionId);
        //}
    }
}