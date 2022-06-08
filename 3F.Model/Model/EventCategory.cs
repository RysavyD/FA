namespace _3F.Model.Model
{
    using _3F.BusinessEntities.Enum;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EventCategory")]
    public class EventCategory : IPrimaryKey, IHtmlName
    {
        public EventCategory()
        {
            Events = new HashSet<Event>();
            AspNetUsers = new HashSet<AspNetUsers>();
        }

        [Required]
        public int Id { get; set; }

        [Required, StringLength(100, ErrorMessage = "Název kategorie může mít max. 100 znaků")]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string HtmlName { get; set; }

        [Required]
        public MainCategory MainCategory { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}