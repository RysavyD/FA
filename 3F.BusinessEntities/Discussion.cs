using System;

namespace _3F.BusinessEntities
{
    public class Discussion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public string Perex { get; set; }
        public User Author { get; set; }
        public int ItemsCount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
