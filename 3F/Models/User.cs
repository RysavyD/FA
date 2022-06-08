using _3F.Model.Model;

namespace _3F.Web.Models
{
    public class User
    {
        public string name { get; set; }
        public string id { get; set; }
        public string htmlName { get; set; }
        public string ProfilePhoto { get; set; }
        public bool IsDeleted { get; set; }

        public User() { }

        public User(AspNetUsers user)
        {
            name = user.UserName;
            id = user.Id.ToString();
            htmlName = user.HtmlName;
            ProfilePhoto = user.ProfilePhoto;
            IsDeleted = user.LoginType == LoginTypeEnum.Deleted;
        }

        public User(BusinessEntities.User user)
        {
            name = user.UserName;
            id = user.Id.ToString();
            htmlName = user.HtmlName;
            ProfilePhoto = user.ProfilePhoto;
            IsDeleted = user.LoginType == (int)LoginTypeEnum.Deleted;
        }
    }

    public class ActivityUser : User
    {
        public string LastActivity { get; set; }

        public ActivityUser()
            : base()
        { }

        public ActivityUser(AspNetUsers user)
            : base(user)
        {
            this.LastActivity = user.DateLastActivity.ToString("d. M. yyyy");
        }
    }
}