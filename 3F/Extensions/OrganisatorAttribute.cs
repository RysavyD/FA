using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3F.Model;
using _3F.Model.Model;

namespace _3F.Web.Extensions
{
    public class OrganisatorAttribute : AuthorizeAttribute
    {
        private readonly IRepository _repository;
        private readonly string[] _roles;

        public OrganisatorAttribute(params string[] roles)
        {
            _repository = DependencyResolver.Current.GetService<IRepository>();
            _roles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                var eventHtmlName = (string)httpContext.Request.RequestContext.RouteData.Values["id"];

                if (!string.IsNullOrWhiteSpace(eventHtmlName))
                {
                    var eventEntity = _repository.OneByHtml<Event>(eventHtmlName);
                    if (eventEntity.EventOrganisator.Any(o => o.AspNetUsers.UserName == httpContext.User.Identity.Name)
                        || CheckRoles(httpContext))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected bool CheckRoles(HttpContextBase httpContext)
        {
            foreach (var role in _roles)
            {
                if (httpContext.User.IsInRole(role))
                    return true;
            }

            return false;
        }
    }
}