using System;
using System.Security.Claims;
using System.Web.Http;
using _3F.Model;
using _3F.Model.Model;
using _3F.Model.Repositories;

namespace _3F.Web.Controllers.API
{
    public class BaseApiController : ApiController
    {
        protected IRepository repository;

        public BaseApiController(IRepository repository)
        {
            this.repository = repository;
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

        protected int GetUserId
        {
            get
            {
                //return (User.Identity.IsAuthenticated && User.Identity is System.Web.Security.FormsIdentity)
                //    ? GetUser.Id
                //    : 0;
                return User.Identity.IsAuthenticated
                    ? Convert.ToInt32(((ClaimsIdentity)User.Identity).FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value)
                    : 0;

            }
        }

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            if (User.Identity.IsAuthenticated)
            {
                //GetUser.DateLastActivity = Info.CentralEuropeNow;
                //repository.Save();
                new UserRepository().UpdateUserActivity(GetUserId);
            }
        }
    }
}
