using System.Collections.Generic;
using System.ComponentModel;
using _3F.Model.Extensions;
using _3F.Model.Repositories.Models;
using _3F.Model.Service.Model;
using _3F.Web.Models.Discussion;

namespace _3F.Web.Models
{
    public class MainPageModel
    {
        public EventsType EventType { get; set; }
        public string CategoryName { get; set; }
        public string CategoryHtmlName { get; set; }
        public IEnumerable<ActivityModel> Activities { get; set; }
        public IEnumerable<DiscussionMainPageModel> Discussion { get; set; }

        public string GetDescription
        {
            get
            {
                return string.IsNullOrEmpty(CategoryName)
                    ? EventType.GetDescription()
                    : "Kategorie: " + CategoryName;
            }
        }
    }

    public enum EventsType
    {
        [Description("Vypsané akce")]
        actual,
        [Description("Uplynulé akce")]
        old,
        [Description("Přihlášené akce")]
        login,
        [Description("Organizované akce")]
        organised,
        [Description("Navštívené akce")]
        visited,
        [Description("Smazané akce")]
        deleted,
        [Description("Nerozhodnuté akce")]
        undecided,
        category,
        [Description("Nápady na akce")]
        suggested,
    }

    public class MainPageEventModel : EventListModel
    {
        public string LastDiscussionItem { get; set; }
        public string Status { get; set; }
        public string YesColor { get; set; }
        public string MayBeColor { get; set; }
        public string NoColor { get; set; }
        public bool ShowButtons { get; set; }

        public MainPageEventModel() { }
        public MainPageEventModel(EventListModel eventList)
        {
            Name = eventList.Name;
            Perex = eventList.Perex;
            HtmlName = eventList.HtmlName;
            Photo = eventList.Photo;
            Start = eventList.Start;
            Stop = eventList.Stop;
            IsPaid = eventList.IsPaid;
            IsOfficial = eventList.IsOfficial;
            HasPhoto = eventList.HasPhoto;
            YesParticipants = eventList.YesParticipants;
            MayBeParticipants = eventList.MayBeParticipants;
            Capacity = eventList.Capacity;
            LastDiscussionItemDate = eventList.LastDiscussionItemDate;
            ParticipantStatus = eventList.ParticipantStatus;
            MayBeAllowed = eventList.MayBeAllowed;
            IsPrivated = eventList.IsPrivated;
            IsInWork = eventList.IsInWork;
            IsCommercial = eventList.IsCommercial;
            IsSuggested = eventList.IsSuggested;
        }
    }

    public class ApiResultList<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public string Name { get; set; }
        public string Template { get; set; }
        public bool ShowPagination { get; set; }

        public ApiResultList()
        {
            Items = new List<T>();
            PageSize = 10;
        }

        public ApiResultList(string template)
            : this()
        {
            Template = template;
        }
    }
}