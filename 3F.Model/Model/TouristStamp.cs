namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TouristStamp")]
    public partial class TouristStamp : IPrimaryKey, ICollectorItem
    {
        public TouristStamp()
        {
            TouristStampOwner = new HashSet<TouristStampOwner>();
        }

        public int Id { get; set; }

        [Column("StampId")]
        public int ItemNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DbGeography Position { get; set; }

        [StringLength(150)]
        public string ImageUrl { get; set; }

        public virtual ICollection<TouristStampOwner> TouristStampOwner { get; set; }
    }

    public interface ICollectorItem
    {
        int ItemNumber { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        DbGeography Position { get; set; }
        string ImageUrl { get; set; }
    }
}
