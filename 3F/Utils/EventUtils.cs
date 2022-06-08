using System;
using System.Collections.Generic;
using System.Linq;
using _3F.Log;
using _3F.Model;
using _3F.Model.Model;
using _3F.Model.Email;
using _3F.Model.Email.Model;
using _3F.Model.Service;
using _3F.Web.Models;

namespace _3F.Web.Utils
{
    public interface IEventUtils
    {
        void WorkFlow(Event entity, AspNetUsers user);
        void Publicate(Event entity, AspNetUsers user, bool confirming = false);
        IEnumerable<KeyValuePair<string,string>> ExtendedValidateEventEntity(Event newEntity, Event oldEntity);
        Event DuplicateEvent(Event entity, int moveDayCount, string newName);
        void RecalculateUsers(Event entity);
    }

    public class EventUtils : IEventUtils
    {
        private readonly IRepository repository;
        private readonly ILogger logger;
        private readonly ILogToEvent logToEvent;
        private readonly IActivityListService activityListService;
        private readonly IEmailSender emailSender; 
        const string SystemName = "Systém";

        public EventUtils(IRepository repository, ILogger logger, ILogToEvent logToEvent, IActivityListService activityListService, IEmailSender emailSender)
        {
            this.repository = repository;
            this.logger = logger;
            this.logToEvent = logToEvent;
            this.activityListService = activityListService;
            this.emailSender = emailSender;
        }

        public void WorkFlow(Event entity, AspNetUsers user)
        {
            foreach (var org in entity.EventOrganisator)
            {
                logToEvent.LogToEvent(entity, repository.One<AspNetUsers>(org.Id_User), EventLoginEnum.Prijdu, false);
            }

            string userName = (user == null) ? SystemName : user.UserName;

            logger.LogInfo(string.Format("Uživatel {0} vypublikoval akci {1}", userName, entity.Name), "EventUtils.WorkFlow");

        }

        public void Publicate(Event entity, AspNetUsers user, bool confirming = false)
        {
            if (user == null)
                user = repository.One<AspNetUsers>(u => u.UserName == SystemName);

            if (entity.IsPrivate)
                return;

            if (!confirming)
            {
                if (entity.EventType == EventTypeEnum.TipNaAkci)
                {
                    activityListService.AddActivity(ActivityCreator.Create(user, entity, ActivityType.CreateSuggestedEvent));
                }
                else
                {
                    activityListService.AddActivity(ActivityCreator.Create(user, entity, ActivityType.CreateEvent));
                }
            }
            else
            {
                activityListService.AddActivity(ActivityCreator.Create(user, entity, ActivityType.ConfirmEvent));
            }

            // send email
            //if (!confirming)
            {
                if (entity.EventType == EventTypeEnum.TipNaAkci)
                {
                    var addressess = GetEmailAddressesByEventCategories(entity);
                    var emailModel = new NewEventEmailModel()
                    {
                        Name = entity.Name,
                        HtmlName = entity.HtmlName,
                        Perex = entity.Perex,
                        Place = entity.Place,
                        Price = entity.Price,
                        StartTime = entity.StartDateTime,
                        StopTime = entity.StopDateTime,
                    };

                    SendEmail(EmailType.NewSuggestedEvent, emailModel, addressess);
                }
                else
                {
                    var addressess = GetEmailAddressesByEventCategories(entity);
                    var emailModel = new NewEventEmailModel()
                    {
                        Name = entity.Name,
                        Capacity = entity.Capacity,
                        HtmlName = entity.HtmlName,
                        MeetPlace = entity.MeetPlace,
                        MeetTime = entity.MeetDateTime.Value,
                        Organisators = string.Join(", ", entity.EventOrganisator.Select(o => o.AspNetUsers.UserName)),
                        Perex = entity.Perex,
                        Place = entity.Place,
                        Price = entity.Price,
                        StartTime = entity.StartDateTime,
                        StopTime = entity.StopDateTime,
                    };

                    SendEmail(EmailType.NewEvent, emailModel, addressess);
                }
            }

            //try
            //{
            //    if (entity.Name.Contains("Badminton")
            //        && entity.Name.Contains("v Letňanech")
            //        && entity.EventOrganisator.Any(o => o.AspNetUsers.UserName == "Nerothar"))
            //    {
            //        var d = repository.One<AspNetUsers>(u => u.UserName == "du-šan");
            //        var l = repository.One<AspNetUsers>(u => u.UserName == "Liliana");

            //        logToEvent.LogToEvent(entity, d, EventLoginEnum.Prijdu, false);
            //        logToEvent.LogToEvent(entity, l, EventLoginEnum.Prijdu, false);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logger.LogInfo("automatické přihlášení uživatele selhalo", "EventUtils.Publicate");
            //    logger.LogException(ex, "EventUtils.Publicate");
            //}

            // post in FB
            // post in G+
        }

