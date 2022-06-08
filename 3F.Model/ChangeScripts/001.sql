CREATE PROCEDURE [dbo].[spGetOldEventList] 
	@user_id int = 0,
	@page int = 1,
	@pagesize int = 10,
	@actualTime datetime
AS
BEGIN
DECLARE @offset int
SET @offset = (@page - 1) * @pagesize

   SELECT COUNT(ev.Id)
   FROM [Event] ev
   WHERE 
   ev.[State] = 0
   AND ev.StartDateTime < @actualTime
   AND (ev.EventType <> 4 OR (SELECT COUNT(*) FROM [EventInvitations] ei WHERE ei.Id_Event = ev.Id AND ei.Id_User = @user_id) > 0)

   SELECT 
		ev.Id,
		ev.Name,
		ev.Perex,
		ev.HtmlName,
		ev.Photo,
		ev.StartDateTime AS Start,
		ev.StopDateTime AS Stop,
		IIF(EventType = 2, 1, 0) AS IsPaid,
		IIF(EventType in (2, 3), 1, 0) AS IsOfficial,
		IIF(Photo is null, 0, 1) AS HasPhoto,
		(SELECT COUNT(*) FROM EventParticipant Where Id_Event=ev.Id AND EventLoginStatus IN (1,4,5,11)) AS YesParticipants,
		(SELECT COUNT(*) FROM EventParticipant Where Id_Event=ev.Id AND EventLoginStatus = 2) AS MayBeParticipants,
		ev.Capacity,
		(SELECT TOP 1 [DateTime] FROM DiscussionItem dis WHERE dis.Id_Discussion = ev.Id_Discussion ORDER BY [DateTime] DESC) AS LastDiscussionItemDate,
		(SELECT EventLoginStatus FROM EventParticipant WHERE Id_Event=ev.Id AND Id_User=@user_id AND IsExternal = 0) AS ParticipantStatus,
		ev.MayBeLogOn AS MayBeAllowed,
		IIF(EventType = 4, 1, 0) AS IsPrivated,
		IIF([State] = 4, 1, 0) AS IsInWork
   FROM [Event] ev
   WHERE 
   ev.[State] = 0
   AND ev.StartDateTime < @actualTime
   AND (ev.EventType <> 4 OR (SELECT COUNT(*) FROM [EventInvitations] ei WHERE ei.Id_Event = ev.Id AND ei.Id_User = @user_id) > 0)

   ORDER BY ev.StartDateTime DESC
   OFFSET @offset ROWS
   FETCH NEXT @pagesize ROWS ONLY
END
