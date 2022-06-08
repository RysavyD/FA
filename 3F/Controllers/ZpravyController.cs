using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3F.Model;
using _3F.Model.Model;
using _3F.Model.Extensions;
using _3F.Model.Service;
using _3F.Web.Models;
using _3F.Web.Utils;
// ReSharper disable LocalizableElement

namespace _3F.Web.Controllers
{
    [Authorize]
    public class ZpravyController : BaseController
    {
        readonly IMessageService _messageService;

        public ZpravyController(IMessageService messageService)
        {
            _messageService = messageService;
            BackgroundColor = "grass-green-background";
            Icon = "icon-envelope-alt";
        }

        public ActionResult Index()
        {
            return CreateResult("~/Api/Message/List/", "Přijaté zprávy", true);
        }

        public ActionResult Odeslane()
        {
            return CreateResult("~/Api/Message/Sended/", "Odeslané zprávy", true);
        }

        public ActionResult Neprectene()
        {
            return CreateResult("~/Api/Message/Unreaded/", "Nepřečtené zprávy", true);
        }

        public ActionResult Vzajemne(string id)
        {
            var userName = repository.OneByHtml<AspNetUsers>(id).UserName;

            return CreateResult("~/Api/Message/EachOther/" + id, "Vzájemné zprávy s " + userName);
        }

        private ActionResult CreateResult(string url, string title, bool addDeleteMessageButton = false)
        {
            var model = new MessagesViewModel() { Url = url, Title = title };
            AddButtons(model, addDeleteMessageButton);

            return View("~/Views/Zpravy/Index.cshtml", model);
        }

        public ActionResult Detail(int id)
        {
            var message = repository.One<Message>(id);
            var userId = GetUserId;
            var recipient = message.MessageRecipient.SingleOrDefault(mr => mr.Id_User == userId);

            if (recipient == null && message.Id_Sender != userId)
                return HttpNotFound();

            var result = GetMessageList(message, userId, recipient, true);
            var model = new MessageDetailViewModel
            {
                Entities = result,
                Title = result[0].Subject,
                Id = Convert.ToInt32(result[0].Id),
                ReplyAllAllowed = result[0].Recipients.Any()
            };

            AddButtons(model, false);

            return View(model);
        }

        public ActionResult Vytvorit(string id)
        {
            var model = new ReplyMessageViewModel();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var user = repository.OneByHtml<AspNetUsers>(id);
                if (user != null)
                {
                    model = new ReplyMessageViewModel()
                    {
                        RecipientNames = user.HtmlName,
                        Recipients = new List<Recipient>() {new Recipient(user, true)},
                    };
                }
            }

            AddButtons(model, false);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Vytvorit")]
        public ActionResult VytvoritPost(ReplyMessageViewModel model)
        {
            ValidateMessage(model);

            CheckDailyMessageAmount();

            if (ModelState.IsValid)
            {
                var user = GetUser;
                var entity = new Message()
                {
                    AspNetUsers = user,
                    Id_Sender = user.Id,
                    Subject = model.Subject.TakeSafetely(50),
                    Text = model.Text.ToHtml(),
                    Time = Info.CentralEuropeNow,
                    Visible = true,
                    Id_ReplyMessage = model.ReplyId,
                    MessageRecipient = model.Recipients.Select(r => new MessageRecipient()
                    {
                        Id_User = Convert.ToInt32(r.id),
                        Unreaded = true,
                        Visible = true,
                    }).ToArray(),
                };

                _messageService.SendMessage(entity);

                CreateToastrMessage("Zpráva byla odeslána");

                return RedirectToAction(string.Empty, "Zpravy");
            }

            AddButtons(model, false);
            return View(SolveRecipients(model));
        }

        private void CheckDailyMessageAmount()
        {
            var userId = GetUserId;
            var dailyMessageCount = repository.Where<Message>
                (m => m.Id_Sender == userId && m.Time > Info.CentralEuropeNow.Date)
                .Count();

            int limit = 50;
            if (dailyMessageCount > limit)
            {
                ModelState.AddModelError("",$"Již jste vyčerpali denní limit {limit} zpráv.");
            }
        }

        public ActionResult Odpoved(int id)
        {
            return Odpoved(id, false);
        }

        public ActionResult OdpovedVsem(int id)
        {
            return Odpoved(id, true);
        }
        private ActionResult Odpoved(int id, bool toAll)
        {
            var message = repository.One<Message>(id);
            var userId = GetUserId;
            var recipient = message.MessageRecipient.SingleOrDefault(mr => mr.Id_User == userId);

            if (recipient == null && message.Id_Sender != userId)
                return HttpNotFound();

            var model = new ReplyMessageViewModel()
            {
                ReplyId = message.Id_ReplyMessage.HasValue ? message.Id_ReplyMessage.Value : id,
                Subject = (message.Subject.StartsWith("Re:") ? message.Subject : "Re: " + message.Subject).TakeSafetely(50),
                Unreaded = true,
                Visible = true,
            };

            if (toAll)
            {
                var ids = message.MessageRecipient.Select(mr => mr.Id_User).Where(recId => recId != userId).ToList();
                ids.Add(message.Id_Sender);
                model.RecipientNames = string.Join(",", ids);
            }
            else
                model.RecipientNames = message.Id_Sender.ToString();

            model.RepliesMessages = GetMessageList(message, userId, recipient, false);
            AddButtons(model, false);

            return View("~/Views/Zpravy/Vytvorit.cshtml", SolveRecipients(model));
        }

