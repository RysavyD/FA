using System.ComponentModel.DataAnnotations;

namespace _3F.Model.Model
{
    public class KnowFrom : IPrimaryKey
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public bool Visible { get; set; }
    }
}
