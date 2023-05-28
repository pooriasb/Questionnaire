using QuestionnaireProject.Areas.Questioner.Services;
using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestionnaireProject.Areas.Questioner.Persistence.Repositories
{
    public class QuestionRepositpry : IQuestionRepository
    {
        private readonly QuestionnaireEntities _context;

        public QuestionRepositpry(QuestionnaireEntities context)
        {
            _context = context;
        }
        public Question Create(string questionnaireId, string text, Enums.MediaType mediaType, Enums.QuestionType questionType)
        {
            var lastQuestion = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId).Select(q => q.Id)
                .ToList().LastOrDefault();
            int questionCount = 1;
            if (lastQuestion != null)
            {
                string idSub = lastQuestion.Substring(0, 4);
                questionCount = int.Parse(idSub)+1;
            }
           
            var newQuestion = new Question()
            {
                Questionnaire_Id = questionnaireId,
                Id = questionCount.ToString().PadLeft(4,'0') +"_"+ Guid.NewGuid(),
                Text = text,
                QuestionType = (int)questionType,
                MediaType = (int) mediaType,
            };
            _context.Questions.Add(newQuestion);
            return newQuestion;
        }
       

        public Question Edit(string questionId, string text, Enums.MediaType mediaType, Enums.QuestionType questionType)
        {
            var question = _context.Questions.Find(questionId);
            question.Text = text;
            question.QuestionType = (int)questionType;
            question.MediaType = (int) mediaType;
            return question;
        }

        public Question Edit(string questionId, string text)
        {
            var question = _context.Questions.Find(questionId);
            question.Text = text;            
            return question;
        }

        public Question Get(string questionId)
        {
            return _context.Questions.FirstOrDefault(q => q.Id == questionId);
        }

        public IEnumerable<Question> GetAll(string questionnaireId)
        {
            return _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId);
        }

        public void Delete(string questionId)
        {
            var question = _context.Questions.Find(questionId);
            if (question != null)
            {
                _context.Questions.Remove(question);
            }
        }
    }
}