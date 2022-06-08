using _3F.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3F.Web.Models
{
    public class ProfilViewModel
    {
        public bool CanEdit { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public string City { get; set; }
        public string RegistrationDate { get; set; }
        public int? BirhtYear { get; set; }
        public string Motto { get; set; }
        public string Hobbies { get; set; }
        public string Status { get; set; }
        public bool SendNewActionToMail { get; set; }
        public bool SendMessagesToMail { get; set; }
        public bool SendMessagesFromAdminToMail { get; set; }
        public string VariableSymbol { get; set; }
        public string Link { get; set; }
        public string Money { get; set; }
        public string Image { get; set; }
        public string PhoneNumber { get; set; }
        public int EventOrganisedCount { get; set; }
        public int EventMissedCount { get; set; }
        public bool IsOrganisationMember { get; set; }
        public List<SimpleEventModel> FutureEvents { get; set; }
        public List<SimpleEventModel> HistoryEvents { get; set; }
        public SexEnum Sex { get; set; }

        public ProfilViewModel ()
        {
            this.FutureEvents = new List<SimpleEventModel>();
            this.HistoryEvents = new List<SimpleEventModel>();
        }
    }
}