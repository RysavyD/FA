using System;
using System.Collections.Generic;
using System.Linq;
using _3F.Log;
using _3F.Model;
using _3F.Model.Model;
using _3F.Model.Extensions;
using _3F.Model.Service;
using _3F.Model.Utils;
using _3F.Web.Models;
using _3F.Web.Models.Discussion;
using _3F.Web.Models.EventModels;

namespace _3F.Web.Extensions
{
    public static class ModelExtensions
    {
        public static IEnumerable<MainPageEventModel> ToMainPageViewModel(this IEnumerable<Event> source, int? id_User)
        {
            var result = new List<MainPageEventModel>();

            foreach(var dbEvent in source)
            {
                result.Add(dbEvent.ToMainPageViewModel(id_User));
            }

            return result;
        }

        public static MainPageEventModel ToMainPageViewModel(this Event entity, int? id_User)
        {
            var part = (id_User.HasValue) ? entity.EventParticipant.FirstOrDefault(p => p.Id_User == id_User && !p.IsExternal) : null;
            string status = (part != null) ? part.EventLoginStatus.GetDescription() : (id_User.HasValue) ? EventLoginEnum.Nevyjadreno.GetDescription() : string.Empty;

            return new MainPageEventModel()
            {
                Name = entity.Name,
                Perex = entity.Perex,
                Start = entity.StartDateTime.ToDayDateTimeString(),
                Stop = entity.StopDateTime.ToDayDateTimeString(),
                HtmlName = entity.HtmlName,
                IsOfficial = (entity.EventType == EventTypeEnum.PlacenaSdruzenim || entity.EventType == EventTypeEnum.OficialniSdruzeni),
                IsPaid = (entity.EventType == EventTypeEnum.PlacenaSdruzenim),
                Photo = entity.Photo,
                HasPhoto = !string.IsNullOrWhiteSpace(entity.Photo),
                Capacity = entity.Capacity,
                MayBeParticipants = entity.EventParticipant.Count(p => p.EventLoginStatus == EventLoginEnum.Mozna),
                YesParticipants = entity.EventParticipant.Count(p => p.EventLoginStatus == EventLoginEnum.Nahradnik || p.EventLoginStatus == EventLoginEnum.NepotvrzenaRezervace
                    || p.EventLoginStatus == EventLoginEnum.Prijdu || p.EventLoginStatus == EventLoginEnum.Rezervace),
                HasDiscussionItem = (entity.Discussion.DiscussionItem.Count > 0),
                LastDiscussionItem = (entity.Discussion.DiscussionItem.Count > 0) ? entity.Discussion.DiscussionItem.OrderByDescending(di => di.DateTime).First().DateTime.ToDayDateTimeString() : string.Empty,
                Status = status,
                MayBeAllowed = entity.MayBeLogOn,
                YesColor = (part != null && part.EventLoginStatus == EventLoginEnum.Prijdu) ? "#49bf67" : "#808080",
                MayBeColor = (part != null && part.EventLoginStatus == EventLoginEnum.Mozna) ? "#e48a07" : "#808080",
                NoColor = (part != null && part.EventLoginStatus == EventLoginEnum.Neprijdu) ? "#f34541" : "#808080",
                ShowButtons = (id_User.HasValue && entity.StopDateTime > Info.CentralEuropeNow),
                IsPrivated = entity.EventType == EventTypeEnum.Soukroma,
                IsInWork = entity.State == EventStateEnum.InWork,
                IsCommercial = entity.EventType == EventTypeEnum.Komercni,
                IsSuggested = entity.EventType == EventTypeEnum.TipNaAkci,
            };
        }

        public static SimpleEventModel ToSimpleViewModel(this Event entity)
        {
            return new SimpleEventModel()
            {
                Name = entity.Name,
                HtmlName = entity.HtmlName,
                Perex = entity.Perex,
                Start = entity.StartDateTime,
                Stop = entity.StopDateTime,
            };
        }

        public static IEnumerable<SimpleEventModel> ToSimpleViewModel(this IEnumerable<Event> entities)
        {
            foreach (var entity in entities)
                yield return entity.ToSimpleViewModel();
        }

        public static Event ToEntity(this EventModel model)
        {
            return new Event()
            {
                Name = model.Name,
                Perex = model.Perex,
                Description = model.Description,
                Capacity = model.Capacity,
                Place = model.Place,
                StartDateTime = model.Start,
                StopDateTime = model.Stop,
                MeetDateTime = model.MeetTime,
                MeetPlace = model.MeetPlace,
                Contact = model.Contact,
                Price = model.Price,
                Link = model.Link,
                MayBeLogOn = model.MayBeAvalaible,
                Photo = model.Photo,
                EventType = model.EventType,
                LastPaidDateTime = model.LastPaidTime,
                LastSignINDateTime = model.LastSignTime,
                MinimumParticipants = model.MinimumParticipants,
                Costs = model.Costs,
                CostsDescription = model.CostsDescription,
                MainCategory = model.MainCategory,
                IsStay = model.IsStay
            };
        }

