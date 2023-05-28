using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;
using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestionnaireProject.Areas.Questioner.Persistence
{
    public class AnswerRepository : IAnswerRepository
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
                Id = answerCount.ToString().PadLeft(3, '0')+"_"+Guid.NewGuid().ToString(),
                AnswerText = answer,
                Question_Id = questionId,
                IsCorrect = isCorrect
            };

            return _context.Answers.Add(newAnswer);

        }

        public IEnumerable<Answer> CreateAll(string questionId, IEnumerable<string> answers)
        {
            List<Answer> newAnswers = new List<Answer>();


            int count = 1;
            foreach (var answer in answers)
            {


                if (answer != "")
                {
                    Create(questionId, answer, false, count);
                }

                count++;
            }
            // _context.Answers.AddRange(newAnswers);
            return newAnswers;
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