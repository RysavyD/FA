using _3F.Model.Model;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace _3F.Web.Models.Profil
{
    public class ProfilViewModel : BaseViewModel
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
        public RelationshipStatus Status { get; set; }
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
        public List<SimpleEventModel> OrganisedEvents { get; set; }
        public SexEnum Sex { get; set; }
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }

        public ProfilViewModel ()
        {
            FutureEvents = new List<SimpleEventModel>();
            HistoryEvents = new List<SimpleEventModel>();
            OrganisedEvents = new List<SimpleEventModel>();
        }
    }

    public class ProfilPhotoViewModel : BaseViewModel
    {
        public string PhotoName { get; set; }

        public ProfilPhotoViewModel()
        {
            Title = "Změna profilové fotky";
            Icon = "icon-camera";
        }
    }

    public class TransakceViewModel : BaseViewModel
    {
        public IEnumerable<string[]> Data { get; set; }
    }
}