using System;

namespace _3F.Web.Controllers.API.Model
{
    public class ApiUser
    {
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public string Photo { get; set; }
    }

    public class ApiParticipant : ApiUser
    {
        public int Status { get; set; }
        public bool IsExtern { get; set; }
    }

    public class ApiUserDetail : ApiUser
    {
        public string BirhtYear { get; set; }
        public string City { get; set; }
        public string Hobbies { get; set; }
        public string Link { get; set; }
        public string Motto { get; set; }
        public string PhoneNumber { get; set; }
        public string Sex { get; set; }
        public string Status { get; set; }
    }
}