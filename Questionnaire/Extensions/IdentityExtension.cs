using System.Security.Claims;
using System.Security.Principal;

namespace QuestionnaireProject.Extensions
{
    public static class IdentityExtension
    {
        public static string GetFirstName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("FirstName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetLastName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("LastName");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetStudyFieldId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("StudyFieldId");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetCityId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("CityId");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetBirthDate(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("BirthDate");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetPId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("PId");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetSexId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Sex");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static string GetPhoneNumber(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("PhoneNumber");
            // Test for null to avoid issues during local testing
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
