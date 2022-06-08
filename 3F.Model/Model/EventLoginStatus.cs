namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EventLoginStatus : IPrimaryKey
    {
        public EventLoginStatus()
        {
            EventParticipant = new HashSet<EventParticipant>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public virtual ICollection<EventParticipant> EventParticipant { get; set; }
    }
}
