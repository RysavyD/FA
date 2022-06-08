using _3F.Log;
using System.Web.Mvc;
using _3F.Web.Models;

namespace _3F.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private ILogger logger;

        public ChatController(ILogger logger)
        {
            this.logger = logger;
        }

        public ActionResult Room(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Room", "Chat", new { id = "Main" });

            ViewBag.Title = "Chat " + id;
            ViewBag.Room = id;

            logger.LogDebug(string.Format("Uživatel {0} navštívil místnost {1}", User.Identity.Name, id), "Chat.Room");

            return View(new ChatModel()
            {
                Title = "Chat " + id,
                Room = id,
            });
        }
    }
}