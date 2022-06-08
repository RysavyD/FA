using System;

namespace _3F.Web.Controllers.API.Model
{
    public class ApiDiscussion
    {
        public ApiUser Author { get; set; }
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public string Perex { get; set; }
        public int ItemsCount { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class ApiDiscussionItem
    {
        public ApiUser Author { get; set; }
        public DateTime DateTime { get; set; }
        public string Text { get; set; }
    }
}