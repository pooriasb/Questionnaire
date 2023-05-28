using QuestionnaireProject.Areas.Questioner.Persistence;
using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;
using QuestionnaireProject.Models;
using QuestionnaireProject.Models.DTOS;
using QuestionnaireProject.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace QuestionnaireProject.Services
{
    public class VBQuestionService
    {
        private readonly QuestionRepositpry _question;
        //private readonly VBAnswerRepository _vbAnswer;
        private readonly VBCorrectAnswerRepository _vbCorrectAnswer;
        private readonly AnswerRepository _answer;


        public VBQuestionService(QuestionnaireEntities _context)
        {
            _question = new QuestionRepositpry(_context);
            //_vbAnswer = new VBAnswerRepository(_context);
            _answer = new QuestionnaireProject.Areas.Questioner.Persistence.AnswerRepository(_context);
            //_vbCorrectAnswer = new VBCorrectAnswerRepository(_context);
        }

        
        public List<FullQuestionPackDto> GetVBQuestionPacks(string questionnaireId)
        {
            var questionPacks = new List<FullQuestionPackDto>();
            var questions = _question.GetAll(questionnaireId)
                .Where(q=>q.QuestionType == (int) Enums.QuestionType.BenchMarking ||
                          q.QuestionType == (int) Enums.QuestionType.Validation);
            if (questions != null)
            {
                foreach (var question in questions)
                {
                    var questionPack = new FullQuestionPackDto()
                    {                                                
                        QuestionId = question.Id,
                        QuestionType = (Enums.QuestionType) question.QuestionType,
                        QuestionText = question.Text,
                        AnswersText = _answer.GetAllByQuestionId(question.Id).Select(a => a.AnswerText).ToList(),
                        EditorComment = question.EditorComment
                    };

                    questionPacks.Add(questionPack);
                }
            }
            return questionPacks;
        }

        public VBQuestionPackViewModel GetVBQuesitonPack(string questionId, string questionnaireId)
        {
            var question = _question.Get(questionId);
            var result = new VBQuestionPackViewModel()
            {
                
                QuestionId = question.Id,
                AnswersId = _answer.GetAllByQuestionId(question.Id).Select(a => a.Id).ToList(),
                QuestionType = question.QuestionType,
                QuestionText = question.Text,
                AnswersText = _answer.GetAllByQuestionId(question.Id).Select(a => a.AnswerText).ToList(),
                CorrectAnswerChoiceNumber = _answer.GetCorrectAnswerChoiceNumber(question.Id)
            };


            // The following code adds "" to the answers to view
            switch ((Enums.QuestionType)result.QuestionType)
            {
                case Enums.QuestionType.BenchMarking:
                    var banswerCount = result.AnswersText.Count;
                    if (banswerCount <= (int)Enums.BenchMarkingQuestion.MaxAnswersNumber)
                    {
                        for (int i = 1; i <= (int)Enums.BenchMarkingQuestion.MaxAnswersNumber - banswerCount; i++)
                        {
                            result.AnswersText.Add("");
                            result.AnswersId.Add("");
                        }
                    }
                    break;
                case Enums.QuestionType.Validation:
                    var vanswerCount = result.AnswersText.Count;
                    if (vanswerCount <= (int)Enums.ValidationQuestion.MaxAnswersNumber)
                    {
                        for (int i = 1; i <= (int)Enums.ValidationQuestion.MaxAnswersNumber - vanswerCount; i++)
                        {
                            result.AnswersText.Add("");
                        }
                    }
                    break;              
                default:
                    break;
            }
                       
            return result;
        }

        public void CreateVB(string questionnaireId,VBQuestionPackViewModel questionPack)
        {
            var newQuestion = _question.Create(questionnaireId,
                questionPack.QuestionText,
                Enums.MediaType.NoMedia,
                (Enums.QuestionType)questionPack.QuestionType);


            // this function find the correct answer and save the correct answer as a bit in DB
            var counter = 1;
            var answerCount = 1;
            foreach (var answer in questionPack.AnswersText)
            {
                if (answer != "")
                {
                    _answer.Create(newQuestion.Id, answer,
                        questionPack.CorrectAnswerChoiceNumber == counter, answerCount);                    
                    answerCount++;
                }

                counter++;


            }
            
        }

        public void EditVB(VBQuestionPackViewModel questionPack)
        {
            _question.Edit(questionPack.QuestionId, questionPack.QuestionText, Enums.MediaType.NoMedia, (Enums.QuestionType) questionPack.QuestionType);
            _answer.DeleteAllByQuestionId(questionPack.QuestionId);
            // this function find the correct answer and save the correct answer as a bit in DB
            var counter = 1;
            int answerCount=1;
            foreach (var answer in questionPack.AnswersText)
            {
                if (answer != "")
                {
                    _answer.Create(questionPack.QuestionId, answer,
                        questionPack.CorrectAnswerChoiceNumber == counter, answerCount);                    
                    answerCount++;
                }

                counter++;


            }

        }
        public void DeleteVB(string questionId)
        {
            _question.Delete(questionId);
        }
    }
}