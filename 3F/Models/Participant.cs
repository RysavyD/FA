using _3F.Model.Model;
using System;
using _3F.Web.Definitions;

namespace _3F.Web.Models
{
    public class Participant
    {
        public User User { get; set; }
        public EventLoginEnum LoginStatus { get; set; }
        public DateTime Time { get; set; }
        public bool IsExternal { get; set; }
        public int Id { get; set; }

        public Participant() { }

        public Participant(EventParticipant participant)
        {
            LoginStatus = participant.EventLoginStatus;
            User = (participant.IsExternal) ?
                new User()
                {
                    htmlName = participant.AspNetUsers.HtmlName,
                    name = "Externí účastník od " + participant.AspNetUsers.UserName,
                    ProfilePhoto = "Ghost.png",                    
                }
                :
                new User(participant.AspNetUsers);
            Time = participant.Time;
            IsExternal = participant.IsExternal;
            Id = participant.Id;
        }
    }

    public class ParticipantHistory : Participant
    {
        public EventLoginEnum OldLoginStatus { get; set; }

        public ParticipantHistory(EventParticipantHistory history)
        {
            LoginStatus = history.NewEventLoginStatus;
            OldLoginStatus = history.OldEventLoginStatus;
            Time = history.Time;
            User = new User(history.AspNetUsers);
            IsExternal = history.IsExternal;
        }

        public string[] ToArray()
        {
            return new[] { (IsExternal) ? User.name + " (external)" : User.name, OldLoginStatus.ToString(), LoginStatus.ToString(), Time.ToString(Strings.DateFormat) };
        }
    }
}