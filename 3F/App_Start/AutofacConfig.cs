using System.Data.Entity;
using _3F.Log;
using _3F.Model;
using _3F.Model.Accounting;
using _3F.Model.Email;
using _3F.Web.Models;
using _3F.Web.Utils;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using _3F.Model.Service;
using _3F.Model.Repositories;
using _3F.Model.Repositories.Interface;
using Autofac.Integration.WebApi;
using Microsoft.Owin.Security;

namespace _3F.Web
{
    public class AutofacConfig
    {
        public static void ConfigureAutofac()
        {
            var builder = new ContainerBuilder();

            // Register your MVC controllers.
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Web API configuration and services
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            builder.RegisterType<Repository>().As<IRepository>().InstancePerLifetimeScope();
            builder.RegisterType<Logger>().As<ILogger>();
            builder.RegisterType<HelpRepository>().As<IHelpRepository>();
            builder.RegisterType<DiscussionRepository>().As<IDiscussionRepository>();
            builder.RegisterType<MessageRepository>().As<IMessageRepository>();
            builder.RegisterType<EventCategoryRepository>().As<IEventCategoryRepository>();
            builder.RegisterType<PostRepository>().As<IPostRepository>();
            builder.RegisterType<FileUploadInfoRepository>().As<IFileUploadInfoRepository>();
            builder.RegisterType<EventRepository>().As<IEventRepository>();
            builder.RegisterType<KnowFromRepository>().As<IKnowFromRepository>();
            builder.RegisterType<OrganizationMembersRepository>().As<IOrganizationMembersRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();

            builder.RegisterFromAppSettings<IAccounting>();
            builder.RegisterFromAppSettings<IEmailSender>();

            builder.RegisterType<LogToEvents>().As<ILogToEvent>();
            builder.RegisterType<ActivityListService>().As<IActivityListService>();
            builder.RegisterType<FileChat>().As<IChat>();
            builder.RegisterType<EventUtils>().As<IEventUtils>();
            builder.RegisterType<MessageService>().As<IMessageService>();
            builder.RegisterType<RajceService>().As<IRajceService>();

            //authentification
            builder.RegisterType<ApplicationDbContext>().As<DbContext>().AsSelf();
            builder.RegisterType<CustomUserStore>().As<IUserStore<ApplicationUser, int>>();
            builder.Register(c => new IdentityFactoryOptions<ApplicationUserManager>()
            {
                DataProtectionProvider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("3F")
            });
            builder.RegisterType<ApplicationUserManager>();

            builder.Register(ctx => HttpContext.Current.GetOwinContext().Authentication).As<IAuthenticationManager>();
            builder.RegisterType<ApplicationSignInManager>();


            var container = builder.Build();

            // Set the dependency resolver for Web API.
            var webApiResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = webApiResolver;

            // Set the dependency resolver for MVC.
            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);
        }
    }
}