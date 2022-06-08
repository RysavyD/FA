using System;
using System.Collections.Generic;

namespace _3F.Web.Controllers.API.Model
{
    public class EventDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Perex { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string Place { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime StopDateTime { get; set; }
        public DateTime? LastSignINDateTime { get; set; }
        public DateTime? LastPaidDateTime { get; set; }
        public DateTime MeetDateTime { get; set; }
        public string MeetPlace { get; set; }
        public string Contact { get; set; }
        public int Price { get; set; }
        public string HtmlName { get; set; }
        public int Id_Discussion { get; set; }
        public string Link { get; set; }
        public bool MayBeLogOn { get; set; }
        public int MinimumParticipants { get; set; }
        public string Photo { get; set; }
        public int AccountSymbol { get; set; }
        public int State { get; set; }
        public IEnumerable<ApiParticipant> Participants { get; set; }
        public IEnumerable<ApiUser> Organisators { get; set; }
    }
}