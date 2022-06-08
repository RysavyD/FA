using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using _3F.Model;
using _3F.Model.Model;
using _3F.Web.Models;

namespace _3F.Web.Controllers
{
    public class TestController : BaseController
    {
        private readonly IRepository _repository;

        public TestController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: Test
        public ActionResult Index()
        {
            ViewBag.Id = GetUserId;

            var user = _repository.One<AspNetUsers>(95);

            ViewBag.Text = string.Join(", ", user.MainCategories.Select(x => x.MainCategory.ToString()));

            return View();
        }

        public ActionResult Test1()
        {
            return View("~/Views/Account/ConfirmEmail.cshtml", new EmptyBaseViewModel()
            {
                Title = "Registrace dokončena",
                Icon = string.Empty,
                BackgroundColor = "orange-background",
            });
        }

        public ActionResult Test2()
        {
            return View("~/Views/Account/EmailSended.cshtml", new EmailSendedViewModel() {Email = "sqwert@seznam.cz"});
        }

        public ActionResult Error()
        {
            var exception = new ApplicationException("je to někde vidět?");

            logger.LogException(exception, "Test.Error");
            throw exception;
        }
    }
}