using System.Data.Entity.Spatial;

namespace _3F.Web.Models
{
    public class TuristickeZnamkyModel
    {
        public int ItemNumber { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string Coordinates { get; set; }

        public bool AlreadyHaven { get; set; }
    }
}