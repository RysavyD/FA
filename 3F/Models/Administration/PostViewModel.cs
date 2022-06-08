using _3F.BusinessEntities;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace _3F.Web.Models.Administration
{
    public class PostViewModel
    {
        public int Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(150)]
        public string HtmlName { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public string EditPermissions { get; set; }
        public string ViewPermissions { get; set; }
        public string OriginalUrl { get; set; }
        public string Icon { get; set; }

        public PostViewModel() { }

        public PostViewModel(Post postEntity)
        {
            Id = postEntity.Id;
            Name = postEntity.Name;
            HtmlName = postEntity.HtmlName;
            Content = postEntity.Content;
            EditPermissions = postEntity.EditPermissions;
            ViewPermissions = postEntity.ViewPermissions;
            OriginalUrl = postEntity.OriginalUrl;
            Icon = postEntity.Icon;
        }

        public Post ToBusinessEntity()
        {
            return new Post()
            {
                Id = Id,
                Name = Name,
                HtmlName = HtmlName,
                Content = Content,
                EditPermissions = EditPermissions,
                ViewPermissions = ViewPermissions,
                OriginalUrl = OriginalUrl,
                Icon = Icon,
            };
        }
    }
}