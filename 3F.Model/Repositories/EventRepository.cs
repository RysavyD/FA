using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using _3F.BusinessEntities;
using _3F.BusinessEntities.Akce;
using _3F.Model.Repositories.Models;
using Dapper;

namespace _3F.Model.Repositories
{
    public class EventRepository : IEventRepository
    {
        public Tuple<int, IEnumerable<EventListModel>> GetActualEvents(int userId, int page, int count)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                int offset = (page - 1) * count;

                sqlConnection.Open();
                var sql = "SELECT COUNT(ev.Id) FROM[Event] ev WHERE ev.[State] = 0 AND ev.StopDateTime >= @actualTime"
                    + " AND(ev.EventType <> 4 OR(SELECT COUNT(*) FROM[EventInvitations] ei WHERE ei.Id_Event = ev.Id AND ei.Id_User = @user_id) > 0)"
                    + " SELECT ev.Id,ev.Name,ev.Perex,ev.HtmlName,ev.Photo,ev.StartDateTime AS Start,ev.StopDateTime AS Stop,"
                    + " IIF(EventType = 2, 1, 0) AS IsPaid,IIF(EventType in (2, 3), 1, 0) AS IsOfficial, IIF(Photo is null, 0, 1) AS HasPhoto,"
                    + " (SELECT COUNT(*) FROM EventParticipant Where Id_Event = ev.Id AND EventLoginStatus IN(1, 4, 5, 11)) AS YesParticipants,"
                    + " (SELECT COUNT(*) FROM EventParticipant Where Id_Event = ev.Id AND EventLoginStatus = 2) AS MayBeParticipants,"
                    + " ev.Capacity,"
                    + " (SELECT TOP 1[DateTime] FROM DiscussionItem dis WHERE dis.Id_Discussion = ev.Id_Discussion ORDER BY[DateTime] DESC) AS LastDiscussionItemDate,"
                    + " (SELECT EventLoginStatus FROM EventParticipant WHERE Id_Event = ev.Id AND Id_User = @user_id AND IsExternal = 0) AS ParticipantStatus,"
                    + " ev.MayBeLogOn AS MayBeAllowed, IIF(EventType = 4, 1, 0) AS IsPrivated,IIF([State] = 4, 1, 0) AS IsInWork, IIF(EventType = 6, 1, 0) AS IsCommercial,"
                    + " IIF(EventType = 5, 1, 0) AS IsSuggested"
                    + " FROM[Event] ev WHERE ev.[State] = 0 AND ev.StopDateTime >= @actualTime"
                    + " AND(ev.EventType <> 4 OR(SELECT COUNT(*) FROM[EventInvitations] ei WHERE ei.Id_Event = ev.Id AND ei.Id_User = @user_id) > 0)"
                    + " ORDER BY ev.StopDateTime OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY";

                var grid = sqlConnection.QueryMultiple(sql,
                    new
                    {
                        user_id = userId,
                        actualTime = Info.CentralEuropeNow,
                        page,
                        pagesize = count,
                        offset,
                    });

                var result = new Tuple<int, IEnumerable<EventListModel>>(grid.ReadFirst<int>(),
                    grid.Read<EventListModel>());

                sqlConnection.Close();

