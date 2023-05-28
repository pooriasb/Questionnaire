using QuestionnaireProject.Areas.Questioner.Persistence;
using QuestionnaireProject.Areas.Questioner.Persistence.Repositories;
using QuestionnaireProject.Models;
using QuestionnaireProject.Models.DTOS;
using QuestionnaireProject.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace QuestionnaireProject.Services
{
    public class MainQuestionService
    {
        private readonly QuestionRepositpry _question;
        private readonly Areas.Questioner.Persistence.AnswerRepository _answer;
        private readonly MediaRepository _media;
        public MainQuestionService(QuestionnaireEntities context)
        {
            _question = new QuestionRepositpry(context);
            _answer = new Areas.Questioner.Persistence.AnswerRepository(context);
            _media = new MediaRepository(context);
        }

        public List<FullQuestionPackDto> GetMainQuestionPacks(string questionnaireId)
        {
            var questionPacks = new List<FullQuestionPackDto>();
            var questions = _question.GetAll(questionnaireId)
                .Where(q => q.QuestionType == (int)Enums.QuestionType.Main ||
                            q.QuestionType == (int)Enums.QuestionType.Anatomical ||
                            q.QuestionType == (int)Enums.QuestionType.MainWithPicture); ;
            if (questions != null)
            {
                foreach (var question in questions)
                {
                    var questionPack = new FullQuestionPackDto()
                    {
                        QuestionId = question.Id,
                        QuestionType = (Enums.QuestionType)question.QuestionType,
                        QuestionText = question.Text,
                        MediaType = (Enums.MediaType)question.MediaType,
                        AnswersText = _answer.GetAllByQuestionId(question.Id).Select(a => a.AnswerText).ToList(),
                        EditorComment = question.EditorComment
                    };
                    if (question.MediaType != (int)Enums.MediaType.NoMedia)
                    {
                        questionPack.MediaUrl = _media.GetByQuestionId(question.Id).Url;
                    }
                    questionPacks.Add(questionPack);
                }
            }
            return questionPacks;
        }

        public MainQuestionPackViewModel GetMainQuesitonPack(string questionId, string questionnaireId)
        {
            var question = _question.Get(questionId);
            var result = new MainQuestionPackViewModel()
            {
                QuestionId = question.Id,
                QuestionType = (Enums.QuestionType)question.QuestionType,
                QuestionText = question.Text,
                MediaType = (Enums.MediaType)question.MediaType,
            };

            if (question.MediaType != (int)Enums.MediaType.NoMedia)
            {
                result.MediaUrl = _media.GetByQuestionId(questionId).Url;
            }


            // The following code adds "" to the answers to view
            switch (result.QuestionType)
            {
                case Enums.QuestionType.Main:
                    //result.AnswersText = _multipleChoiceAnswer.GetAllByQuestionId(question.Id).Select(a => a.Answer)
                    //    .ToList();
                    result.AnswersText = _answer.GetAllByQuestionId(question.Id).Select(a => a.AnswerText)
                        .ToList();
                    var banswerCount = result.AnswersText.Count;
                    if (banswerCount <= (int)Enums.BenchMarkingQuestion.MaxAnswersNumber)
                    {
                        for (int i = 1; i <= (int)Enums.BenchMarkingQuestion.MaxAnswersNumber - banswerCount; i++)
                        {
                            result.AnswersText.Add("");
                        }
                    }
                    break;
                case Enums.QuestionType.MainWithPicture:
                    //result.AnswersText = _multipleChoiceAnswer.GetAllByQuestionId(question.Id).Select(a => a.Answer)
                    //    .ToList();
                    result.AnswersText = _answer.GetAllByQuestionId(question.Id).Select(a => a.AnswerText)
                        .ToList();
                    var wanswerCount = result.AnswersText.Count;
                    if (wanswerCount <= (int)Enums.BenchMarkingQuestion.MaxAnswersNumber)
                    {
                        for (int i = 1; i <= (int)Enums.BenchMarkingQuestion.MaxAnswersNumber - wanswerCount; i++)
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
        public void CreateMain(string questionnaireId, MainQuestionPackViewModel questionPack)
        {
            var newQuestion = _question.Create(questionnaireId,
                questionPack.QuestionText,
                questionPack.MediaType,
                questionPack.QuestionType);
            if (questionPack.QuestionType == Enums.QuestionType.Main || questionPack.QuestionType == Enums.QuestionType.MainWithPicture)
            {
                _answer.CreateAll(newQuestion.Id, questionPack.AnswersText); //todo: newly added and should be tested

            }
            if (questionPack.MediaType != Enums.MediaType.NoMedia)
            {
                _media.Create(newQuestion.Id, questionPack.MediaUrl);
            }
        }

        public void EditMain(MainQuestionPackViewModel questionPack)
        {

            //todo: how to edit answers?!
            _question.Edit(questionPack.QuestionId,
                questionPack.QuestionText,
                questionPack.MediaType,
                questionPack.QuestionType);
            _answer.DeleteAllByQuestionId(questionPack.QuestionId); //todo: newly added and should be tested

            if (questionPack.QuestionType == Enums.QuestionType.Main)
            {
                _answer.CreateAll(questionPack.QuestionId, questionPack.AnswersText); //todo: newly added and should be tested

            }
            if (questionPack.MediaType != Enums.MediaType.NoMedia)
            {
                //it edits the media if there is anything other than NoMedia
                _media.Edit(questionPack.QuestionId, questionPack.MediaUrl);
            }
            else if (questionPack.MediaType == Enums.MediaType.NoMedia)
            {

                // It deletes the media if there is no media in the model.
                _media.Delete(questionPack.QuestionId);
            }
        }
        public void DeleteMain(string questionId)
        {
            _question.Delete(questionId);
        }
    }
}