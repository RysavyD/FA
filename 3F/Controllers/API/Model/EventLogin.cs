using _3F.Model.Model;

namespace _3F.Web.Controllers.API.Model
{
    public class EventUserLog
    {
        public string Status { get; set; }
        public EventLoginEnum StatusEnum { get; set; }
        public string EventName { get; set; }
        public int IdEvent { get; set; }
    }
}