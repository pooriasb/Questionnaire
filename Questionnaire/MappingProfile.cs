using AutoMapper;
using QuestionnaireProject.Models;
using QuestionnaireProject.Models.DTOS;

namespace QuestionnaireProject
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Condition Mapping
            CreateMap<Condition, ConditionDto>();


            
        }
    }
}