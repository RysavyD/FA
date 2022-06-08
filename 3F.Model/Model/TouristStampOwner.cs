namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TouristStampOwner")]
    public partial class TouristStampOwner : IPrimaryKey, ICollectorOwner
    {
        public int Id { get; set; }

        public int Id_Owner { get; set; }

        [Column("Id_Stamp")]
        public int Id_Item { get; set; }

        public ItemOwnerStatus Status { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual TouristStamp TouristStamp { get; set; }
    }
}