                return result;
            }
        }

        public Tuple<int, IEnumerable<EventListModel>> GetOldEvents(int userId, int page, int count)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                int offset = (page - 1) * count;
                sqlConnection.Open();

                var sql = "SELECT COUNT(ev.Id) FROM[Event] ev WHERE ev.[State] = 0 AND ev.StopDateTime < @actualTime"
                    + " AND(ev.EventType <> 4 OR(SELECT COUNT(*) FROM[EventInvitations] ei WHERE ei.Id_Event = ev.Id AND ei.Id_User = @user_id) > 0)"
                    + " SELECT ev.Id,ev.Name,ev.Perex,ev.HtmlName,ev.Photo,ev.StartDateTime AS Start,ev.StopDateTime AS Stop,"
                    + " IIF(EventType = 2, 1, 0) AS IsPaid,IIF(EventType in (2, 3), 1, 0) AS IsOfficial, IIF(Photo is null, 0, 1) AS HasPhoto,"
                    + " (SELECT COUNT(*) FROM EventParticipant Where Id_Event = ev.Id AND EventLoginStatus IN(1, 4, 5, 11)) AS YesParticipants,"
                    + " (SELECT COUNT(*) FROM EventParticipant Where Id_Event = ev.Id AND EventLoginStatus = 2) AS MayBeParticipants,"
                    + " ev.Capacity,"
                    + " (SELECT TOP 1[DateTime] FROM DiscussionItem dis WHERE dis.Id_Discussion = ev.Id_Discussion ORDER BY[DateTime] DESC) AS LastDiscussionItemDate,"
                    + " (SELECT EventLoginStatus FROM EventParticipant WHERE Id_Event = ev.Id AND Id_User = @user_id AND IsExternal = 0) AS ParticipantStatus,"
                    + " ev.MayBeLogOn AS MayBeAllowed, IIF(EventType = 4, 1, 0) AS IsPrivated,IIF([State] = 4, 1, 0) AS IsInWork, IIF(EventType = 6, 1, 0) AS IsCommercial,"
                    + " IIF(EventType = 5, 1, 0) AS IsSuggested"
                    + " FROM[Event] ev WHERE ev.[State] = 0 AND ev.StartDateTime < @actualTime"
                    + " AND(ev.EventType <> 4 OR(SELECT COUNT(*) FROM[EventInvitations] ei WHERE ei.Id_Event = ev.Id AND ei.Id_User = @user_id) > 0)"
                    + " ORDER BY ev.StartDateTime DESC OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY";

                var grid = sqlConnection.QueryMultiple(sql,
                    new
                    {
                        user_id = userId,
                        actualTime = Info.CentralEuropeNow,
                        page,
                        pagesize = count,
                        offset,
                    });

                var result = new Tuple<int, IEnumerable<EventListModel>>(grid.ReadFirst<int>(),
                    grid.Read<EventListModel>());

                sqlConnection.Close();

                return result;
            }
        }

        public IEnumerable<EventModel> GetEventsFromMonth(int year, int month)
        {
            var startDate = new DateTime(year, month, 1).AddDays(-7);
            var stopDate = new DateTime(year, month, 1).AddMonths(1).AddDays(7);

            var sql = "SELECT [Name],[Perex],[HtmlName],[StartDateTime] AS Start,[StopDateTime] AS Stop, [EventType] FROM [dbo].[Event] ev"
                      + " WHERE ev.[State] = 0 AND ev.[EventType] <> 4 "
                      + " AND ev.[StartDateTime] > @startDate AND ev.[StopDateTime] < @stopDate";

            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var result = sqlConnection.Query<EventModel>(sql, new {startDate, stopDate});
                sqlConnection.Close();

                return result;
            }
        }

        public EventDetail GetEventDetail(string htmlName)
        {
            var sql = "DECLARE @IdEvent int "
                + "SET @IdEvent = (SELECT TOP 1 Id FROM[Event] WHERE HtmlName = @htmlName) "
                + " "
                + " SELECT[Id] ,[Name] ,[Perex] ,[Description] ,[Capacity] ,[Place] ,[StartDateTime] AS Start ,[StopDateTime] AS Stop,[LastSignINDateTime] AS LastSignIn"
                + " ,[MeetDateTime] AS MeetTime ,[MeetPlace] ,[Contact] ,[Price]  ,[HtmlName] ,[Id_Discussion] AS IdDiscussion ,[Link]"
                + " ,[MayBeLogOn] ,[LastPaidDateTime] AS LastPaid ,[MinimumParticipants] "
                + " ,[Photo] ,[AccountSymbol]  ,[State] ,[Costs]  ,[EventType]  ,[CostsDescription] "
                + " FROM [Event] WHERE[Id] = @IdEvent"
                + " "
                + "SELECT usr.Id, usr.UserName, usr.HtmlName, usr.ProfilePhoto, usr.LoginType FROM [EventOrganisator] eo"
                + " JOIN [AspNetUsers] usr ON eo.Id_User = usr.Id WHERE eo.Id_Event = @IdEvent"
                + " "
                + "SELECT part.EventLoginStatus, part.IsExternal, part.[Time], usr.Id, usr.UserName, usr.HtmlName, usr.ProfilePhoto, usr.LoginType FROM [EventParticipant] part"
                + " JOIN [AspNetUsers] usr ON part.Id_User = usr.Id WHERE part.Id_Event = @IdEvent ORDER BY part.[Time]"
                + " "
                + "SELECT evSum.Name, evSum.Perex, ev.HtmlName, usr.UserName, usr.HtmlName AS AuthorHtml, usr.ProfilePhoto, usr.LoginType FROM EventSummary evSum"
                + " JOIN[AspNetUsers] usr ON evSum.Id_User = usr.Id JOIN[Event] ev ON evSum.Id_Event = ev.Id"
                + " WHERE evSum.Id_Event = @IdEvent"
                + " "
                + "SELECT pho.Id, pho.CoverPhotoLink, pho.PhotoCount, usr.UserName, usr.HtmlName, usr.ProfilePhoto, usr.LoginType FROM PhotoAlbum pho"
                + " JOIN[AspNetUsers] usr ON pho.Id_User = usr.Id WHERE pho.Id_Event = @IdEvent"
                + " "
                + "SELECT Id, Name, HtmlName FROM EventCategories ecs"
                + " JOIN EventCategory ec ON ec.Id = ecs.Id_EventCategory WHERE ecs.Id_Event = @IdEvent";

            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var grid = sqlConnection.QueryMultiple(sql, new { htmlName });

                var result = grid.ReadFirst<EventDetail>();
                result.Organisators = grid.Read<User>();
                var participants = grid.Read();
                var summary = grid.Read().SingleOrDefault();
                var photos = grid.Read();
                result.Categories = grid.Read<Category>();

                sqlConnection.Close();

                result.Participants = participants.Select(p =>
                    new Participant()
                    {
                        LoginStatus = p.EventLoginStatus,
                        IsExternal = p.IsExternal,
                        Time = p.Time,
                        User = new User()
                        {
                            Id = p.Id,
                            HtmlName = p.HtmlName,
                            LoginType = p.LoginType,
                            ProfilePhoto = p.IsExternal ? "Ghost.png" : p.ProfilePhoto,
                            UserName = p.IsExternal ? "Externí účastník od " + p.UserName : p.UserName,
                        }
                    });

                if (summary != null)
                {
                    result.Summary = new EventSummary()
                    {
                        HtmlName = summary.HtmlName,
                        Name = summary.Name,
                        Perex = summary.Perex,
                        Author = new User()
                        {
                            UserName = summary.UserName,
                            HtmlName = summary.AuthorHtml,
                            ProfilePhoto = summary.ProfilePhoto,
                            LoginType = summary.LoginType,
                        }
                    };
                }

                result.PhotoAlbums = photos.Select(p =>
                    new PhotoAlbum()
                    {
                         Id = p.Id,
                         CoverPhotoLink = p.CoverPhotoLink,
                         PhotoCount = p.PhotoCount,
                         Author = new User()
                         {
                             UserName = p.UserName,
                             HtmlName = p.HtmlName,
                             ProfilePhoto = p.ProfilePhoto,
                             LoginType = p.LoginType,
                         }
                    }).ToList();
                   
                return result;
            }
        }

        public EventDetail GetEventParticipantsPart(int idEvent)
        {
            var sql = "SELECT[Id] ,[Name] ,[Perex] ,[Description] ,[Capacity] ,[Place] ,[StartDateTime] AS Start ,[StopDateTime] AS Stop,[LastSignINDateTime] AS LastSignIn"
                      + " ,[MeetDateTime] AS MeetTime ,[MeetPlace] ,[Contact] ,[Price]  ,[HtmlName] ,[Id_Discussion] AS IdDiscussion ,[Link]"
                      + " ,[MayBeLogOn] ,[LastPaidDateTime] AS LastPaid ,[MinimumParticipants] "
                      + " ,[Photo] ,[AccountSymbol]  ,[State] ,[Costs]  ,[EventType]  ,[CostsDescription] "
                      + " FROM [Event] WHERE[Id] = @IdEvent"
                      + " "
                      + "SELECT part.EventLoginStatus, part.IsExternal, part.[Time], usr.Id, usr.UserName, usr.HtmlName, usr.ProfilePhoto, usr.LoginType FROM [EventParticipant] part"
                      + " JOIN [AspNetUsers] usr ON part.Id_User = usr.Id WHERE part.Id_Event = @IdEvent ORDER BY part.[Time]";

            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var grid = sqlConnection.QueryMultiple(sql, new { idEvent });

                var result = grid.ReadFirst<EventDetail>();
                var participants = grid.Read();

                sqlConnection.Close();

                result.Participants = participants.Select(p =>
                    new Participant()
                    {
                        LoginStatus = p.EventLoginStatus,
                        IsExternal = p.IsExternal,
                        Time = p.Time,
                        User = new User()
                        {
                            Id = p.Id,
                            HtmlName = p.HtmlName,
                            LoginType = p.LoginType,
                            ProfilePhoto = p.IsExternal ? "Ghost.png" : p.ProfilePhoto,
                            UserName = p.IsExternal ? "Externí účastník od " + p.UserName : p.UserName,
                        }
                    });
               
                return result;
            }
        }

        public IEnumerable<User> GetInvitedUsers(int eventId)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                var sql = "SELECT usr.Id, usr.UserName, usr.HtmlName, usr.ProfilePhoto"
                    + " FROM[EventInvitations] ei JOIN[AspNetUsers] usr ON ei.Id_User = usr.Id"
                    + " WHERE ei.Id_Event = @eventId";

                sqlConnection.Open();
                var users = sqlConnection.Query<User>(sql, new {eventId});
                sqlConnection.Close();
                return users;
            }
        }

        public int UndecidedEventsCount(int userId)
        {
            var sql = "SELECT COUNT(ev.id) FROM[Event] ev WHERE ev.[State] = 0"
                + " AND((SELECT COUNT(ep.Id) FROM[EventParticipant] ep WHERE ep.Id_User = @user_id AND ep.Id_Event = ev.Id) = 0)"
                + " AND ev.StopDateTime >= @actualTime AND(ev.EventType <> 4 OR(SELECT COUNT(*) FROM[EventInvitations] ei WHERE ei.Id_Event = ev.Id AND ei.Id_User = @user_id) > 0)"
                + " AND (ev.EventType <> 5)";

            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var result = sqlConnection.ExecuteScalar(sql, new
                {
                    user_id = userId,
                    actualTime = Info.CentralEuropeNow,
                });
                sqlConnection.Close();

                return Convert.ToInt32(result);
            }
        }
    }
}
