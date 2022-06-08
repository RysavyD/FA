using _3F.Log;
using System;

namespace _3F.Model.Utils
{
    public class Photo
    {
        public string Thumbnail { get; set; }
        public string Image { get; set; }
    }

    public class Album
    {
        public Photo[] Photos { get; private set; }

        public Photo CoverPhoto { get; private set; }

        public Album(Photo[] photos, Photo coverPhoto)
        {
            Photos = photos;
            CoverPhoto = coverPhoto;
        }
    }

    internal class PhotoManager
    {
        public Photo[] Photos{ get; private set; }

        public Photo CoverPhoto { get; private set; }

        public PhotoManager(string link, ILogger logger)
        {
            var rajceSeparator = new RajceSeparator();

            try
            {
                rajceSeparator.Separate(link);
            }
            catch (Exception e)
            {
                logger.LogInfo($"Nezdařilo se načtení fotoalba '{link}'", "PhotoManager");
                logger.LogException(e, "PhotoManager");
            }

            Photos = rajceSeparator.Photos;
            CoverPhoto = rajceSeparator.CoverPhoto;
        }
    }
}