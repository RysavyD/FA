using _3F.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3F.Web.Models
{
    public class EventSummaryViewModel : EventSummaryBasicViewModel
    {
        public string EventName { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string HtmlName { get; set; }
        public int Id_Discussion { get; set; }
        public string EventStart { get; set; }
        public string EventStop { get; set; }
        public List<PhotoAlbumViewModel> Photos { get; set; }
        public bool HasPhoto { get; set; }
    }

    public class EventSummaryBasicViewModel : BaseViewModel
    {
        public User Author { get; set; }
        [MaxLength(300)]
        public string Name { get; set; }
        [MaxLength(300)]
        public string Perex { get; set; }
    }
}