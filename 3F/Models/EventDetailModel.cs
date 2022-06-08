using _3F.Model.Model;
using System.Collections.Generic;

namespace _3F.Web.Models
{
    public class EventDetailModel : EventModel
    {
        public IEnumerable<Participant> Participants { get; set; }
        public EventLoginEnum UserEventLogin { get; set; }
        public int Availability { get; set; }
        public bool HasSummary { get; set; }
        public bool CanAddSummary { get; set; }
        public bool HasPhoto { get; set; }
        public bool IsInPastOrDeleted { get; set; }
        public EventSummaryBasicViewModel Summary { get; set; }
        public List<PhotoAlbumViewModel> Photos { get; set; }
        public bool ShowAccountSymbol { get; set; }
        public int AccountSymbol { get; set; }
        public bool IsDeleted { get; set; }
        public string GoogleCalendarUrl { get; set; }
        public EventStateEnum State { get; set; }
        public bool IsOrganisator { get; set; }

        public EventDetailModel()
        {
            Participants = new List<Participant>();
            UserEventLogin = EventLoginEnum.Nevyjadreno;
            Photos = new List<PhotoAlbumViewModel>();
        }
    }


}