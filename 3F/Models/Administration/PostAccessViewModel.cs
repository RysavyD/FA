using System.Collections.Generic;
using System.Web.Mvc;

namespace _3F.Web.Models.Administration
{
    public class PostAccessViewModel
    {
        public string Name { get; set; }
        public IList<string> SelectedEditRights { get; set; }
        public IList<string> SelectedViewRights { get; set; }
        public IList<SelectListItem> AvailableEditRights { get; set; }
        public IList<SelectListItem> AvailableViewRights { get; set; }
        public int Id { get; set; }
    }
}