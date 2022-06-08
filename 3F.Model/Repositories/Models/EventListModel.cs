using System;

namespace _3F.Model.Repositories.Models
{
    public class EventListModel
    {
        public string Name { get; set; }
        public string Perex { get; set; }
        public string HtmlName { get; set; }
        public string Photo { get; set; }
        public string Start { get; set; }
        public string Stop { get; set; }
        public bool IsPaid { get; set; }
        public bool IsOfficial { get; set; }
        public bool HasPhoto { get; set; }
        public int YesParticipants { get; set; }
        public int MayBeParticipants { get; set; }
        public int Capacity { get; set; }
        public bool HasDiscussionItem { get; set; }
        public DateTime? LastDiscussionItemDate { get; set; }
        public int? ParticipantStatus { get; set; }
        public bool MayBeAllowed { get; set; }
        public bool IsPrivated { get; set; }
        public bool IsInWork { get; set; }
        public bool IsCommercial { get; set; }
        public bool IsSuggested { get; set; }
    }
}
