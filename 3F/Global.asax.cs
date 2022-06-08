using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin.Security;
using _3F.Log;
using _3F.Model;

namespace _3F.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            var logger = new Logger();
            logger.LogInfo("Application.Start", "Application.Start");

            // Code that runs on application startup
            //AreaRegistration.RegisterAllAreas();
            AutofacConfig.ConfigureAutofac();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            FileConfig.CheckDirectory();
            JobScheduler.Start();

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.UseXmlSerializer = true;

            Info.ConnectionString = ConfigurationManager.ConnectionStrings["DapperConnection"].ConnectionString;
        }

        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (User != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var claimsIdentity = (ClaimsIdentity) User.Identity;
                    if (claimsIdentity.Claims.All(x => x.Type != "VOP"))
                    {
                        var authManager = DependencyResolver.Current.GetService<IAuthenticationManager>();
                        authManager.SignOut();
                        return;
                    }

                    var symbol = Convert.ToInt32(claimsIdentity.FindFirst("VOP").Value);
                    var availableSites = new[] {"/test", "/account/logoff", "/profil/vop"};
                    if (symbol < Settings.MinVopVersion && !availableSites.Contains(Request.Url.LocalPath.ToLower()))
                    {
                        Response.Redirect("~/profil/vop");
                    }
                }
            }
        }
    }
}