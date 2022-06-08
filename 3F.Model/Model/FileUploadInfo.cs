namespace _3F.Model.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class FileUploadInfo : IPrimaryKey
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
