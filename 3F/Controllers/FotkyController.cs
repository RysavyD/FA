using System.Linq;
using System.Web.Mvc;
using _3F.Model.Model;
using _3F.Model.Service;
using _3F.Web.Extensions;
using _3F.Web.Utils;

namespace _3F.Web.Controllers
{
    public class FotkyController : BaseController
    {
        private readonly IRajceService _rajceService;

        public FotkyController(IRajceService rajceService)
        {
            _rajceService = rajceService;

            BackgroundColor = "purple-background";
            Icon = "icon-picture";
        }

        public ActionResult Index()
        {
            var albums = repository.All<PhotoAlbum>()
                .OrderByDescending(pa => pa.Event.StartDateTime)
                .AsEnumerable()
                .Select(pa => pa.ToViewModel());

            return View(albums);
        }
        
        public ActionResult Album(int id)
        {
            var album = repository.One<PhotoAlbum>(id);
            if (album != null)
            {
                var model = album.ToPhotoViewModel(_rajceService);
                if (User.Identity.IsAuthenticated && !string.IsNullOrWhiteSpace(User.Identity.Name))
                    logger.LogInfo(string.Format("Uživatel {0} navštívil fotoalbum {1} od {2}", User.Identity.Name, model.EventName, model.User.name), "Fotky.Detail");

                model.AddActionButton("Přejít na akci", Utilities.Url("~/Akce/Detail/", model.EventHtml, HttpContext), "icon-star");
                return View(model);
            }

            return RedirectToAction("Index", "Fotky");
        }
    }
}