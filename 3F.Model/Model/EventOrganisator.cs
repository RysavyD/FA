namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EventOrganisator")]
    public partial class EventOrganisator : IPrimaryKey
    {
        public int Id { get; set; }

        public int Id_Event { get; set; }

        public int Id_User { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Event Event { get; set; }
    }
}
