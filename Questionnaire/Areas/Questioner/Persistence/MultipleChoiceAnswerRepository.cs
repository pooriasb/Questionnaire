using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;

namespace QuestionnaireProject.Areas.Questioner.Persistence
{
    public class MultipleChoiceAnswerRepository : IMultipleChoiceAnswerRepository
    {
    //    private readonly QuestionnaireEntities _context;

    //    public MultipleChoiceAnswerRepository(QuestionnaireEntities context)
    //    {
    //        _context = context;
    //    }
    //    public MultipleChoiceAnswer Create(string questionId, string answer, int answerChoice)
    //    {
    //        var newAnswer = new MultipleChoiceAnswer()
    //        {
    //            Id= answerChoice.ToString().PadLeft(3,'0')+"_"+Guid.NewGuid(),
    //            Answer = answer,
    //            Question_Id = questionId
    //        };
    //        return _context.MultipleChoiceAnswers.Add(newAnswer);
    //    }

    //    public IEnumerable<MultipleChoiceAnswer> CreateAll(string questionId, IEnumerable<string> answers)
    //    {
    //        int answerChoice = 1;
    //        foreach (var answer in answers)
    //        {
    //            if (answer!="")
    //            {
    //                Create(questionId, answer, answerChoice);
    //            }

    //            answerChoice++;
    //        }

    //        return _context.MultipleChoiceAnswers;
    //    }

    //    public MultipleChoiceAnswer Edit(string questionId, string answer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public MultipleChoiceAnswer Get(string id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IQueryable<MultipleChoiceAnswer> GetAllByQuestionId(string questionId)
    //    {
    //        return _context.MultipleChoiceAnswers.Where(a => a.Question_Id == questionId);
    //    }

    //    public void Delete(string multipleChoiceAnswerId)
    //    {
    //        //todo error handler
    //        var answerToDelete = _context.MultipleChoiceAnswers.Find(multipleChoiceAnswerId);
    //        if (answerToDelete!=null)
    //        {
    //            _context.MultipleChoiceAnswers.Remove(answerToDelete);

    //        }
    //    }

    //    public void DeleteAllByQuestionId(string questionId)
    //    {
    //        var answers = GetAllByQuestionId(questionId);

    //        foreach (var answer in answers)
    //        {
    //            Delete(answer.Id);
    //        }
    //    }

    //    public int AnswersCount(IEnumerable<string> answers)
    //    {
    //        int counter = 0;
    //        foreach (var answer in answers)
    //        {
    //            if (answer != "")
    //            {
    //                counter++;
    //            }
    //        }

    //        return counter;
    //    }
    }
}