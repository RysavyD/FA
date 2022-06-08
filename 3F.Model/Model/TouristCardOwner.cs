namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TouristCardOwner")]
    public partial class TouristCardOwner : IPrimaryKey, ICollectorOwner
    {
        public int Id { get; set; }

        public int Id_Owner { get; set; }

        [Column("Id_Card")]
        public int Id_Item { get; set; }

        public ItemOwnerStatus Status { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual TouristCard TouristCard { get; set; }
    }

    public enum ItemOwnerStatus
    {
        [Description("Vlastním")]
        Have = 1,
        [Description("Byl jsem, ale nevlastním")]
        Visited = 2,
        [Description("Nevlastním")]
        NotHave = 3,
        [Description("Chtěl bych")]
        WantTo = 4,
    }

    public interface ICollectorOwner
    {
        int Id_Item { get; set; }
        int Id_Owner { get; set; }
        ItemOwnerStatus Status { get; set; }
        AspNetUsers AspNetUsers { get; set; }
    }
}
