using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using _3F.Model.Repositories;
using _3F.Model.Repositories.Interface;
using _3F.Web.Models;
using _3F.Web.Utils;
using Post = _3F.BusinessEntities.Post;

namespace _3F.Web.Controllers
{
    public class TextController : BaseController
    {
        private readonly ApplicationUserManager _userManager;
        private readonly IPostRepository _postRepository;
        private readonly IOrganizationMembersRepository _organizationMembersRepository;

        public TextController(IPostRepository postRepository, IOrganizationMembersRepository organizationMembersRepository)
        {
            _userManager = DependencyResolver.Current.GetService<ApplicationUserManager>();
            _postRepository = postRepository;
            _organizationMembersRepository = organizationMembersRepository;
        }

        public ActionResult Detail(string id)
        {
            var entity = _postRepository.GetByHtml(id);

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

            return View("~/Views/Text/Detail.cshtml", model);
        }

        [Authorize]
        public ActionResult Edit(string id)
        {
            var entity = _postRepository.GetByHtml(id);

            if (UserHasEditAccess(entity))
            {
                var model = new TextViewModel()
                {
                    Key = entity.HtmlName,
                    Text = entity.Content,
                    Title = entity.Name
                };

                return View(model);
            }

            logger.LogInfo($"Uživatel {User.Identity.Name} se pokouší editovat {id}", "Text.Edit");
            return View("~/Views/Text/NoRights.cshtml", new EmptyBaseViewModel("Nedostatečná práva"));
        }

        [Authorize, ValidateInput(false), ValidateAntiForgeryToken, HttpPost, ActionName("Edit")]
        public ActionResult EditPost(TextViewModel model)
        {
            var entity = _postRepository.GetByHtml(model.Key);
            if (ModelState.IsValid && UserHasEditAccess(entity))
            {
                entity.Name = model.Title;
                entity.Content = model.Text;
                _postRepository.Update(entity);

                logger.LogInfo($"Uživatel {User.Identity.Name} editoval text {model.Key}", "Text.Edit");

                if (string.IsNullOrEmpty(entity.OriginalUrl))
                    return RedirectToAction("Detail", new {id = entity.HtmlName});

                return Redirect(Url.Content(entity.OriginalUrl));
            }

            return View(model);
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
    }
}