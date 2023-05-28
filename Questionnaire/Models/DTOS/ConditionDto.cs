namespace QuestionnaireProject.Models.DTOS
{
    public class ConditionDto
    {
        public int? MaxAge { get; set; }
        public int? MinAge { get; set; }
        public int? PeopleCount { get; set; }
        public decimal? EachPersonMoney { get; set; }
        public string Field { get; set; }
        public string City { get; set; }
        public string Provience { get; set; }
    }
}