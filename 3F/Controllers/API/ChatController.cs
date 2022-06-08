using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using _3F.Web.Utils;

namespace _3F.Web.Controllers.API
{
    [Authorize]
    public class ChatController : ApiController
    {
        private IChat chat;

        public ChatController(IChat chat)
        {
            this.chat = chat;
        }

        [HttpGet]
        public IHttpActionResult Rooms()
        {
            return Ok(chat.GetRooms().OrderBy(r => r));
        }

        [HttpGet]
        public IHttpActionResult Users(string room)
        {
            return Ok(chat.GetOnlineUsers(room).OrderBy(u => u.name));
        }

        [HttpPost]
        public IHttpActionResult Messages(string room, [FromBody]LastMessage lastMessage)
        {
            var messages = chat.GetMessages(room).OrderByDescending(m => m.DateTime);
            bool needToClear = messages.Last().DateTime > lastMessage.LastDateTime && !lastMessage.IsFirst;
            if (!needToClear && !lastMessage.IsFirst)
            {
                messages = messages
                    .Where(m => m.DateTime > lastMessage.LastDateTime)
                    .OrderByDescending(m => m.DateTime);
            }

            var items = messages
                    .Select(m => new ApiChatMessage()
                        {
                            Color = m.Color,
                            DateTimeStr = m.DateTime.ToString("ddd HH:mm:ss"),
                            DateTime = m.DateTime,
                            Text = m.Text,
                            UserName = m.UserName,
                        }
                    );

            return Ok(new ApiResponse()
            {
                NeedToClear = needToClear,
                LastTime = items?.FirstOrDefault()?.DateTime,
                Items = items
            });
        }

        [HttpPost]
        public string DeleteRoom([FromBody]string room)
        {
            chat.DeleteRoom(room);
            return string.Empty;
        }

        [HttpPost]
        public string CreateRoom([FromBody]string room)
        {
            chat.CreateRoom(room);
            return string.Empty;
        }

        [HttpPost]
        public string AddMessage([FromBody]string text)
        {
            var roomName = Request.Headers.Referrer.Segments.Last();
            chat.AddMessage(text, roomName);
            return string.Empty;
        }
    }

    class ApiResponse
    {
        public bool NeedToClear { get; set; }
        public DateTime? LastTime { get; set; }
        public IEnumerable<ApiChatMessage> Items { get; set; }
    }

    class ApiChatMessage
    {
        public string UserName { get; set; }
        public string DateTimeStr { get; set; }
        public DateTime DateTime { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
    }

    public class LastMessage
    {
        public DateTime LastDateTime { get; set; }
        public bool IsFirst { get; set; }
    }
}
