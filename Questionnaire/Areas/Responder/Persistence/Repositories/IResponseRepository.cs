using QuestionnaireProject.Areas.Responder.Models;
using QuestionnaireProject.Models;
using System.Collections.Generic;

namespace QuestionnaireProject.Areas.Responder.Persistence.Repositories
{
    public interface IResponseRepository
    {
        bool UserEvaluation(string questionnaireId,string userId, Condition condition, bool isBanned);
        Respons CreateResponse(string userId, string questionnaireId);
        bool MrEvaluator(List<ResponderQuesionPackValidatorModel> questionAnswerModel);
        void MrCreator(string userId, List<ResponderQuesionPackValidatorModel> questionAnswerModel);
        void BanUser(string userId, string questionnaireId);
        

        List<QuestionPackServerViewModel> FromServer(string questionnaireId); //Gets all questions and answers from server.


        ///////////////////////////////VBAnswers///////////////////////////////
        void CreateVbResponse(string userId, ResponderQuesionPackValidatorModel questionAnswerModel);

        //BenchMarking Answers
        bool BenchMarkingEvaluation(List<ResponderQuesionPackValidatorModel> questionAnswerModel); //

        bool BenchMarkingFinalDecision(List<ResponderQuesionPackValidatorModel> questionAnswerModel); // can allow to next step or not
        
        //Validation Answers
        int ValidationEvaluation( List<ResponderQuesionPackValidatorModel> questionAnswerModel); // return score

        void SetValidationScore(int score, string questionnaireId ,string userId);
        //Site BenchMarkimg Answers
        bool SiteBenchMarkingEvaluation(List<ResponderQuesionPackValidatorModel> questionAnswerModel); //


        /////////////////////////////////////////Answers//////////////////////////

        //MultipleChoice Answers       
        void CreateMultipleChoiceResponse(string userId, ResponderQuesionPackValidatorModel questionAnswerModel);
        //Anatomical Answers
        void CreateAnatomicalResponse(string userId, ResponderQuesionPackValidatorModel questionAnswerModel);

        //finish date
        void Finished(string userId, string questionnaireId);


        //Set Rate Value
        void SetRate(string userId, string questionnaireId, int rate);
        void FinalizeResponse();
    }
}