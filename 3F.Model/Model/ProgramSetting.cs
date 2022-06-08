namespace _3F.Model.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProgramSetting")]
    public partial class ProgramSetting : IPrimaryKey
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Key { get; set; }

        [Required]
        [StringLength(200)]
        public string Value { get; set; }
    }
}