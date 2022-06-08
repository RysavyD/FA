using System;
using System.Collections.Generic;
using _3F.Web.Models;

namespace _3F.Web.Controllers.API.Model
{
    public class MessageApiModel
    {
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
        public int Id { get; set; }
        public bool Unreaded { get; set; }
        public string Class { get; set; }
    }

    public class ApiMessageList : ApiResultList<MessageApiModel>
    {
        public bool ShowSenderText { get; set; }

        public ApiMessageList(string template) : base(template)
        {
            
        }
    }

    public class ApiMessage
    {
        public ApiUser Sender { get; set; }
        public IEnumerable<ApiUser> Recipients { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
    }
}