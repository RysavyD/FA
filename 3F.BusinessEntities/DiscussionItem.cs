using System;

namespace _3F.BusinessEntities
{
    public class DiscussionItem
    {
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public User Author { get; set; }
    }
}
