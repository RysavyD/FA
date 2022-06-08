using System;
using System.ComponentModel.DataAnnotations;

namespace _3F.Web.Models.Administration
{
    public class ChangeToPaidEventViewModel : BaseViewModel
    {
        public string EventName { get; set; }
        public int Id { get; set; }
        [Required]
        public DateTime LastPaidDate { get; set; }
        [Required]
        public string Contact { get; set; }
        [Required]
        public int  Costs { get; set; }
        [Required]
        public string CostsDescription { get; set; }
        [Required]
        public string Link { get; set; }
    }
}