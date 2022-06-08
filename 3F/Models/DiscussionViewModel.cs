using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _3F.Web.Models
{
    public class DiscussionViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string HtmlName { get; set; }
        [Required]
        public string Perex { get; set; }
        public User Author { get; set; }
        public int ItemsCount { get; set; }
        public DateTime? LastItemDateTime { get; set; }
    }

    public class DiscussionItemViewModel
    {
        public string Text { get; set; }
        public User Author { get; set; }
        public string DateTime { get; set; }
    }

    public class DiscussionItemsViewModel
    {
        public List<DiscussionItemViewModel> Items { get; set; }
        public int Page { get; set; }
        public int TotalItems { get; set; }

        public DiscussionItemsViewModel()
        {
            Items = new List<DiscussionItemViewModel>();
        }
    }
}