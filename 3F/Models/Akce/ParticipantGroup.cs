using System.Collections.Generic;
using _3F.Model.Model;
using B = _3F.BusinessEntities.Akce;

namespace _3F.Web.Models.Akce
{
    public class ParticipantGroup
    {
        public EventLoginEnum EventLogin { get; }
        public IEnumerable<B.Participant> Participants { get; }
        public bool AlwaysShow { get; }

        public ParticipantGroup(EventLoginEnum eventLogin, IEnumerable<B.Participant> participants, bool alwaysShow = false)
        {
            EventLogin = eventLogin;
            Participants = participants;
            AlwaysShow = alwaysShow;
        }
    }
}