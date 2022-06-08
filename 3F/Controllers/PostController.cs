using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using _3F.BusinessEntities;
using _3F.Model.Repositories;
using _3F.Model.Repositories.Interface;
using _3F.Web.Models;
using _3F.Web.Models.Administration;
using _3F.Web.Utils;

namespace _3F.Web.Controllers
{
    public class PostController : BaseController
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IPostRepository _postRepository;
        private readonly IOrganizationMembersRepository _organizationMembersRepository;

        public PostController(IPostRepository postRepository, IOrganizationMembersRepository organizationMembersRepository)
        {
            _userManager = DependencyResolver.Current.GetService<ApplicationUserManager>();
            _postRepository = postRepository;
            _organizationMembersRepository = organizationMembersRepository;
        }

        [Route("Post/{id}")]
        public ActionResult Index(string id)
        {
            int numId;
            var entity = (int.TryParse(id, out numId)) ? _postRepository.GetById(numId) : _postRepository.GetByHtml(id);

            if (!UserHasViewAccess(entity))
            {
                return View("~/Views/Text/NoRights.cshtml", new EmptyBaseViewModel("Nedostatečná práva"));
            }

            var model = new TextViewModel()
            {
                Key = entity.HtmlName,
                Text = entity.Content,
                Title = entity.Name,
                Icon = entity.Icon,
            };

            if (UserHasEditAccess(entity))
            {
                model.AddActionButton("Editovat", Utilities.Url("~/Text/Edit/" + entity.HtmlName), "icon-edit");
            }

            var viewModel = new GenericBaseViewModel<PostViewModel>(new PostViewModel(entity))
            {
                Title = entity.Name,
                BackgroundColor = "brown-background",
            };

            return View(viewModel);
        }

        private bool UserHasViewAccess(Post entity)
        {
            if (entity == null)
                return false;

            if (string.IsNullOrEmpty(entity.ViewPermissions))
                return true;

            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }

            var permissions = entity.ViewPermissions.Split(',');

            // hvězdička -> přístupné pro všechny přihlášené uživatele
            if (permissions.Contains("*"))
                return true;

            // procento -> přístupné pouze členům
            if (permissions.Contains("%"))
            {
                if (_organizationMembersRepository.IsMember(GetUserId))
                    return true;
            }

            // není hvězdička -> zkontrolovat role a jména
            var roles = _userManager.GetRoles(GetUserId);

            return (permissions.Intersect(roles).Any() || permissions.Contains(User.Identity.Name));
        }

        private bool UserHasEditAccess(Post entity)
        {
            if (!User.Identity.IsAuthenticated)
                return false;

            var permissions = entity.EditPermissions.Split(',');
            var roles = _userManager.GetRoles(GetUserId);

            if (permissions.Contains("%") && _organizationMembersRepository.IsMember(GetUserId))
                return true;

            return (permissions.Intersect(roles).Any() || permissions.Contains(User.Identity.Name));
        }
    }
}