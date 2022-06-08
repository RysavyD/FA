using System.Collections.Generic;
using _3F.BusinessEntities;

namespace _3F.Web.Models.Diskuze
{
    public class DiscussionItemsPartialViewModel
    {
        public IEnumerable<DiscussionItem> Items { get; set; }
        public int IdDiscussion { get; set; }
        public int Page { get; set; }
        public int MaxPage { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
    }
}