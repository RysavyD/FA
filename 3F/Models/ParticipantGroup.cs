using _3F.Model.Model;
using _3F.Web.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3F.Web.Models
{
    public class ParticipantGroup
    {
        public EventLoginEnum EventLogin { get; set; }
        public IEnumerable<Participant> Participants { get; set; }
        public bool AlwaysShow { get; set; }

        public ParticipantGroup(EventLoginEnum eventLogin, IEnumerable<Participant> participants, bool alwaysShow = false)
        {
            this.EventLogin = eventLogin;
            this.Participants = participants.OrderBy(p => p.Time);
            this.AlwaysShow = alwaysShow;
        }
    }
}