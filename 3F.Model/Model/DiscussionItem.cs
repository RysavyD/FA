namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DiscussionItem")]
    public partial class DiscussionItem : IPrimaryKey
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public int Id_Discussion { get; set; }

        public int Id_Author { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual Discussion Discussion { get; set; }
    }
}
