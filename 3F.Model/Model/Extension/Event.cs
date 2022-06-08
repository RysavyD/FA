using System;
using System.Linq;

namespace _3F.Model.Model
{
    public partial class Event
    {
        public string GetWebHtmlLink
        {
            get
            {
                return String.Format("<a href=\"{0}/akce/detaily/{1}\">{2}</a>",
                    Properties.Settings.Default.WebUrl,
                    HtmlName,
                    Name);
            }
        }

        public bool IsPaidByOrganisation
        {
            get
            {
                return EventType == EventTypeEnum.PlacenaSdruzenim;
            }
        }

        public bool IsPrivate
        {
            get
            {
                return EventType == EventTypeEnum.Soukroma;
            }
        }

        public bool UserHasInvivation(AspNetUsers user)
        {
            return EventInvitation.Any(ev => ev.AspNetUsers == user);
        }
    }
}
