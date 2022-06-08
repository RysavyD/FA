using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;
using _3F.Model;
using _3F.Web.Utils;

namespace _3F.Web.Extensions
{
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var values = actionContext.Request.Headers.GetCookies(".ApiLoginCookie").FirstOrDefault();
            if (values != null)
            {
                var authCookie = values.Cookies.FirstOrDefault(c => c.Name == ".ApiLoginCookie");
                if (authCookie != null)
                {
                    var cookieValue = Utilities.Decrypt(authCookie.Value).Split('|');

                    var user = cookieValue[0];

                    // If caching roles in userData field then extract
                    string[] roles = cookieValue[1].Split(',');

                    // Create the IIdentity instance for one year
                    IIdentity identity = new FormsIdentity(new FormsAuthenticationTicket(user, true, 60 * 24 * 365));

                    // Create the IPrinciple instance
                    IPrincipal principal = new GenericPrincipal(identity, roles);

                    // Set the context user 
                    actionContext.RequestContext.Principal = principal;
                    return true;
                }
            }

            return base.IsAuthorized(actionContext);
        }
    }

    public class IdentityAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var values = actionContext.Request.Headers.GetCookies(".ApiLoginCookie").FirstOrDefault();
            if (values != null)
            {
                var authCookie = values.Cookies.FirstOrDefault(c => c.Name == ".ApiLoginCookie");
                if (authCookie != null)
                {
                    var cookieValue = Utilities.Decrypt(authCookie.Value).Split('|');

                    var user = cookieValue[0];

                    // If caching roles in userData field then extract
                    string[] roles = cookieValue[1].Split(',');

                    // Create the IIdentity instance for one year
                    IIdentity identity = new FormsIdentity(new FormsAuthenticationTicket(user, true, 60 * 24 * 365));

                    // Create the IPrinciple instance
                    IPrincipal principal = new GenericPrincipal(identity, roles);

                    // Set the context user 
                    actionContext.RequestContext.Principal = principal;
                }
            }

            return true;
        }
    }
}