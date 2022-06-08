using System.ComponentModel.DataAnnotations;

namespace _3F.Web.Models
{
    public class TextViewModel : BaseViewModel
    {
        [DataType(DataType.MultilineText), Required]
        public string Text { get; set; }
        [Required]
        public string Key { get; set; }
        public string OriginalUrl { get; set; }
    }
}