        private string[] GetEmailAddressesByEventCategories(Event entity)
        {
            var categories = entity.EventCategories.Select(x => x.Id).ToArray();
            var lastActiveDate = Info.CentralEuropeNow.AddYears(-2);
            var addressess = repository
                    .Where<AspNetUsers>(u => u.LoginType == LoginTypeEnum.Confirmed ||
                                          u.LoginType == LoginTypeEnum.OldSystemConfirmed)
                    .Where(u => u.DateLastActivity >= lastActiveDate)
                    .Where(u => u.EventCategories.Any(ec => categories.Contains(ec.Id)) || u.MainCategories.Any(mc => mc.MainCategory == entity.MainCategory)
                        || (entity.IsStay && u.Profiles.SendEventIsStayMail))
                    .Select(u => u.Email)
                    .Where(u => u != null)
                    .ToArray();

            return addressess;
        }

        private void SendEmail(EmailType emailType, NewEventEmailModel emailModel, string[] addressess)
        {
            try
            {
                emailSender.SendEmail(emailType, emailModel, addressess);
            }
            catch (Exception e)
            {
                logger.LogInfo("Error při odesílání mailu", "EventUtils.SendEmail");
                logger.LogException(e, "EventUtils.SendEmail");
            }
        }

        public IEnumerable<KeyValuePair<string,string>> ExtendedValidateEventEntity(Event newEntity, Event oldEntity)
        {
            var blockDate = new DateTime(2008, 1, 1);
            if (newEntity.StartDateTime < blockDate)
                yield return new KeyValuePair<string, string>("Start", "Čas začátku musí být zadán");

            if (newEntity.StartDateTime < Info.CentralEuropeNow)
                yield return new KeyValuePair<string, string>("Start", "Čas začátku nesmí být v minulosti");

            if (newEntity.StopDateTime < blockDate)
                yield return new KeyValuePair<string, string>("Stop", "Čas konce musí být zadán");

            if (newEntity.StartDateTime > newEntity.StopDateTime)
                yield return new KeyValuePair<string, string>("Start", "Začátek akce musí být dříve než konec akce");

            if (newEntity.LastSignINDateTime != null && (newEntity.LastSignINDateTime > newEntity.StartDateTime))
                yield return new KeyValuePair<string, string>("LastSignINDateTime", "Čas posledního přihlášení musí být před začátkem akce");

            if (newEntity.LastSignINDateTime != null && (oldEntity != null && newEntity.LastSignINDateTime != oldEntity.LastSignINDateTime) && newEntity.LastSignINDateTime < Info.CentralEuropeNow)
                yield return new KeyValuePair<string, string>("LastSignINDateTime", "Čas posledního přihlášení nesmí být v minulosti");

            if (newEntity.MinimumParticipants > newEntity.Capacity && newEntity.Capacity > 0)
                yield return new KeyValuePair<string, string>("MinimumParticipants", "Minimální počet účastníků nesmí být větší než kapacita akce");

            if (newEntity.EventType != EventTypeEnum.TipNaAkci)
            {
                if (string.IsNullOrEmpty(newEntity.MeetPlace))
                    yield return new KeyValuePair<string, string>("MeetPlace", "Místo srazu je povinné");

                if (newEntity.MeetDateTime < blockDate)
                    yield return new KeyValuePair<string, string>("MeetTime", "Čas srazu musí být zadán");

                if (newEntity.MeetDateTime > newEntity.StartDateTime)
                    yield return new KeyValuePair<string, string>("MeetTime", "Čas srazu musí být před začátkem akce");
            }

            if (newEntity.EventType == EventTypeEnum.PlacenaSdruzenim)
            {
                if (newEntity.LastPaidDateTime < blockDate)
                    yield return new KeyValuePair<string, string>("LastPaidDateTime", "Čas posledního vrácení peněz musí být zadán");

                if (string.IsNullOrWhiteSpace(newEntity.Place))
                    yield return new KeyValuePair<string, string>("Place", "Místo akce je povinné");

                if (string.IsNullOrWhiteSpace(newEntity.Contact))
                    yield return new KeyValuePair<string, string>("Contact", "Je třeba uvést kontakt na organizátora");

                if (string.IsNullOrWhiteSpace(newEntity.Link))
                    yield return new KeyValuePair<string, string>("Link", "Je třeba uvést odkaz");

                if (newEntity.LastPaidDateTime == null)
                    yield return new KeyValuePair<string, string>("LastPaidDateTime", "Čas posledního vrácení peněz je povinný");

                if (newEntity.LastSignINDateTime == null)
                    yield return new KeyValuePair<string, string>("LastSignINDateTime", "Čas posledního odhlášení je povinný");

                if (newEntity.LastPaidDateTime != null && oldEntity != null && oldEntity.LastPaidDateTime != newEntity.LastPaidDateTime 
                    && newEntity.LastPaidDateTime < Info.CentralEuropeNow)
                    yield return new KeyValuePair<string, string>("LastPaidDateTime", "Čas posledního vrácení peněz nesmí být v minulosti");

                if (newEntity.LastPaidDateTime != null && newEntity.StartDateTime <= newEntity.LastPaidDateTime)
                    yield return new KeyValuePair<string, string>("LastPaidDateTime", "Čas posledního vrácení peněz musí být před začátkem akce");

                if (newEntity.MinimumParticipants > newEntity.Capacity && newEntity.Capacity > 0)
                    yield return new KeyValuePair<string, string>("MinimumParticipants", "Minimální počet účastníků je vyšší než rezervace");

                if (newEntity.Costs <= 0)
                    yield return new KeyValuePair<string, string>("Costs", "Náklady akce nesmí být nulové nebo záporné");

                if (newEntity.Costs != 0 && newEntity.Capacity > 0 && (newEntity.Costs > (newEntity.Capacity * newEntity.Price)))
                    yield return new KeyValuePair<string, string>("Costs", "Náklady akce převyšují příjmy akce");

                if (string.IsNullOrWhiteSpace(newEntity.CostsDescription))
                    yield return new KeyValuePair<string, string>("CostsDescription", "Náklady akce musí být popsány!");
            }
        }

