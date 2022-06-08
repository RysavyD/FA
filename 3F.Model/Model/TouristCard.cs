namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TouristCard")]
    public partial class TouristCard : IPrimaryKey, ICollectorItem
    {
        public TouristCard()
        {
            TouristCardOwner = new HashSet<TouristCardOwner>();
        }

        public int Id { get; set; }

        [Column("CardId")]
        public int ItemNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DbGeography Position { get; set; }

        [StringLength(150)]
        public string ImageUrl { get; set; }

        public virtual ICollection<TouristCardOwner> TouristCardOwner { get; set; }
    }
}
