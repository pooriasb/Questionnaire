using QuestionnaireProject.Models;
using QuestionnaireProject.Models.DTOS;
using QuestionnaireProject.Models.ViewModels;
using System.Collections.Generic;

namespace QuestionnaireProject.Services
{

    public class QuestionService
    {
        private readonly QuestionnaireEntities _context;
        public MainQuestionService MainQuestionService { get; set; }
        public VBQuestionService VBQuestionService { get; set; }

        public QuestionService(QuestionnaireEntities context)
        {
            _context = context;
            MainQuestionService = new MainQuestionService(_context);
            VBQuestionService = new VBQuestionService(_context);
        }

        public List<FullQuestionPackDto> GetQuestionPacks(string questionnaireId)
        {
            var questionPacks = new List<FullQuestionPackDto>();
            questionPacks.AddRange(MainQuestionService.GetMainQuestionPacks(questionnaireId));
            questionPacks.AddRange(VBQuestionService.GetVBQuestionPacks(questionnaireId));

            return questionPacks;
        }

        public object GetQuestionPack(string questionId, string quesitonnaireId, Enums.QuestionType questionType)
        {
            if (questionType == Enums.QuestionType.Validation || questionType == Enums.QuestionType.BenchMarking)
            {
                return VBQuestionService.GetVBQuesitonPack(questionId, questionId);

            }
            else if (questionType == Enums.QuestionType.Main || 
                     questionType == Enums.QuestionType.Anatomical ||
                     questionType == Enums.QuestionType.MainWithPicture)
            {
                return MainQuestionService.GetMainQuesitonPack(questionId, questionId);

            }
            return null;

        }

        // This Function checks the Validaiton and returns error
        public string ConditionValidator(string questionnaireId,
            Enums.QuestionType questionType,
            Enums.QuestionServiceMode questionServiceMode,
            object questionPack)
        {


            switch (questionType)
            {
                case Enums.QuestionType.BenchMarking:
                    VBQuestionValidator(questionnaireId, questionServiceMode, questionPack as VBQuestionPackViewModel);
                    break;
                case Enums.QuestionType.Validation:
                    VBQuestionValidator(questionnaireId, questionServiceMode, questionPack as VBQuestionPackViewModel);
                    break;
                case Enums.QuestionType.Main:
                    MainQuestionValidator(questionnaireId, questionServiceMode, questionPack as MainQuestionPackViewModel);
                    break;
                case Enums.QuestionType.Anatomical:
                    AnatomicalQuestionValidator(questionnaireId, questionServiceMode, questionPack as MainQuestionPackViewModel);
                    break;
                case Enums.QuestionType.MainWithPicture:
                    MainQuestionValidator(questionnaireId, questionServiceMode, questionPack as MainQuestionPackViewModel);
                    break;

            }

            return null;
        }

       

        private void MainQuestionValidator(string questionnaireId,
            Enums.QuestionServiceMode questionServiceMode,
            MainQuestionPackViewModel questionPack)
        {


            switch (questionServiceMode)
            {
                case Enums.QuestionServiceMode.CreateMode:

                    MainQuestionService.CreateMain(questionnaireId, questionPack);
                    break;
                case Enums.QuestionServiceMode.EditMode:                    
                    MainQuestionService.EditMain(questionPack);
                    break;
                case Enums.QuestionServiceMode.DeleteMode:
                    MainQuestionService.DeleteMain(questionPack.QuestionId);
                    break;
                default:
                    break;
            }

        }


        private void AnatomicalQuestionValidator(string questionnaireId, 
            Enums.QuestionServiceMode questionServiceMode, 
            MainQuestionPackViewModel questionPack)
        {
                switch (questionServiceMode)
                {
                    case Enums.QuestionServiceMode.CreateMode:
                        
                        MainQuestionService.CreateMain(questionnaireId, questionPack);
                        break;
                    case Enums.QuestionServiceMode.EditMode:
                        
                        MainQuestionService.EditMain(questionPack);
                        break;
                    case Enums.QuestionServiceMode.DeleteMode:
                        MainQuestionService.DeleteMain(questionPack.QuestionId);
                        break;
                    default:
                        break;
                


            }
            
        }



        private void VBQuestionValidator(string questionnaireId, Enums.QuestionServiceMode questionServiceMode,
                                                        object questionPack)
        {

            switch (questionServiceMode)
            {
                case Enums.QuestionServiceMode.CreateMode:
                    VBQuestionService.CreateVB(questionnaireId, questionPack as VBQuestionPackViewModel);
                    break;
                case Enums.QuestionServiceMode.EditMode:
                    VBQuestionService.EditVB(questionPack as VBQuestionPackViewModel);
                    break;
                case Enums.QuestionServiceMode.DeleteMode:
                    //VBQuestionService.DeleteVB(questionPack.QuestionId); it is not necessary
                    break;
                default:
                    break;
            }

        }
    }
}