using System;

namespace QuestionnaireProject.Areas.Editor.Models
{
    public class UsersDto
    {
        public string NationalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? IsBlocked { get; set; }
    }
}