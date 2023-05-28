using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;

namespace QuestionnaireProject.Areas.Questioner.Persistence
{
    public class VBCorrectAnswerRepository:IVBCorrectAnswerRepository
    {
        //private readonly QuestionnaireEntities _context;

        //public VBCorrectAnswerRepository(QuestionnaireEntities context)
        //{
        //    _context = context;
        //}
        //public int? CorrectAnswerSelector(int? correctAnswer, List<string> vbAnswers)
        //{
        //    int counter = 1;
        //    int vbAnswersCount = vbAnswers.Count();
        //    for (int i = 0; i < vbAnswersCount; i++)
        //    {
        //        if (vbAnswers[i] != "")
        //        {
        //            if (correctAnswer - 1 == i)
        //            {
        //                return counter;
        //            }

        //            counter++;
        //        }
        //    }
        //    return null;
        //}

        //public VBCorrectAnswer Create(int questionId, int correctAnswer)
        //{
        //    var newCorrectAnswer = new VBCorrectAnswer()
        //    {
        //        Question_Id = questionId,
        //        Answer_Id = correctAnswer
        //    };
            
        //    return _context.VBCorrectAnswers.Add(newCorrectAnswer);
        //}

        //public VBCorrectAnswer Edit(int vbCorrectAnswerId, int questionId, int correctAnswer)
        //{
        //    var newCorrectAnswer = _context.VBCorrectAnswers.Find(vbCorrectAnswerId);
        //    newCorrectAnswer.Question_Id = questionId;
        //    newCorrectAnswer.Answer_Id = correctAnswer;
        //    return newCorrectAnswer;
        //}

        //public VBCorrectAnswer EditByQuestionId(int questionId, int correctAnswer)
        //{
        //    //todo check for null and try to avoid that
        //    int vbCorrectAnswerId = GetIdByQuestionId(questionId);
        //    VBCorrectAnswer editedAnswer = Edit(vbCorrectAnswerId, questionId, correctAnswer);
        //    return editedAnswer;
        //}

        //public int GetIdByQuestionId(int questionId)
        //{
        //    //todo check the null
        //    int? correctAnswerId = _context.VBCorrectAnswers.Where(a=>a.Question_Id==questionId).Select(a=>a.Id).FirstOrDefault();
        //    if (correctAnswerId != null)
        //    {
        //        return (int)correctAnswerId;
        //    }

        //    return 0;
        //}

        //public int GetAnswerByQuestionId(int questionId)
        //{
        //    //todo check the logics and null exceptions
        //    int? correctAnswer = _context.VBCorrectAnswers.Where(a => a.Question_Id == questionId).Select(a => a.Answer_Id).FirstOrDefault();
        //    if (correctAnswer != null)
        //    {
        //        return (int)correctAnswer;
        //    }
        //    var correctAnswers = _context.VBCorrectAnswers.ToList();
        //    return 0;
        //}
    }
}