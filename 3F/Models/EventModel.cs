using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using _3F.BusinessEntities.Enum;
using _3F.Model.Model;
using _3F.Web.Models.EventModels;

namespace _3F.Web.Models
{
    public class SimpleEventModel : BaseViewModel
    {
        [Required,StringLength(150)]
        public string Name { get; set; }
        [Required, StringLength(150)]
        public string Perex { get; set; }

        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime Stop { get; set; }
        public string HtmlName { get; set; }
    }

    [Serializable]
    public class EventModel : SimpleEventModel
    {
        [Required, AllowHtml]
        public string Description { get; set; }
        public int Capacity { get; set; }
        [Required]
        public string Place { get; set; }
        public DateTime? MeetTime { get; set; }
        public DateTime? LastSignTime { get; set; }
        public DateTime? LastPaidTime { get; set; }
        public string MeetPlace { get; set; }
        public string OrganisatorNames { get; set; }
        public IEnumerable<User> Organisators { get; set; }
        public string Contact { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Price { get; set; }
        public string Link { get; set; }
        public bool MayBeAvalaible { get; set; }
        public string Photo { get; set; }
        public EventTypeEnum EventType { get; set; }
        public int Id_Discussion { get; set; }
        [Range(0, int.MaxValue)]
        public int MinimumParticipants { get; set; }
        public int Costs { get; set; }
        [AllowHtml]
        public string CostsDescription { get; set; }
        public string OldHtml { get; set; }
        public EventEditMode EventEditMode { get; set; }
        public IList<Category> Categories { get; set; }
        public int[] CategoryIds { get; set; }
        public string Image { get; set; }
        [Required]
        public MainCategory MainCategory { get; set; }
        public bool IsStay { get; set; }

        public EventModel()
        {
            Organisators = new List<User>();
            Categories = new List<Category>();
            CategoryIds = new int[] {};
        }


        public string PriceString
        {
            get
            { return (Price == 0) ? "Akce je zdarma" : string.Format("{0} Kč", Price); }
        }

        public string CapacityString
        {
            get { return Capacity == 0 ? "Bez omezení" : Capacity.ToString(); }
        }

        public static EventModel GetEventModel(EventTypeEnum eventType)
        {
            return new EventModel() { EventType = eventType };
        }        
    }

    public enum EventEditMode
    {
        Create = 0,
        Edit = 1,
    }
}