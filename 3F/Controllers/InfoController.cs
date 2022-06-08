using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using _3F.Model.Model;
using _3F.Web.Models;
using _3F.Web.Utils;
using _3F.Web.Definitions;
using _3F.Web.Extensions;

namespace _3F.Web.Controllers
{
    public class InfoController : BaseController
    {
        private readonly ApplicationUserManager _userManager;

        public InfoController(ApplicationUserManager userManager)
        {
            Icon = "";
            BackgroundColor = "green-background";
            _userManager = userManager;
        }

        public ActionResult Index()
        {
            var spolek = repository.One<Post>(t => t.HtmlName == "FA-o-nas");
            var organisationName = "Spolecne-aktivity-zs";

            var model = new InfoViewModel()
            {
                Title = "O nás",
                Spolek = spolek.Content,
                Year2013 = repository.One<Post>(t => t.HtmlName == "FA-2013").Content,
                Year2014 = repository.One<Post>(t => t.HtmlName == "FA-2014").Content,
                Year2015 = repository.One<Post>(t => t.HtmlName == "FA-2015").Content,
                Year2016 = repository.One<Post>(t => t.HtmlName == "FA-2016").Content,
                Authors = repository.One<Post>(t => t.HtmlName == "FA-Authors").Content,
            };

            var text = repository.One<Post>(t => t.HtmlName == organisationName);
            var orgModel = new OrganisationViewModel
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

            var organisationId = repository.One<Organisation>(o => o.HtmlName == organisationName).Id;
            orgModel.Members = repository.Where<OrganisationMember>(
                    o => o.Id_Organisation == organisationId && o.To == null)
                .OrderBy(o => o.From)
                .Select(o => o.AspNetUsers)
                .ToArray()
                .Select(u => u.ToWebUser());

            model.Organisation = orgModel;

            if (UserHasEditAccess(spolek))
            {
                model.AddActionButton("Editovat texty", Utilities.Url("~/Administrace/Texty"), "icon-edit");
                model.AddActionButton("Administrace sdružení", Utilities.Url("~/Administrace"), "icon-cogs");
            }

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

        public ActionResult Podminky()
        {
            return View(new EmptyBaseViewModel() { Title = "Podmínky užití" });
        }
    }
}