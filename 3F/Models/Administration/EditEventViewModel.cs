using _3F.Model.Model;

namespace _3F.Web.Models.Administration
{
    public class EditEventViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EventTypeEnum EventType { get; set; }
        public EventStateEnum State { get; set; }
    }
}