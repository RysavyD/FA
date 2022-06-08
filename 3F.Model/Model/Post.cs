namespace _3F.Model.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Post")]
    public partial class Post : IPrimaryKey
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Required]
        [StringLength(150)]
        public string HtmlName { get; set; }

        [Required]
        public string Content { get; set; }

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
