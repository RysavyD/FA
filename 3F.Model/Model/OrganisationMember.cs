namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrganisationMember")]
    public partial class OrganisationMember : IPrimaryKey
    {
        public int Id { get; set; }

        public int Id_User { get; set; }

        public int Id_Organisation { get; set; }

        public DateTime From { get; set; }

        public DateTime? To { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Organisation Organisation { get; set; }
    }
}
