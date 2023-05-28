using System.Web.Http;

namespace QuestionnaireProject.Controllers.API
{
    //[EnableCors(origins: "https://51cfcce3.ngrok.io", headers: "*", methods: "*")]
    public class TestController : ApiController
    {
        public class TestAccountAndroid : AccountController
        {
            
        }
        //public class AnswerResult
        //{
        //    public string Question { get; set; }
        //    public string Answer { get; set; }
        //}
            
        //[HttpPost]
        //public IHttpActionResult SaveAnswers(List<AnswerResult> answer)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest();

        //    return Json(answer.Last());
        //}

        //[HttpGet]
        //public IHttpActionResult TestGet()
        //{
        //    //var db = new QuestionnaireEntities();
        //    //var query = db.Questionnaires.Select(x => x.Name).ToList();
        //    //return Json(new
        //    //{
        //    //    list = query
        //    //});
        //}

        public class Test
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Family { get; set; }
        }

        [HttpPost]
        public IHttpActionResult TestPost([FromBody]Test test)
        {
                       

            return Json(new
            {
                message = $"Hi{test.Name} {test.Family} with code {test.Id}"
            });
        }

        [HttpGet]
        public IHttpActionResult GetTest()
        {
            return Ok();
        }
    }
}
