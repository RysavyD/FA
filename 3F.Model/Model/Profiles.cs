namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Profiles
    {
        public Profiles()
        {
            AspNetUsers = new HashSet<AspNetUsers>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public int? BirhtYear { get; set; }

        public string Motto { get; set; }

        public string Hobbies { get; set; }

        public RelationshipStatus Status { get; set; }

        [StringLength(200)]
        public string Link { get; set; }

        public bool SendNewActionToMail { get; set; }

        public bool SendMessagesToMail { get; set; }

        public bool SendMessagesFromAdminToMail { get; set; }

        public bool SendMayBeEventNotice { get; set; }

        public bool SendNewAlbumsToMail { get; set; }

        public bool SendNewSummaryToMail { get; set; }

        public bool SendNewSuggestedEventToMail { get; set; }

        public bool SendEventIsStayMail { get; set; }

        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }

        public SexEnum Sex { get; set; }
    }

    public enum RelationshipStatus
    {
        [Description("Neuvedeno"), Display(Name = "Neuvedeno")]
        Undefined = 0,
        [Description("Nezadan�(-�)"), Display(Name = "Nezadan�(-�)")]
        Single = 1,
        [Description("Zadan�(-�)"), Display(Name = "Zadan�(-�)")]
        Engaged = 2,
        [Description("�enat� / vdan�"), Display(Name = "�enat� / vdan�")]
        Married = 3,
        [Description("Rozveden�(-�)"), Display(Name = "Rozveden�(-�)")]
        Divorced = 4,
        [Description("Vdovec / vdova"), Display(Name = "Vdovec / vdova")]
        Widower = 5,
    }

    public enum SexEnum
    {
        [Description("Neuvedeno"), Display(Name = "Neuvedeno")]
        Undefined = 0,
        [Description("Mu�"), Display(Name = "Mu�")]
        Male = 1,
        [Description("�ena"), Display(Name = "�ena")]
        Female = 2,
    }
}
