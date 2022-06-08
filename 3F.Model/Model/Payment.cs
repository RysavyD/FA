namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Payment")]
    public partial class Payment : IPrimaryKey
    {
        public int Id { get; set; }

        public int Id_User { get; set; }

        public int Id_Event { get; set; }

        public string Note { get; set; }

        public double Amount { get; set; }

        public DateTime? CreateDate { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime? UpdateDate { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Event Event { get; set; }

        [Required]
        public Guid guid { get; set; }

        public virtual ICollection<EventParticipant> EventParticipant { get; set; }
    }

    public enum PaymentStatus
    {
        [Description("Aktivní")]
        Active = 0,
        [Description("Zaplacena")]
        Paid = 1,
        [Description("Čekající")]
        Waiting = 2,
        [Description("Zrušena")]
        Cancelled = 3,
    }
}
