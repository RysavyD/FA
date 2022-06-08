namespace _3F.BusinessEntities.Akce
{
    public class PhotoAlbum
    {
        public int Id { get; set; }
        public string CoverPhotoLink { get; set; }
        public int PhotoCount { get; set; }
        public User Author { get; set; }
    }
}
