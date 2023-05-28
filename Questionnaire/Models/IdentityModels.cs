using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;


namespace QuestionnaireProject.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(40)]
        public string FirstName { get; set; }

        [MaxLength(40)]
        public string LastName { get; set; }

        [Required]
        public int StudyFieldId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        [Column(TypeName = "Date")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "وارد نمودن کد ملی الزامی است.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "تنها عدد قابل قبول است.")]
        [Display(Name = "کد ملی")]
        public string National_Id { get; set; }

        [Required]
        public int Sex { get; set; }


        public string PId { get; set; }

        public string UniqueCode { get; set; }

        public bool HasPaymentRequest { get; set; }

        public string BankId { get; set; }

        public decimal MoneyBalance { get; set; }

        public byte[] NationalCardImage { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FirstName", this.FirstName.ToString()));
            userIdentity.AddClaim(new Claim("StudyFieldId", this.StudyFieldId.ToString()));
            userIdentity.AddClaim(new Claim("LastName", this.LastName.ToString()));
            userIdentity.AddClaim(new Claim("NationalId", this.National_Id.ToString()));
            userIdentity.AddClaim(new Claim("CityId", this.CityId.ToString()));
            userIdentity.AddClaim(new Claim("BirthDate", this.BirthDate.ToString()));
            userIdentity.AddClaim(new Claim("Sex", this.Sex.ToString()));
            //userIdentity.AddClaim(new Claim("PId", this.PId.ToString()));
            userIdentity.AddClaim(new Claim("PhoneNumber", this.PhoneNumber.ToString()));
            userIdentity.AddClaim(new Claim("PhoneNumberConfirmed", this.PhoneNumberConfirmed.ToString()));

            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}