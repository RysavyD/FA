namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhotoAlbum")]
    public partial class PhotoAlbum : IPrimaryKey
    {
        public int Id { get; set; }

        public int Id_User { get; set; }

        public int Id_Event { get; set; }

        [Required]
        [StringLength(200)]
        public string AlbumLink { get; set; }

        [StringLength(200)]
        public string CoverPhotoLink { get; set; }

        public int PhotoCount { get; set; }

        public int Id_Discussion { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Discussion Discussion { get; set; }

        public virtual Event Event { get; set; }
    }
}
