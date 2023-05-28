using QuestionnaireProject.Attributes;
using QuestionnaireProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuestionnaireProject.Areas.Questioner.Models
{
    public class QuestionnaireModel
    {
        public QuestionnairesDto QuestionnairesDto { get; set; }
        public NewQuestionnaireViewModel NewQuestionnaireViewModel { get; set; }
    }
    public class QuestionnairesDto
    {
        public IEnumerable<Questionnaire> Questionnaires { get; set; }
        public IEnumerable<Province> Provinces { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<StudyField> StudyFields { get; set; }
        public IEnumerable<QuestionnaireCategory> Categories { get; set; }


    }

    public class NewQuestionnaireViewModel
    {
        [Required(ErrorMessage = "ثبت {0} اجباری می باشد.")]
        [Display(Name = "نام پرسشنامه")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ثبت {0} اجباری می باشد.")]
        [Display(Name = "جنسیت")]
        [Range(0, 2, ErrorMessage = "لطفا عدد بزرگتر از صفر وارد نمایید.")]        
        public int Sex { get; set; }

        //[Required(ErrorMessage = "ثبت {0} اجباری می باشد.")]
        [Display(Name = "نام استان")]
        //[Range(0, int.MaxValue, ErrorMessage = "لطفا عدد بزرگتر از صفر وارد نمایید.")]
        public int? ProvinceId { get; set; }
        


        //[Required(ErrorMessage = "ثبت {0} اجباری می باشد.")]
        [Display(Name = "نام شهر")]
        //[Range(0, int.MaxValue, ErrorMessage = "لطفا عدد بزرگتر از صفر وارد نمایید.")]
        public int? CityId { get; set; }

        //[Required(ErrorMessage = "ثبت {0} اجباری می باشد.")]
        [Display(Name = "رشته تحصیلی")]
        //[Range(0, int.MaxValue, ErrorMessage = "لطفا عدد بزرگتر از صفر وارد نمایید.")]
        public int? StudyFieldId { get; set; }


        //todo write an attr to compare ages
        [Required(ErrorMessage = "ثبت {0} اجباری می باشد.")]
        [Display(Name = "حداقل سن")]
        [Range(1, int.MaxValue, ErrorMessage = "لطفا عدد بزرگتر از {0} وارد نمایید.")]
        public int MinAge { get; set; }

        [Required(ErrorMessage = "ثبت {0} اجباری می باشد.")]
        [Display(Name = "حداکثر سن")]
        [Range(1, int.MaxValue, ErrorMessage = "لطفا عدد بزرگتر از یک وارد نمایید.")]
        [AgeComparisonHigherAttr(ErrorMessage = "حداکثر سن بایستی بزرگتر و یا مساوی از حداقل سن باشد.")]
        public int MaxAge { get; set; }

        
        


        [Required(ErrorMessage = "ثبت {0} اجباری می باشد.")]
        [Display(Name = "تعداد افراد پاسخگو")]
        [Range(1, int.MaxValue, ErrorMessage = "لطفا عدد بزرگتر از یک وارد نمایید.")]
        public int PeopleCount { get; set; }

        [Required(ErrorMessage = "ثبت {0} اجباری می باشد.")]
        [Display(Name = "گروه")]
        [Range(1, int.MaxValue, ErrorMessage = "انتخاب گروه پرسشنامه الزامی میباشد.")]
        public int? CategoryId { get; set; }


        //to edit the questionnaire
        public string QuestionnaireId { get; set; }

    }

    
}