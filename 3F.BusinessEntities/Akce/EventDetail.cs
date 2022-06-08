using System;
using System.Collections.Generic;

namespace _3F.BusinessEntities.Akce
{
    public class EventDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Perex { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string Place { get; set; }
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }
        public DateTime? LastSignIn { get; set; }
        public DateTime MeetTime { get; set; }
        public string MeetPlace { get; set; }
        public string Contact { get; set; }
        public int Price { get; set; }
        public string HtmlName { get; set; }
        public int IdDiscussion { get; set; }
        public string Link { get; set; }
        public bool MayBeLogOn { get; set; }
        public DateTime? LastPaid { get; set; }
        public int MinimumParticipants { get; set; }
        public string Photo { get; set; }
        public int AccountSymbol { get; set; }
        public int State { get; set; }
        public int Costs { get; set; }
        public int EventType { get; set; }
        public string CostsDescription { get; set; }
        public IEnumerable<User> Organisators { get; set; }
        public IEnumerable<Participant> Participants { get; set; }
        public EventSummary Summary { get; set; }
        public IEnumerable<PhotoAlbum> PhotoAlbums { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
