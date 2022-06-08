namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventParticipantHistory")]
    public partial class EventParticipantHistory : IPrimaryKey
    {
        public int Id { get; set; }

        public int Id_User { get; set; }

        public int Id_Event { get; set; }

        public int Id_Participant { get; set; }

        public EventLoginEnum OldEventLoginStatus { get; set; }

        public EventLoginEnum NewEventLoginStatus { get; set; }

        public DateTime Time { get; set; }

        public bool IsExternal { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Event Event { get; set; }
    }
}
