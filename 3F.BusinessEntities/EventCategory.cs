using _3F.BusinessEntities.Enum;

namespace _3F.BusinessEntities
{
    public class EventCategory : AbstractEntity
    {
        public string Name { get; set; }

        public string HtmlName { get; set; }

        public MainCategory MainCategory { get; set; }

        public int EventCount { get; set; }
    }
}
