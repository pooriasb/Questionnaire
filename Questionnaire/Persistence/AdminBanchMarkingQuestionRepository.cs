using QuestionnaireProject.Areas.Editor.Models;
using QuestionnaireProject.Models;
using QuestionnaireProject.Persistence.Repositories;
using System;
using System.Data.Entity;
using System.Linq;

namespace QuestionnaireProject.Persistence
{
    public class AdminBanchMarkingQuestionRepository:IAdminBenchMarkingQuestionRepository
    {
        private readonly QuestionnaireEntities _context;

        public AdminBanchMarkingQuestionRepository(QuestionnaireEntities context)
        {
            _context = context;
        }
        public void Create(AdminNewQuestion question)
        {
            var newQuestion = _context.AdminQuestions.Add(new AdminQuestion()
            {
                Id = Guid.NewGuid().ToString(),
                Text = question.QuestionText,
            });

            _context.AdminAnswers.Add(new AdminAnswer()
            {
                Id = Guid.NewGuid().ToString(),
                AdminQuestion_Id = newQuestion.Id,
                IsCorrect = true,
                AnswerText = question.CorrectAnswer
            });
            foreach (var answer in question.AnswersText.Where(a => a != ""))
            {
                _context.AdminAnswers.Add(new AdminAnswer()
                {
                    Id = Guid.NewGuid().ToString(),
                    AdminQuestion_Id = newQuestion.Id,
                    IsCorrect = false,
                    AnswerText = answer
                });
            }
        }

        public void Delete(string questionId)
        {
            var question = _context.AdminQuestions.Find(questionId);
            if (question != null)
            {
                _context.AdminQuestions.Remove(question);
            }
        }

        public void Edit(AdminNewQuestion question)
        {
            Delete(question.QuestionId);
            Create(question);
        }

        public IQueryable<AdminQuestion> GetAll()
        {
            return _context.AdminQuestions.OrderBy(q => q.Id).Include(q => q.AdminAnswers);
        }

        public AdminNewQuestion GetSingle(string questionId)
        {
            var model = new AdminNewQuestion();
            var question = _context.AdminQuestions.FirstOrDefault(q => q.Id == questionId);
            if (question != null)
            {
                model = new AdminNewQuestion()
                {
                    QuestionText = question.Text,
                    CorrectAnswer = question.AdminAnswers.FirstOrDefault(q => q.IsCorrect == true)?.AnswerText,
                    AnswersText = question.AdminAnswers.Where(a=>a.IsCorrect!=true).Select(a => a.AnswerText).ToList(),
                    QuestionId = question.Id
                };
                int difference = (int)Enums.BenchMarkingQuestion.MaxNumber - question.AdminAnswers.Count;
                if (difference > 0)
                {
                    for (int i = 0; i < difference; i++)
                    {
                        model.AnswersText.Add("");
                    }
                }
            }

            return model;
        }
    }
}