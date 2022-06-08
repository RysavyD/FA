namespace _3F.BusinessEntities
{
    public class Post: AbstractEntity
    {
        public string Name { get; set; }

        public string HtmlName { get; set; }

        public string Content { get; set; }
        public string EditPermissions { get; set; }
        public string ViewPermissions { get; set; }
        public string OriginalUrl { get; set; }
        public string Icon { get; set; }
    }
}