        public Event DuplicateEvent(Event entity, int moveDayCount, string newName)
        {
            var newEntity = new Event()
            {
                BankAccount = entity.BankAccount,
                Capacity = entity.Capacity,
                Contact = entity.Contact,
                Costs = entity.Costs,
                CostsDescription = entity.CostsDescription,
                Description = entity.Description,
                EventType = entity.EventType,
                ExternParticipants = 0,
                LastPaidDateTime = (entity.LastPaidDateTime.HasValue) ? entity.LastPaidDateTime.Value.AddDays(moveDayCount) : (DateTime?)null,
                LastSignINDateTime = (entity.LastSignINDateTime.HasValue) ? entity.LastSignINDateTime.Value.AddDays(moveDayCount) : (DateTime?)null,
                Link = entity.Link,
                MayBeLogOn = entity.MayBeLogOn,
                MeetDateTime = entity.MeetDateTime.Value.AddDays(moveDayCount),
                MeetPlace = entity.MeetPlace,
                MinimumParticipants = entity.MinimumParticipants,
                Name = newName,
                Perex = entity.Perex,
                Photo = entity.Photo,
                Place = entity.Place,
                Price = entity.Price,
                State = entity.State,
                StartDateTime = entity.StartDateTime.AddDays(moveDayCount),
                StopDateTime = entity.StopDateTime.AddDays(moveDayCount),
                MainCategory = entity.MainCategory,
                IsStay = entity.IsStay,
            };

            this.repository.SetHtmlName<Event>(newEntity, newEntity.Name);

            var discussionEntity = new Discussion() { Name = newEntity.Name, HtmlName = newEntity.HtmlName, IsAlone = false };
            newEntity.Discussion = discussionEntity;

            newEntity.AccountSymbol = repository.All<Event>().Max(ev => ev.AccountSymbol) + 1;

            foreach(var organisator in entity.EventOrganisator)
            {
                newEntity.EventOrganisator.Add(new EventOrganisator()
                    {
                        Event = newEntity,
                        Id_User = organisator.Id_User,
                    });
            }

            foreach(var category in entity.EventCategories)
            {
                newEntity.EventCategories.Add(category);
            }

            return newEntity;
        }

        public void RecalculateUsers(Event entity)
        {
            // Recalculate participants on event (when capacity was changed)
            int eventParticipantsCount = entity.EventParticipant.Count(i => i.EventLoginStatus == EventLoginEnum.Prijdu
                    || i.EventLoginStatus == EventLoginEnum.Rezervace
                    || i.EventLoginStatus == EventLoginEnum.NepotvrzenaRezervace);
            if (entity.Capacity > eventParticipantsCount)
            {
                // there is place for other participants
                int alternateParticipantsCount =
                    entity.EventParticipant.Count(i => i.EventLoginStatus == EventLoginEnum.Nahradnik);
                if (alternateParticipantsCount > 0)
                {
                    //there are alternates -> to reservation
                    IEnumerable<EventParticipant> alternate = entity.EventParticipant
                        .Where(i => i.EventLoginStatus == EventLoginEnum.Nahradnik)
                        .OrderBy(i => i.Time)
                        .Take(entity.Capacity - eventParticipantsCount);
                    
                    foreach (var participant in alternate)
                    {
                        logToEvent.LogToEvent(entity, participant.AspNetUsers, EventLoginEnum.NepotvrzenaRezervace, false);

                        var emailModel = new NewReservation()
                        {
                            Name = entity.Name,
                            HtmlName = entity.HtmlName,
                            EndReservationTime = Info.CentralEuropeNow.AddDays(5),
                        };

                        emailSender.SendEmail(EmailType.FreePlaceOnEvent, emailModel, participant.AspNetUsers.Email);
                    }
                }
            }
        }
    }
}