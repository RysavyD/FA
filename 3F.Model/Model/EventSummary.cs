namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventSummary")]
    public partial class EventSummary : IPrimaryKey
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Name { get; set; }

        [Required]
        [StringLength(300)]
        public string Perex { get; set; }

        [Required]
        public string Description { get; set; }

        public int Id_Event { get; set; }

        public int Id_Discussion { get; set; }

        public int Id_User { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Discussion Discussion { get; set; }

        public virtual Event Event { get; set; }
    }
}
