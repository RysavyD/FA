using _3F.Model.Model;
using _3F.Model.Utils;
using _3F.Model.Extensions;

namespace _3F.Web.Models
{
    public class PhotoAlbumViewModel
    {
        public User User { get; set; }
        public string EventName { get; set; }
        public string EventHtml { get; set; }
        public string EventStart { get; set; }
        public string EventStop { get; set; }
        public int Id { get; set; }
        public int PhotoCount { get; set; }
        public string CoverPhotoLink { get; set; }

        public PhotoAlbumViewModel(PhotoAlbum album)
        {
            User = new User(album.AspNetUsers);
            EventName = album.Event.Name;
            EventHtml = album.Event.HtmlName;
            EventStart = album.Event.StartDateTime.ToDayDateTimeString();
            EventStop = album.Event.StopDateTime.ToDayDateTimeString();
            Id = album.Id;
            PhotoCount = album.PhotoCount;
            CoverPhotoLink = album.CoverPhotoLink;
        }
    }

    public class PhotoAlbumPhotos : BaseViewModel
    {
        public User User { get; set; }
        public string EventName { get; set; }
        public string EventHtml { get; set; }
        public Photo[] Photos { get; set; }
        public int Id_Discussion { get; set; }
    }

    public class CreatePhotoAlbum : BaseViewModel
    {
        public string Link { get; set; }
    }
}