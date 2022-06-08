using System.Linq;
using System.Web.Http;
using _3F.Model;
using _3F.Model.Extensions;
using _3F.Model.Model;
using _3F.Web.Controllers.API.Model;
using _3F.Web.Extensions;
using _3F.Web.Models;

namespace _3F.Web.Controllers.API
{
    public class UserController : BaseApiController
    {
        public UserController(IRepository repository)
            : base(repository)
        {
        }

        [HttpGet]
        public IHttpActionResult Find(string q)
        {
            var users = repository.Where<AspNetUsers>(u => u.UserName.Contains(q.ToLower()) && u.EmailConfirmed).ToArray()
                .Select(u => new User(u)).ToArray();
            return Ok(users);
        }

        [HttpGet, ApiAuthorize, Compress]
        public IHttpActionResult Detail(string id)
        {
            var user = repository.OneByHtml<AspNetUsers>(id);
            var model = new ApiUserDetail()
            {
                Name = user.UserName,
                BirhtYear = user.Profiles.BirhtYear.HasValue ? user.Profiles.BirhtYear.ToString() : string.Empty,
                City = user.Profiles.City.NullToEmpty(),
                Hobbies = user.Profiles.Hobbies.NullToEmpty(),
                HtmlName = user.HtmlName,
                Link = user.Profiles.Link.NullToEmpty(),
                Motto = user.Profiles.Motto.NullToEmpty(),
                PhoneNumber = user.PhoneNumber.NullToEmpty(),
                Sex = user.Profiles.Sex.GetDescription(),
                Status = user.Profiles.Status.GetDescription(),
            };

            return Ok(model);
        }
    }  
}
