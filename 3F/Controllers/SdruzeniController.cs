using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using _3F.Model.Model;
using _3F.Web.Definitions;
using _3F.Web.Extensions;
using _3F.Web.Models;
using _3F.Web.Utils;

namespace _3F.Web.Controllers
{
    public class SdruzeniController : BaseController
    {
        private readonly ApplicationUserManager _userManager;

        public SdruzeniController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public ActionResult Index(string id)
        {
            var text = repository.One<Post>(t => t.HtmlName == id);

            var model = new OrganisationViewModel
            {
                Text = text.Content,
                Title = text.Name,
                Icon = text.Icon,
                Chief = repository.One<AspNetRoles>(r => r.Name == Strings.Chief).AspNetUsers.First()
                    .ToWebUser(),
                Council = repository.One<AspNetRoles>(r => r.Name == Strings.Council).AspNetUsers
                    .OrderBy(u => u.UserName)
                    .Select(u => new User(u)),
                Supervisors = repository.One<AspNetRoles>(r => r.Name == Strings.Supervisor).AspNetUsers
                    .OrderBy(u => u.UserName)
                    .Select(u => new User(u)),
                CertifiedOrganisators = repository.One<AspNetRoles>(r => r.Name == Strings.CertifiedOrganisator).AspNetUsers
                    .OrderBy(u => u.UserName)
                    .Select(u => new User(u)),
            };

            var organisationId = repository.One<Organisation>(o => o.HtmlName == id).Id;
            model.Members = repository.Where<OrganisationMember>(
                    o => o.Id_Organisation == organisationId && o.To == null)
                .OrderBy(o => o.From)
                .Select(o => o.AspNetUsers)
                .ToArray()
                .Select(u => u.ToWebUser());


            if (UserHasEditAccess(text))
            {
                model.AddActionButton("Editovat text", Utilities.Url("~/Text/Edit/" + id), "icon-edit");
                model.AddActionButton("Administrace sdružení", Utilities.Url("~/Administrace"), "icon-cogs");
            }


            logger.LogInfo(string.Format("Uživatel {0} navštívil stránku sdružení", User.Identity.Name), "Sdruzeni.Index");

            return View(model);
        }

        private bool UserHasEditAccess(Post entity)
        {
            if (!User.Identity.IsAuthenticated)
                return false;

            var permissions = entity.EditPermissions.Split(',');
            var roles = _userManager.GetRoles(GetUserId);

            return (permissions.Intersect(roles).Any() || permissions.Contains(User.Identity.Name));
        }
    }
}