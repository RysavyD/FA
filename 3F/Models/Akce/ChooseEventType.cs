using System.Collections.Generic;

namespace _3F.Web.Models.Akce
{
    public class ChooseEventType
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Descriptions { get; set; }

        public ChooseEventType()
        {
            Descriptions = new List<string>();
        }
    }
}