using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using _3F.Log;
using _3F.Model;
using _3F.Model.Model;
using _3F.Model.Repositories;
using _3F.Model.Repositories.Interface;
using _3F.Web.Models;

namespace _3F.Web.Controllers
{
    public class BaseController : Controller
    {
        protected IRepository repository;
        protected ILogger logger;
        protected IUserRepository userRepository;
        protected string BackgroundColor { get; set; }
        protected string Icon { get; set; }

        public BaseController()
        {
            repository = DependencyResolver.Current.GetService<IRepository>();
            logger = DependencyResolver.Current.GetService<ILogger>();
            userRepository = DependencyResolver.Current.GetService<IUserRepository>();
            BackgroundColor = "green-background";
            Icon = "icon-star";
        }

        private AspNetUsers _user;
        protected AspNetUsers GetUser 
        {
            get
            {
                if (_user == null)
                    _user = repository.One<AspNetUsers>(u => u.UserName == User.Identity.Name);

                return _user;
            }
        }

        private AspNetUsers _systemUser;
        protected AspNetUsers GetSystemUser
        {
            get
            {
                if (_systemUser == null)
                    _systemUser = repository.One<AspNetUsers>(u => u.UserName == Definitions.Strings.System);

                return _systemUser;
            }
        }

        protected int GetUserId
        {
            get
            {
                return User.Identity.IsAuthenticated
                    ? Convert.ToInt32(((ClaimsIdentity) User.Identity).FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value)
                    : 0;

            }
        }

        protected string GetUserHtmlName
        {
            get
            {
                return User.Identity.IsAuthenticated
                    ? ((ClaimsIdentity)User.Identity).FindFirst("HtmlName").Value
                    : "";

            }
        }

        protected void CreateToastrMessage(string text)
        {
            TempData["Toastr"] = text;
        }

        protected ActionButton CreateActionButton(string title, string url, string icon)
        {
            return new ActionButton(title, UrlHelper.GenerateContentUrl(url, HttpContext), icon);
        }

        protected ActionResult RedirectToHome()
        {
            return Redirect(Url.Content("~/"));
        }

        protected ActionResult TextDetail(string key)
        {
            var controller = new TextController
            (
                DependencyResolver.Current.GetService<IPostRepository>(),
                DependencyResolver.Current.GetService<IOrganizationMembersRepository>())
            {
                ControllerContext = ControllerContext
            };
            return controller.Detail(key);
        }

        protected void LogValidationErrors(Exception ex, string actionName)
        {
            if (ex is DbEntityValidationException exception)
            {
                foreach (var validationError in exception.EntityValidationErrors.SelectMany(e => e.ValidationErrors))
                {
                    logger.LogError(
                        $"'{validationError.ErrorMessage}' on property '{validationError.PropertyName}'", actionName);
                }
            }
        }

        #region overriding
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                repository.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                userRepository.UpdateUserActivity(GetUserId);
            }
            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            logger.LogException(filterContext.Exception, filterContext.Controller.ToString());
            base.OnException(filterContext);
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var viewModel = model as BaseViewModel;
            if (viewModel != null)
            {
                viewModel.BackgroundColor = string.IsNullOrEmpty(viewModel.BackgroundColor)
                    ? BackgroundColor
                    : viewModel.BackgroundColor;
                viewModel.Icon = string.IsNullOrEmpty(viewModel.Icon) ? Icon : viewModel.Icon;
            }

            string path = System.IO.Path.Combine(Values.Instance.AppDataPath, "news.txt");
            if (System.IO.File.Exists(path))
            {
                var news = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
                if (!string.IsNullOrEmpty(news))
                    ViewBag.News = news;

            }

            return base.View(viewName, masterName, model);
        }
        #endregion
    }
}