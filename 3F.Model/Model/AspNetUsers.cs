namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AspNetUsers : IPrimaryKey, IHtmlName
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            Discussion = new HashSet<Discussion>();
            DiscussionItem = new HashSet<DiscussionItem>();
            EventOrganisator = new HashSet<EventOrganisator>();
            EventParticipant = new HashSet<EventParticipant>();
            EventParticipantHistory = new HashSet<EventParticipantHistory>();
            EventSummary = new HashSet<EventSummary>();
            Message = new HashSet<Message>();
            MessageRecipient = new HashSet<MessageRecipient>();
            OldPassword = new HashSet<OldPassword>();
            Payment = new HashSet<Payment>();
            PhotoAlbum = new HashSet<PhotoAlbum>();
            TouristCardOwner = new HashSet<TouristCardOwner>();
            TouristStampOwner = new HashSet<TouristStampOwner>();
            AspNetRoles = new HashSet<AspNetRoles>();
            OrganisationMember = new HashSet<OrganisationMember>();
            EventInvitation = new HashSet<EventInvitation>();
            EventCategories = new HashSet<EventCategory>();
            MainCategories = new HashSet<AspNetUsersMainCategory>();
        }

        public int Id { get; set; }

        public LoginTypeEnum LoginType { get; set; }

        public string HtmlName { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateLastActivity { get; set; }

        public string ProfilePhoto { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        [Required]
        [StringLength(256)]
        public string UserName { get; set; }

        public int? VariableSymbol { get; set; }

        public int? Profile_Id { get; set; }

        public int RegisterType { get; set; }

        public int VopVersion { get; set; }

        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }

        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }

        public virtual Profiles Profiles { get; set; }

        public virtual ICollection<Discussion> Discussion { get; set; }

        public virtual ICollection<DiscussionItem> DiscussionItem { get; set; }

        public virtual ICollection<EventOrganisator> EventOrganisator { get; set; }

        public virtual ICollection<EventParticipant> EventParticipant { get; set; }

        public virtual ICollection<EventParticipantHistory> EventParticipantHistory { get; set; }

        public virtual ICollection<EventSummary> EventSummary { get; set; }

        public virtual ICollection<Message> Message { get; set; }

        public virtual ICollection<MessageRecipient> MessageRecipient { get; set; }

        public virtual ICollection<OldPassword> OldPassword { get; set; }

        public virtual ICollection<Payment> Payment { get; set; }

        public virtual ICollection<PhotoAlbum> PhotoAlbum { get; set; }

        public virtual ICollection<TouristCardOwner> TouristCardOwner { get; set; }

        public virtual ICollection<TouristStampOwner> TouristStampOwner { get; set; }

        public virtual ICollection<AspNetRoles> AspNetRoles { get; set; }

        public virtual ICollection<OrganisationMember> OrganisationMember { get; set; }

        public virtual ICollection<EventInvitation> EventInvitation { get; set; }

        public virtual ICollection<EventCategory> EventCategories { get; set; }

        public virtual ICollection<AspNetUsersMainCategory> MainCategories { get; set; }
    }

    public enum LoginTypeEnum
    {
        NotConfirmed = 0,
        Confirmed = 1,
        Deleted = 2,
        OldSystemConfirmed = 3,
        OldSystemNotConfirmed = 4,
        Blocked = 5,
    }

    public enum RegisterTypeEnum
    {
        [Description("")]
        None = 0,
        [Description("Nevím")]
        DontKnow = 1,
        [Description("Kamarád/kamarádka")]
        Friends = 2,
        [Description("Na Facebooku")]
        Facebook = 3,
        [Description("Odkaz na webu")]
        Link = 4,
        [Description("Z doslechu")]
        HearSay = 5,
        [Description("Na akci")]
        Action = 6,
        [Description("Inzerát")]
        Advertisement = 7,
        [Description("Leták / vizitka")]
        Leaflet = 8,
        [Description("Tričko")]
        TShirt = 9,
    }
}
