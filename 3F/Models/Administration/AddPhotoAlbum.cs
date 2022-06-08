using System.Collections.Generic;
using System.Web.Mvc;

namespace _3F.Web.Models.Administration
{
    public class AddPhotoAlbum
    {
        public IEnumerable<SelectListItem> Events { get; set; }
        public string HtmlName { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }
        public string UserName { get; set; }

        public string Link { get; set; }
    }
}