using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using _3F.Log;
using _3F.Model;
using _3F.Model.Extensions;
using _3F.Model.Model;
using _3F.Web.Controllers.API.Model;
using _3F.Web.Extensions;
using _3F.Model.Repositories;

namespace _3F.Web.Controllers.API
{
    [ApiAuthorize]
    public class MessageController : BaseApiController
    {
        private readonly ILogger _logger;
        private readonly IMessageRepository _messageRepository;

        public MessageController(IRepository repository, ILogger logger, IMessageRepository messageRepository)
            : base(repository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
        }

        [HttpGet, Compress]
        public IHttpActionResult List(int page, int pagesize = 20)
        {
            var userId = GetUserId;
            var recipients = repository.Where<MessageRecipient>(mr => mr.Id_User == userId && mr.Visible)
                .OrderByDescending(mr => mr.Message.Time);

            var result = new ApiMessageList("messageListTemplate");

            result.Items = recipients
                .Skip((page - 1)* pagesize)
                .Take(pagesize)
                .ToArray()
                .Select(recipient => new MessageApiModel()
                {
                    Id = recipient.Message.Id,
                    Sender = recipient.Message.AspNetUsers.UserName.TakeSafetely(20),
                    Subject = recipient.Message.Subject,
                    Text = recipient.Message.Text.TakeSafetely(200),
                    Time = recipient.Message.Time.ToDayDateTimeString(),
                    Unreaded = recipient.Unreaded,
                })
                .ToList();

            result.ShowSenderText = true;
            result.Page = page;
            result.TotalItems = recipients.Count();
            result.PageSize = pagesize;
            SetClasses(result);

            return Ok(result);
        }

        [HttpGet, Compress]
        public IHttpActionResult Sended(int page, int pagesize = 20)
        {
            var userId = GetUserId;
            var messages = repository.Where<Message>(m => m.Id_Sender == userId && m.Visible)
                .OrderByDescending(m => m.Time);

            var result = new ApiMessageList("messageListTemplate");

            result.Items = messages
                .Skip((page - 1)*pagesize)
                .Take(pagesize)
                .ToArray()
                .Select(message => new MessageApiModel()
                {
                    Id = message.Id,
                    Sender = string.Join(", ",
                        message.MessageRecipient
                            .Select(m => m.Unreaded ? "<b>" + m.AspNetUsers.UserName + "</b>" : m.AspNetUsers.UserName)),
                    Subject = message.Subject,
                    Time = message.Time.ToDayDateTimeString(),
                    Text = message.Text.TakeSafetely(200),
                    Unreaded = false,
                })
                .ToList();

            result.ShowSenderText = false;
            result.Page = page;
            result.TotalItems = messages.Count();
            result.PageSize = pagesize;
            SetClasses(result);

            return Ok(result);
        }

        [HttpGet, Compress]
        public IHttpActionResult Unreaded(int page, int pagesize = 20)
        {
            var userId = GetUserId;
            var recipients = repository.Where<MessageRecipient>(mr => mr.Id_User == userId && mr.Visible && mr.Unreaded)
                .OrderByDescending(mr => mr.Message.Time);

            var result = new ApiMessageList("messageListTemplate");

            result.Items = recipients
                .Skip((page - 1)*pagesize)
                .Take(pagesize)
                .ToArray()
                .Select(recipient => new MessageApiModel()
                {
                    Id = recipient.Message.Id,
                    Sender = recipient.Message.AspNetUsers.UserName.TakeSafetely(20),
                    Subject = recipient.Message.Subject,
                    Text = recipient.Message.Text.TakeSafetely(200),
                    Time = recipient.Message.Time.ToDayDateTimeString(),
                    Unreaded = recipient.Unreaded,
                })
                .ToList();

            result.ShowSenderText = true;
            result.Page = page;
            result.TotalItems = recipients.Count();
            result.PageSize = pagesize;
            SetClasses(result);

            return Ok(result);
        }

        [HttpGet, Compress]
        public IHttpActionResult UnreadedCount()
        {
            var userId = GetUserId;
            //var result = repository.Where<MessageRecipient>(mr => mr.Id_User == userId && mr.Visible && mr.Unreaded)
            //    .Count();
            var result = _messageRepository.UnreadedMessages(userId);

            return Ok(result);
        }

