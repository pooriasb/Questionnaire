using QuestionnaireProject.Areas.Responder.Models;
using QuestionnaireProject.Areas.Responder.Persistence.Repositories;
using QuestionnaireProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestionnaireProject.Areas.Responder.Persistence
{
    public class ResponseRepository : IResponseRepository
    {
        public QuestionnaireEntities _context { get; set; }

        public ResponseRepository(QuestionnaireEntities context)
        {
            _context = context;
        }
        public bool UserEvaluation(string questionnaireId, string userId, Condition condition, bool isBanned)
        {
            var user = _context.AspNetUsers.Where(u => userId == u.Id).Select(u => new { u.CityId, u.BirthDate, u.StudyFieldId }).Single();
            var age = DateTime.Now.Year - user.BirthDate.Year;
            var response = _context.Responses.Where(r => r.Questionnaire_Id == questionnaireId && r.User_Id == userId);
            if (age <= condition.MaxAge && age >= condition.MinAge)
            {
                if (condition.Field_Id == user.StudyFieldId)
                {
                    if (condition.City_Id == user.CityId) // check for proviences
                    {

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public Respons CreateResponse(string userId, string questionnaireId)
        {
            //check the available response;
            var newResponse = new Respons()
            {
                User_Id = userId,
                Questionnaire_Id = questionnaireId,
                DateStarted = DateTime.Now,
                IsBanned = false
            };

            _context.Responses.Add(newResponse);
            return newResponse;
        }


        /// <summary>
        /// MrEvaluator receives the data from server and the responder and check whether the question and answers are 
        /// valid or not.
        /// </summary>
        /// <param name="questionAnswerModel"></param>
        /// <returns></returns>
        public bool MrEvaluator(List<ResponderQuesionPackValidatorModel> questionAnswerModel)
        {

            //q relates to questions

            foreach (var q in questionAnswerModel)
            {
                if (q.QuestionPackServerViewModel.QuestionType!=Enums.QuestionType.Anatomical)
                {
                    if (q.QuestionPackServerViewModel.AnswerId.Contains(q.ResponderQuestionPackViewModel.Answer))
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                
                return false;
            }
            return true;

        }

        /// <summary>
        /// MrCreator creates the responses depending on question type.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="questionAnswerModel"></param>
        public void MrCreator(string userId, List<ResponderQuesionPackValidatorModel> questionAnswerModel)
        {
            foreach (var qum in questionAnswerModel)
            {
                switch (qum.QuestionPackServerViewModel.QuestionType)
                {
                    case Enums.QuestionType.BenchMarking:
                        CreateVbResponse(userId, qum);
                        break;
                    case Enums.QuestionType.Validation:
                        CreateVbResponse(userId, qum);
                        break;
                    case Enums.QuestionType.Anatomical:
                        CreateAnatomicalResponse(userId, qum);
                        break;
                    case (Enums.QuestionType.Main):
                        CreateMultipleChoiceResponse(userId, qum);
                        break;
                    case Enums.QuestionType.MainWithPicture:
                        CreateMultipleChoiceResponse(userId, qum);
                        break;
                    case Enums.QuestionType.SiteBenchMarking:

                        //todo check it
                        CreateMultipleChoiceResponse(userId, qum);
                        break;
                }
            }

         

        }

        public void BanUser(string userId, string questionnaireId)
        {
            try
            {
                var response = _context.Responses.Single(r => r.AspNetUser.Id == userId &&
                                                              r.Questionnaire_Id == questionnaireId);
                response.DateFinished = DateTime.Now;
                response.IsBanned = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           

        }

        public void CreateVbResponse(string userId, ResponderQuesionPackValidatorModel questionAnswerModel)
        {
            var newResponse = new UsersRespons()
            {
                User_Id = userId,
                Response = questionAnswerModel.ResponderQuestionPackViewModel.Answer,
                IsCorrect = (questionAnswerModel.QuestionPackServerViewModel.CorrectAnswer ==
                             questionAnswerModel.ResponderQuestionPackViewModel.Answer)
            };
            _context.UsersResponses.Add(newResponse);
        }

        public void CreateAnatomicalResponse(string userId, ResponderQuesionPackValidatorModel questionAnswerModel)
        {
            var newResponse = new UsersRespons()
            {
                User_Id = userId,
                Question_Id = questionAnswerModel.ResponderQuestionPackViewModel.QuestionId,
                Response = questionAnswerModel.ResponderQuestionPackViewModel.Answer
            };
            _context.UsersResponses.Add(newResponse);
        }

        public void Finished(string userId, string questionnaireId)
        {
            try
            {
                var response = _context.Responses.Single(r => r.AspNetUser.Id == userId &&
                                                              r.Questionnaire_Id == questionnaireId);
                response.DateFinished = DateTime.Now;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public void CreateMultipleChoiceResponse(string userId, ResponderQuesionPackValidatorModel questionAnswerModel)
        {
            var newResponse = new UsersRespons()
            {
                User_Id = userId,
                Response = questionAnswerModel.ResponderQuestionPackViewModel.Answer
            };
            _context.UsersResponses.Add(newResponse);
        }

        public List<QuestionPackServerViewModel> FromServer(string questionnaireId)
        {
            var listQasm = new List<QuestionPackServerViewModel>();
            var questions = _context.Questions.Where(q => q.Questionnaire_Id == questionnaireId);
            foreach (var q in questions)
            {
                var qasm = new QuestionPackServerViewModel()
                {
                    QuestionId = q.Id,
                    QuestionType = (Enums.QuestionType)q.QuestionType,
                    AnswerId = new List<string>()
                };
                if (qasm.QuestionType == Enums.QuestionType.Validation ||
                    qasm.QuestionType == Enums.QuestionType.BenchMarking)
                {
                    var answers = q.Answers.Where(a => a.Question_Id == q.Id).Select(a => a.Id).ToList();
                    foreach (var answer in answers)
                    {
                        qasm.AnswerId.Add(answer);
                    }
                    qasm.CorrectAnswer =
                        q.Answers.First(a => a.Question_Id == q.Id && a.IsCorrect == true).Id;
                }
                else
                if (qasm.QuestionType == Enums.QuestionType.Main ||
                        qasm.QuestionType == Enums.QuestionType.MainWithPicture)
                {

                    var answers = q.Answers.Where(a => a.Question_Id == q.Id).Select(a => a.Id).ToList();
                    foreach (var answer in answers)
                    {
                        qasm.AnswerId.Add(answer);
                    }
                }

                listQasm.Add(qasm);
            }

            return listQasm;
        }

        public bool BenchMarkingEvaluation(List<ResponderQuesionPackValidatorModel> questionAnswerModel)
        {
            foreach (var q in questionAnswerModel)
            {
                if (q.QuestionPackServerViewModel.CorrectAnswer == q.ResponderQuestionPackViewModel.Answer)
                {
                    continue;
                }
                return false;

            }
            return true;
        }

        public bool BenchMarkingFinalDecision(List<ResponderQuesionPackValidatorModel> questionAnswerModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This function evaluate the responses received from the user and return the score.
        /// </summary>
        /// <param name="questionAnswerModel"></param>
        /// <returns></returns>
        public int ValidationEvaluation(List<ResponderQuesionPackValidatorModel> questionAnswerModel)
        {
            var validationAnswers = questionAnswerModel.Where(q =>
                q.QuestionPackServerViewModel.QuestionType == Enums.QuestionType.Validation).ToList();
            int correctAnswers = 0;
            foreach (var qa in validationAnswers)
            {
                if (qa.ResponderQuestionPackViewModel.Answer == qa.QuestionPackServerViewModel.CorrectAnswer)
                {
                    correctAnswers++;
                }
            }
            var score = 0;
            if (validationAnswers.Count!=0)
            {
                score = Convert.ToInt32(((decimal)correctAnswers / validationAnswers.Count) * 100);
            }
           

            return score;
        }

        /// <summary>
        /// This method uses the ValidationEvaluation to get the score and would set the value in database
        /// </summary>
        /// <param name="score"></param>
        /// <param name="questionnaireId"></param>
        /// <param name="userId"></param>
        public void SetValidationScore(int score,string questionnaireId, string userId)
        {
           
            try
            {
                var response = _context.Responses.Single(r => r.Questionnaire_Id == questionnaireId &&
                                                              r.User_Id == userId);
                response.Score = score;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void SetRate(string userId, string questionnaireId, int rate)
        {
            
            try
            {
                var response = _context.Responses.Single(r => r.Questionnaire_Id == questionnaireId &&
                                                              r.User_Id == userId);
                response.Rate = rate;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public bool SiteBenchMarkingEvaluation(List<ResponderQuesionPackValidatorModel> questionAnswerModel)
        {
            throw new NotImplementedException();
        }

        public void FinalizeResponse()
        {
            throw new NotImplementedException();
        }
    }
}