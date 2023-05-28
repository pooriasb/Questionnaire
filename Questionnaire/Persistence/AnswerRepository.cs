using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestionnaireProject.Persistence
{
    class AnswerRepository : IAnswerRepository
    {
        private readonly QuestionnaireEntities _context;

        public AnswerRepository(QuestionnaireEntities context)
        {
            _context = context;
        }
        public Answer Create(string questionId, string answer, bool isCorrect, int answerCount)
        {
            Answer newAnswer = new Answer()
            {
                Id = answerCount.ToString().PadLeft(3, '0') + "_" + Guid.NewGuid().ToString(),
                AnswerText = answer,
                Question_Id = questionId,
                IsCorrect = isCorrect
            };

            return _context.Answers.Add(newAnswer);

        }

        public IEnumerable<Answer> CreateAll(string questionId, IEnumerable<string> answers)
        {
            //just should be used for main questions because it doesnt get the true answer


            int answerChoice = 1;
            foreach (var answer in answers)
            {


                if (answer != "")
                {
                    Create(questionId, answer, false, answerChoice);
                }

                answerChoice++;
            }
            return _context.Answers;
        }

        public Answer Edit(string questionId, string answer)
        {
            throw new NotImplementedException();
        }

        public Answer Get(string id)
        {
            throw new NotImplementedException();
        }




        public void Delete(string vbAnswerId)
        {
            //todo error handler
            var answerToDelete = _context.Answers.Find(vbAnswerId);
            _context.Answers.Remove(answerToDelete);

        }

        public void DeleteAllByQuestionId(string questionId)
        {
            var answerIds = GetAllByQuestionId(questionId);

            foreach (var answerId in answerIds)
            {
                Delete(answerId.Id);
            }
        }

        public int GetCorrectAnswerChoiceNumber(string questionId)
        {
            var answers = _context.Answers.Where(a => a.Question_Id == questionId);
            int correctAnswerChoiceNumber = 1;

            foreach (var answer in answers)
            {
                if (answer.IsCorrect == true)
                {
                    return correctAnswerChoiceNumber;
                }
                correctAnswerChoiceNumber++;
            }

            return correctAnswerChoiceNumber;
        }

        public int AnswersCount(IEnumerable<string> answers)
        {
            int counter = 0;
            foreach (var answer in answers)
            {
                if (answer != "")
                {
                    counter++;
                }
            }

            return counter;
        }

        public IQueryable<Answer> GetAllByQuestionId(string questionId)
        {
            return _context.Answers.Where(a => a.Question_Id == questionId);
        }
    }
}
