using System.Linq;
using System.Web.Http;
using _3F.Model;
using _3F.Model.Model;
using _3F.Web.Definitions;

namespace _3F.Web.Controllers.API
{
    public class AdministraceController : BaseApiController
    {

        public AdministraceController(IRepository repository)
            : base(repository)
        {
        }

        [HttpGet, Authorize(Roles= "Administrator,Council,Supervisor")]
        public IHttpActionResult Uzivatele()
        {
            var model = repository.All<AspNetUsers>()
                .ToArray()
                .Select(u => new [] 
                {
                    u.Id.ToString(), 
                    u.UserName, u.Email, 
                    u.DateCreated.ToString(Strings.DateFormat), 
                    u.LoginType.ToString(),
                    u.Profile_Id.ToString(), 
                    u.VariableSymbol?.ToString() ?? "",
                    u.DateLastActivity.ToString(Strings.DateFormat),
                }).ToArray();

            return Ok(new { data = model });
        }

        [HttpGet, Authorize(Roles = "Administrator")]
        public IHttpActionResult Akce()
        {
            var model = repository.All<Event>()
                .ToArray()
                .Select(u => new []
                {
                    u.Id.ToString(),
                    u.Name,
                    u.StartDateTime.ToString(Strings.DateFormat),
                    u.EventType.ToString(),
                    u.State.ToString(),
                    u.HtmlName,
                }).ToArray();

            return Ok(new { data = model });
        }
    }
}
