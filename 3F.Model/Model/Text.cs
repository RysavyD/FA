namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Text")]
    public partial class Text : IPrimaryKey
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(150)]
        public string EditPermissions { get; set; }

        [StringLength(150)]
        public string ViewPermissions { get; set; }

        [StringLength(50)]
        public string OriginalUrl { get; set; }

        [StringLength(50)]
        public string Icon { get; set; }
    }
}
