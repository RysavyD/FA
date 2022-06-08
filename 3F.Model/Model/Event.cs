namespace _3F.Model.Model
{
    using _3F.BusinessEntities.Enum;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event : IPrimaryKey, IHtmlName
    {
        public Event()
        {
            EventOrganisator = new HashSet<EventOrganisator>();
            EventParticipant = new HashSet<EventParticipant>();
            EventParticipantHistory = new HashSet<EventParticipantHistory>();
            EventSummary = new HashSet<EventSummary>();
            Payment = new HashSet<Payment>();
            PhotoAlbum = new HashSet<PhotoAlbum>();
            EventInvitation = new HashSet<EventInvitation>();
            EventCategories = new HashSet<EventCategory>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        public string Perex { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(0, 32000)]
        public int Capacity { get; set; }

        [StringLength(150)]
        public string Place { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime StopDateTime { get; set; }

        public DateTime? LastSignINDateTime { get; set; }

        public DateTime? MeetDateTime { get; set; }

        [StringLength(150)]
        public string MeetPlace { get; set; }

        [StringLength(150)]
        public string Contact { get; set; }

        public int Price { get; set; }

        [Required]
        [StringLength(200)]
        public string HtmlName { get; set; }

        public int Id_Discussion { get; set; }

        public string Link { get; set; }

        [StringLength(50)]
        public string BankAccount { get; set; }

        public bool MayBeLogOn { get; set; }

        public DateTime? LastPaidDateTime { get; set; }

        public int ExternParticipants { get; set; }

        [Range(0, 32000)]
        public int MinimumParticipants { get; set; }

        public string Photo { get; set; }

        public int AccountSymbol { get; set; }

        public EventStateEnum State { get; set; }

        public int Costs { get; set; }

        public string CostsDescription { get; set; }

        public virtual Discussion Discussion { get; set; }

        public virtual EventTypeEnum EventType { get; set; }

        public virtual MainCategory MainCategory { get; set; } 

        public virtual bool IsStay { get; set; }

        public virtual ICollection<EventOrganisator> EventOrganisator { get; set; }

        public virtual ICollection<EventParticipant> EventParticipant { get; set; }

        public virtual ICollection<EventParticipantHistory> EventParticipantHistory { get; set; }

        public virtual ICollection<EventSummary> EventSummary { get; set; }

        public virtual ICollection<Payment> Payment { get; set; }

        public virtual ICollection<PhotoAlbum> PhotoAlbum { get; set; }

        public virtual ICollection<EventInvitation> EventInvitation { get; set; }

        public virtual ICollection<EventCategory> EventCategories { get; set; }
    }

    public enum EventStateEnum
    {
        [Description("Aktivní")]
        Active = 0,
        [Description("Smazána")]
        Deleted = 1,
        [Description("Ke schválení")]
        ForAcceptance = 2,
        [Description("Zamítnuto")]
        Rejected = 3,
        [Description("Rozděláno")]
        InWork = 4,
    }

    public enum EventTypeEnum
    {
        Bezna = 1,
        PlacenaSdruzenim = 2,
        OficialniSdruzeni = 3,
        Soukroma = 4,
        TipNaAkci = 5,
        Komercni = 6,
    }
}