        [HttpGet, Compress]
        public IHttpActionResult EachOther(string id, int page, int pagesize = 20)
        {
            var userId = GetUserId;
            var other = repository.OneByHtml<AspNetUsers>(id);
            int otherId = other.Id;
            var recipients = repository.Where<MessageRecipient>(
                    mr => mr.Id_User == userId && mr.Visible && mr.Message.Id_Sender == otherId)
                .Union(
                    repository.Where<MessageRecipient>(
                        mr => mr.Id_User == otherId && mr.Message.Id_Sender == userId && mr.Message.Visible))
                .OrderByDescending(mr => mr.Message.Time);

            var result = new ApiMessageList("messageListTemplate");

            result.Items = recipients
                .Skip((page - 1)*pagesize)
                .Take(pagesize)
                .ToArray()
                .Select(recipient => new MessageApiModel()
                {
                    Id = recipient.Message.Id,
                    Sender = recipient.Message.AspNetUsers.UserName.TakeSafetely(20),
                    Subject = recipient.Message.Subject,
                    Text = recipient.Message.Text.TakeSafetely(200),
                    Time = recipient.Message.Time.ToDayDateTimeString(),
                    Unreaded = recipient.Unreaded,
                })
                .ToList();

            result.ShowSenderText = true;
            result.Page = page;
            result.TotalItems = recipients.Count();
            result.PageSize = pagesize;
            SetClasses(result);

            return Ok(result);
        }

        [HttpGet, Compress]
        public IHttpActionResult Search(int page, string q, int pagesize = 20)
        {
            var userId = GetUserId;
            var messages = repository.Where<Message>(
                    m =>
                        (m.Id_Sender == userId && m.Visible) ||
                        m.MessageRecipient.Any(mr => mr.Id_User == userId && mr.Visible))
                .Where(m => m.Subject.Contains(q) || m.Text.Contains(q))
                .OrderByDescending(m => m.Time);

            var result = new ApiMessageList("messageListTemplate");

            result.Items = messages
                .Skip((page - 1)*pagesize)
                .Take(pagesize)
                .ToArray()
                .Select(message => new MessageApiModel()
                {
                    Id = message.Id,
                    Sender =
                        string.Join(", ", message.MessageRecipient.Select(m => m.AspNetUsers.UserName)).TakeSafetely(50),
                    Subject = message.Subject,
                    Time = message.Time.ToDayDateTimeString(),
                    Text = message.Text.TakeSafetely(200),
                    Unreaded = false,
                })
                .ToList();

            result.ShowSenderText = true;
            result.Page = page;
            result.TotalItems = messages.Count();
            result.PageSize = pagesize;
            SetClasses(result);

            return Ok(result);
        }

        private void SetClasses(ApiMessageList model)
        {
            for (int i = 0; i < model.Items.Count; i++)
            {
                model.Items[i].Class = (i % 2 == 0) ? "Even" : "Odd";
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteMessages(int[] ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var message = repository.One<Message>(id);
                    if (message.Id_Sender == GetUserId)
                    {
                        message.Visible = false;
                    }
                    else
                    {
                        var recipient = message.MessageRecipient.FirstOrDefault(r => r.Id_User == GetUserId);
                        if (recipient != null)
                        {
                            recipient.Visible = false;
                        }
                    }
                }

                repository.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Api.Message.DeleteMessages");
                return Ok("Při mazání zpráv nastala chyba");
            }
        }

        [HttpGet, Compress]
        public IHttpActionResult Detail(int id)
        {
            var message = repository.One<Message>(id);
            var userId = GetUserId;
            var recipient = message.MessageRecipient.SingleOrDefault(mr => mr.Id_User == userId);

            if (recipient == null && message.Id_Sender != userId)
                return NotFound();

            var model = GetMessageList(message, userId);

            return Ok(model);
        }

        private List<ApiMessage> GetMessageList(Message message, int userId)
        {
            // všechny zprávy, které odpovídají na stejnou zprávu nebo jsou první zprávou v řetězu nebo jsou zprávou z parametru
            var messages = repository.Where<Message>(m => m.Id == message.Id_ReplyMessage || m.Id_ReplyMessage == message.Id_ReplyMessage || m.Id == message.Id)
                .Where(m => m.Time <= message.Time)
                .Where(m => (m.Id_Sender == userId && m.Visible)
                    || m.MessageRecipient.Any(j => j.Id_User == userId && j.Visible))
                .OrderByDescending(m => m.Time);

            // všechny zprávy v řetězu označit jako přečtené
            var unreadedMessages = messages.Where(m => m.MessageRecipient.Any(mr => mr.Unreaded && mr.Id_User == userId));
            if (unreadedMessages.Any())
            {
                foreach (var unreadedMessage in unreadedMessages)
                {
                    var rec = unreadedMessage.MessageRecipient.Single(m => m.Id_User == userId);
                    rec.Unreaded = false;
                }

                repository.Save();
            }

            var result = messages.Select(m => new ApiMessage()
            {
                Subject = m.Subject,
                Text = m.Text,
                Time = m.Time,
                Sender = new ApiUser()
                {
                    Name = m.AspNetUsers.UserName,
                    HtmlName = m.AspNetUsers.HtmlName,
                    Photo = m.AspNetUsers.ProfilePhoto,
                },
                Recipients = m.MessageRecipient.Select(mr => new ApiUser()
                {
                    Name = mr.AspNetUsers.UserName,
                    HtmlName = mr.AspNetUsers.HtmlName,
                    Photo = mr.AspNetUsers.ProfilePhoto,
                }),
            })
            .ToList();

            return result;
        }
    }
}
