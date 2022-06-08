using System;
using System.Collections.Generic;
using _3F.BusinessEntities;
using _3F.BusinessEntities.Akce;
using _3F.Model.Repositories.Models;

namespace _3F.Model.Repositories
{
    public interface IEventRepository
    {
        Tuple<int, IEnumerable<EventListModel>> GetActualEvents(int userId, int page, int count);
        Tuple<int, IEnumerable<EventListModel>> GetOldEvents(int userId, int page, int count);
        IEnumerable<EventModel> GetEventsFromMonth(int year, int month);
        EventDetail GetEventDetail(string htmlName);
        EventDetail GetEventParticipantsPart(int idEvent);
        IEnumerable<User> GetInvitedUsers(int eventId);
        int UndecidedEventsCount(int userId);
    }
}
