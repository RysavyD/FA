using System;
using System.Linq;
using _3F.BusinessEntities.Akce;
using _3F.Model;
using _3F.Model.Model;

namespace _3F.Web.Models.Akce
{
    public class EventDetailViewModel : EventDetail
    {
        public bool ShowAccountSymbol { get; set; }
        public string GoogleCalendarUrl { get; set; }

        public int Availability => Capacity == 0
            ? 0
            : Participants.Count(p => p.LoginStatus == (int)EventLoginEnum.Nahradnik ||
                                      p.LoginStatus == (int)EventLoginEnum.NepotvrzenaRezervace
                                      || p.LoginStatus == (int)EventLoginEnum.Prijdu ||
                                      p.LoginStatus == (int)EventLoginEnum.Rezervace) * 100 / Capacity;

        public bool IsInPastOrDeleted => Stop < Info.CentralEuropeNow || State == (int) EventStateEnum.Deleted;
        public bool IsDeleted => State == (int)EventStateEnum.Deleted;
        public bool IsPrivate => EventType == (int) EventTypeEnum.Soukroma;
        public bool HasSummary => Summary != null;
        public bool HasPhoto => PhotoAlbums.Any();
        public string CapacityString => Capacity == 0 ? "Bez omezení" : Capacity.ToString();
        public string PriceString => Price == 0 ? "Akce je zdarma" : $"{Price} Kč";
        public bool IsOrganisator { get; set; }
        public bool CanAddSummary { get; set; }
        public int UserEventLogin { get; set; }

        public static EventDetailViewModel FromModel(EventDetail model)
        {
            return new EventDetailViewModel()
            {
                Id = model.Id,
                Name = model.Name,
                Perex = model.Perex,
                Description = model.Description,
                Capacity = model.Capacity,
                Place = model.Place,
                Start = model.Start,
                Stop = model.Stop,
                LastSignIn = model.LastSignIn,
                MeetTime = model.MeetTime,
                MeetPlace = model.MeetPlace,
                Contact = model.Contact,
                Price = model.Price,
                HtmlName = model.HtmlName,
                IdDiscussion = model.IdDiscussion,
                Link = GetLink(model.Link),
                MayBeLogOn = model.MayBeLogOn,
                LastPaid = model.LastPaid,
                MinimumParticipants = model.MinimumParticipants,
                Photo = model.Photo,
                AccountSymbol = model.AccountSymbol,
                State = model.State,
                Costs = model.Costs,
                EventType = model.EventType,
                CostsDescription = model.CostsDescription,
                Organisators = model.Organisators,
                Participants = model.Participants,
                Summary = model.Summary,
                PhotoAlbums = model.PhotoAlbums,
                Categories = model.Categories,
            };
        }

        private static string GetLink(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return string.Empty;

            try
            {
                return link.StartsWith("http")
                    ? $"<a href=\"{link}\">{new Uri(link).Host}</a>"
                    : $"<a href=\"{"https://" + link}\">{new Uri("https://" + link).Host}</a>";
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}