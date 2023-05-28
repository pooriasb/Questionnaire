using Microsoft.Owin;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace QuestionnaireProject.Controllers.API
{
    public class UserProfileController : ApiController
    {
        public class UserProfileInfo
        {
            [MaxLength(20, ErrorMessage = "حداکثر 20 کاراکتر مجاز است.")]
            public string FirstName { get; set; }

            [MaxLength(20, ErrorMessage = "حداکثر 20 کاراکتر مجاز است.")]
            public string LastName { get; set; }

            [DataType(dataType:DataType.DateTime)]
            public DateTime BirthDate { get; set; }

            [EmailAddress]
            public string Email { get; set; }

        }

        [HttpGet]
        public IHttpActionResult UserInfo()
        {
            var a = 1;
            return Json(new {a});
        }

        [HttpPost]
        public IHttpActionResult UserInfo(QuestionnaireProject.Models.RegisterViewModel model)
        {

            var a = 1;
            return Json(new { a });
        }

        [HttpPost]
        public IHttpActionResult UserNationalCardPicture(FormCollection collection)
        {
            var a = 1;
            return Json(new { a });
        }
    }

    
}
