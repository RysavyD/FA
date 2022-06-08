using _3F.Model;
using _3F.Model.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace _3F.Web.Models
{
    public class ApplicationUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType 
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here 
            return userIdentity;
        }

        public LoginTypeEnum LoginType { get; set; }
        public string HtmlName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastActivity { get; set; }
        public string ProfilePhoto { get; set; }
        public int? VariableSymbol { get; set; }
        public int RegisterType { get; set; }
        public int VopVersion { get; set; }

        public virtual Profile Profile { get; set; }

        public ApplicationUser()
        {
            DateCreated = DateLastActivity = Info.CentralEuropeNow;
            ProfilePhoto = "Unknown_prof.jpg";
        }
    }

    public class Profile
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int? BirhtYear { get; set; }
        public string Motto { get; set; }
        public string Hobbies { get; set; }
        public RelationshipStatus Status { get; set; }
        public bool SendNewActionToMail { get; set; }
        public bool SendMessagesToMail { get; set; }
        public bool SendMessagesFromAdminToMail { get; set; }
        public bool SendMayBeEventNotice { get; set; }
        public bool SendNewAlbumsToMail { get; set; }
        public bool SendNewSummaryToMail { get; set; }
        public string Link { get; set; }
        public SexEnum Sex { get; set; }
    }

    public class CustomUserRole : IdentityUserRole<int> { }
    public class CustomUserClaim : IdentityUserClaim<int> { }
    public class CustomUserLogin : IdentityUserLogin<int> { }

    public class CustomRole : IdentityRole<int, CustomUserRole>
    {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }

    public class CustomUserStore : UserStore<ApplicationUser, CustomRole, int,
        CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        public CustomRoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim> 
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}