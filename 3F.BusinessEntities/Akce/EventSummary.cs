namespace _3F.BusinessEntities.Akce
{
    public class EventSummary
    {
        public string Name { get; set; }
        public string Perex { get; set; }
        public string Content { get; set; }
        public string HtmlName { get; set; }
        public User Author { get; set; }
    }
}
