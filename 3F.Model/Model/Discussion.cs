namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Discussion")]
    public partial class Discussion : IPrimaryKey, IHtmlName
    {
        public Discussion()
        {
            DiscussionItem = new HashSet<DiscussionItem>();
            Event = new HashSet<Event>();
            EventSummary = new HashSet<EventSummary>();
            PhotoAlbum = new HashSet<PhotoAlbum>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsAlone { get; set; }

        [Required]
        public string HtmlName { get; set; }

        public string Perex { get; set; }

        public int? Id_Author { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual ICollection<DiscussionItem> DiscussionItem { get; set; }

        public virtual ICollection<Event> Event { get; set; }

        public virtual ICollection<EventSummary> EventSummary { get; set; }

        public virtual ICollection<PhotoAlbum> PhotoAlbum { get; set; }
    }
}