        public static EventModel ToViewModel(this Event entity)
        {
            return new EventModel()
            {
                Name = entity.Name,
                Capacity = entity.Capacity,
                Contact = entity.Contact,
                Description = entity.Description,
                EventType = entity.EventType,
                HtmlName = entity.HtmlName,
                Id_Discussion = entity.Id_Discussion,
                LastPaidTime = entity.LastPaidDateTime,
                Link = entity.Link,
                MayBeAvalaible = entity.MayBeLogOn,
                MeetPlace = entity.MeetPlace,
                MeetTime = entity.MeetDateTime,
                Photo = entity.Photo,
                Place = entity.Place,
                Perex = entity.Perex,
                Price = entity.Price,
                Start = entity.StartDateTime,
                Stop = entity.StopDateTime,
                LastSignTime = entity.LastSignINDateTime,
                Organisators = entity.EventOrganisator.Select(o => new User(o.AspNetUsers)).ToArray(),
                MinimumParticipants = entity.MinimumParticipants,
                Costs = entity.Costs,
                CostsDescription = entity.CostsDescription,
                EventEditMode = entity.State == EventStateEnum.Active ? EventEditMode.Edit : EventEditMode.Create,
                CategoryIds = entity.EventCategories.Select(cat => cat.Id).ToArray(),
                Image = entity.Photo,
                MainCategory = entity.MainCategory,
                IsStay = entity.IsStay,
            };
        }

        public static EventDetailModel ToDetailViewModel(this Event entity, AspNetUsers user)
        {
            var participant = entity.EventParticipant.FirstOrDefault(ep => ep.AspNetUsers == user && !ep.IsExternal);
            EventLoginEnum userStatus = (participant == null) ? EventLoginEnum.Nevyjadreno : participant.EventLoginStatus;

            // Rezervace is as login to event
            if (participant != null && participant.EventLoginStatus == EventLoginEnum.Rezervace)
                userStatus = EventLoginEnum.Prijdu;

            return new EventDetailModel()
            {
                Name = entity.Name,
                Capacity = entity.Capacity,
                Contact = entity.Contact,
                Description = entity.Description,
                EventType = entity.EventType,
                HtmlName = entity.HtmlName,
                Id_Discussion = entity.Id_Discussion,
                LastPaidTime = entity.LastPaidDateTime,
                Link = string.IsNullOrWhiteSpace(entity.Link) ? string.Empty : GetLink(entity.Link),
                MayBeAvalaible = entity.MayBeLogOn,
                MeetPlace = entity.MeetPlace,
                MeetTime = entity.MeetDateTime.Value,
                LastSignTime = entity.LastSignINDateTime,
                Photo = entity.Photo,
                Place = entity.Place,
                Perex = entity.Perex,
                Price = entity.Price,
                Start = entity.StartDateTime,
                Stop = entity.StopDateTime,
                Organisators = entity.EventOrganisator.Select(o => new User(o.AspNetUsers)).ToArray(),
                Participants = entity.EventParticipant.Select(p => new Participant(p)).ToArray(),
                UserEventLogin = userStatus,
                Availability = (entity.Capacity == 0) ? 0 :
                    entity.EventParticipant.Count(p => p.EventLoginStatus == EventLoginEnum.Nahradnik || p.EventLoginStatus == EventLoginEnum.NepotvrzenaRezervace
                                   || p.EventLoginStatus == EventLoginEnum.Prijdu || p.EventLoginStatus == EventLoginEnum.Rezervace) * 100 / entity.Capacity,
                HasSummary = entity.EventSummary.Any(),
                HasPhoto = entity.PhotoAlbum.Any(),
                IsInPastOrDeleted = (entity.StopDateTime < Info.CentralEuropeNow) || entity.State == EventStateEnum.Deleted,
                MinimumParticipants = entity.MinimumParticipants,
                AccountSymbol = entity.AccountSymbol,
                ShowAccountSymbol = (user != null) && entity.EventOrganisator.Any(eo => eo.AspNetUsers == user),
                IsDeleted = entity.State == EventStateEnum.Deleted,
                State = entity.State,
                Categories = entity.EventCategories.OrderBy(ec => ec.Id)
                .Select(ec => new Category()
                {
                    Name = ec.Name,
                    HtmlName = ec.HtmlName,
                }).ToList(),
                IsOrganisator = entity.EventOrganisator.Any(o => o.AspNetUsers == user),
            };
        }

        private static string GetLink(string link)
        {
            try
            {
                return string.Format("<a href=\"{0}\">{1}</a>", link, new Uri(link).Host);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static DiscussionViewModel ToViewModel(this Discussion entity)
        {
            return new DiscussionViewModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                HtmlName = entity.HtmlName,
                Perex = entity.Perex,
                Author = new User(entity.AspNetUsers),
                ItemsCount = entity.DiscussionItem.Count,
                LastUpdateDate = entity.LastUpdateDate.Value,
                CreateDate = entity.CreateDate.Value,
                CreateDateString = entity.CreateDate.Value.ToDayDateTimeString(),
            };
        }

        public static DiscussionItemViewModel ToViewModel(this DiscussionItem entity)
        {
            return new DiscussionItemViewModel()
            {
                Author = new User(entity.AspNetUsers),
                DateTime = entity.DateTime.ToDayDateTimeString(),
                Text = entity.Text,
            };
        }

        public static PhotoAlbumViewModel ToViewModel(this PhotoAlbum entity)
        {
            return new PhotoAlbumViewModel(entity);
        }

        public static PhotoAlbumPhotos ToPhotoViewModel(this PhotoAlbum entity, IRajceService rajceService)
        {
            var album = rajceService.GetAlbum(entity.AlbumLink);

            return new PhotoAlbumPhotos()
            {
                EventName = entity.Event.Name,
                EventHtml = entity.Event.HtmlName,
                User = new User(entity.AspNetUsers),
                Photos = album.Photos,
                Id_Discussion = entity.Id_Discussion,
                Title = $"Album k akci {entity.Event.Name} od uživatele {entity.AspNetUsers.UserName}",
            };
        }

        public static User ToWebUser(this AspNetUsers user)
        {
            return new User
            {
                htmlName = user.HtmlName,
                id = user.Id.ToString(),
                name = user.HtmlName,
                IsDeleted = user.LoginType == LoginTypeEnum.Deleted,
                ProfilePhoto = user.ProfilePhoto,
            };
        }
    }
}