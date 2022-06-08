
using System.Collections.Generic;

namespace _3F.Web.Models
{
    public class OrganisationViewModel : BaseViewModel
    {
        public string Text { get; set; }
        public User Chief { get; set; }
        public IEnumerable<User> Council { get; set; }
        public IEnumerable<User> Supervisors { get; set; }
        public IEnumerable<User> CertifiedOrganisators { get; set; }
        public IEnumerable<User> Members { get; set; }

        public OrganisationViewModel()
        {
            Council = new List<User>();
            Supervisors= new List<User>();
            CertifiedOrganisators = new List<User>();
            Members = new List<User>();
        }
    }
}