        [HttpPost]
        public ActionResult Search(string value)
        {
            return CreateResult("~/Api/Message/Search/?q=" + HttpUtility.UrlEncode(value), "Vyhledávání ve zprávách");
        }

        private void ValidateMessage(ReplyMessageViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Subject))
                ModelState.AddModelError("", "Je třeba vyplnit předmět zprávy");

            if (string.IsNullOrWhiteSpace(model.Text))
                ModelState.AddModelError("", "Je třeba vyplnit text zprávy");

            if (string.IsNullOrWhiteSpace(model.RecipientNames))
                ModelState.AddModelError("", "Je třeba vyplnit alespoň jednoho adresáta");

            if (ModelState.IsValid)
            {
                var ids = model.RecipientNames.Split(',').Select(o => Convert.ToInt32(o)).Distinct();
                var userId = GetUserId;
                foreach (var id in ids)
                {
                    if (id != userId)
                    {
                        var user = repository.One<AspNetUsers>(id);
                        model.Recipients.Add(new Recipient(user, true));
                    }
                }

                if (model.Recipients == null || model.Recipients.Count == 0)
                    ModelState.AddModelError("", "Je třeba vyplnit alespoň jednoho adresáta");
            }
        }

        private ReplyMessageViewModel SolveRecipients(ReplyMessageViewModel model)
        {
            if (model.Recipients == null || !model.Recipients.Any())
            {
                if (!string.IsNullOrWhiteSpace(model.RecipientNames))
                {
                    // create recipients from their names
                    var ids = model.RecipientNames.Split(',').Select(o => Convert.ToInt32(o));
                    model.Recipients = ids.Select(id => new Recipient(repository.One<AspNetUsers>(id), true)).ToList();
                }
            }
            else
            {
                // set recipients names
                model.RecipientNames = string.Join(",", model.Recipients.Select(o => o.name));
            }
            return model;
        }

        private List<MessageViewModel> GetMessageList(Message message, int userId, MessageRecipient recipient, bool markAsReaded)
        {
            var model = new MessageViewModel()
            {
                Id = message.Id.ToString(),
                ReplyId = message.Id_ReplyMessage,
                Sender = new User(message.AspNetUsers),
                Subject = message.Subject,
                Text = message.Text,
                Time = message.Time.ToString(),
                Visible = message.Visible,
                Recipients = message.MessageRecipient
                    .Where(mr => mr.Id_User != userId)
                    .Select(mr => new Recipient(mr.AspNetUsers, mr.Unreaded)).ToList(),
            };
            var result = new List<MessageViewModel>() { model };

            if (markAsReaded && recipient != null && recipient.Unreaded)
                recipient.Unreaded = false;

            if (message.Id_ReplyMessage != null)
            {
                var messages = repository.Where<Message>(m => m.Id == message.Id_ReplyMessage || m.Id_ReplyMessage == message.Id_ReplyMessage)
                    .Where(m => m.Time <= message.Time)
                    .Where(m => (m.Id_Sender == userId && m.Visible)
                        || m.MessageRecipient.Any(j => j.Id_User == userId && j.Visible))
                    .OrderByDescending(m => m.Time);

                result = messages.ToArray()
                    .Select(m => new MessageViewModel()
                    {
                        Id = m.Id.ToString(),
                        ReplyId = m.Id_ReplyMessage,
                        Sender = new User(m.AspNetUsers),
                        Subject = m.Subject,
                        Text = m.Text,
                        Time = m.Time.ToString(),
                        Visible = m.Visible,
                        Recipients = m.MessageRecipient
                            .Where(mr => mr.Id_User != userId)
                            .Select(mr => new Recipient(mr.AspNetUsers, mr.Unreaded)).ToList(),
                    }).ToList();

                if (markAsReaded)
                {
                    // všechny zprávy v řetězu označit jako přečtené
                    var unreadedMessages = messages.Where(m => m.MessageRecipient.Any(mr => mr.Unreaded && mr.Id_User == userId));
                    foreach (var unreadedMessage in unreadedMessages)
                    {
                        var rec = unreadedMessage.MessageRecipient.Single(m => m.Id_User == userId);
                        rec.Unreaded = false;
                    }
                }
            }

            if (markAsReaded)
                repository.Save();

            return result;
        }

        private void AddButtons(BaseViewModel model, bool addDeleteMessageButton)
        {
            model.AddActionButton("Nová zpráva", Utilities.Url("~/Zpravy/Vytvorit"), "icon-envelope-alt");
            model.AddActionButton("Přijaté", Utilities.Url("~/Zpravy"), "icon-long-arrow-down");
            model.AddActionButton("Odeslané", Utilities.Url("~/Zpravy/Odeslane"), "icon-long-arrow-up");
            model.AddActionButton("Nepřečtené", Utilities.Url("~/Zpravy/Neprectene"), "icon-envelope");
            if (addDeleteMessageButton)
            {
                model.AddActionButton("Smazat vybrané zprávy", Utilities.Url("~/Zpravy/Vytvorit"), "icon-trash", "",
                    "deleteMessages");
            }
        }
    }
}
