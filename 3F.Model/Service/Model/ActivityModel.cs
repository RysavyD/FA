using System;
using _3F.Model.Model;

namespace _3F.Model.Service.Model
{
    public class ActivityModel
    {
        public User User { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
    }

    public class User
    {
        public string name { get; set; }
        public string id { get; set; }
        public string htmlName { get; set; }
        public string ProfilePhoto { get; set; }
        public bool IsDeleted { get; set; }

        public User() { }

        public User(AspNetUsers entityUser)
        {
            name = entityUser.UserName;
            id = entityUser.Id.ToString();
            htmlName = entityUser.HtmlName;
            ProfilePhoto = entityUser.ProfilePhoto;
            IsDeleted = entityUser.LoginType == LoginTypeEnum.Deleted;
        }
    }
}
