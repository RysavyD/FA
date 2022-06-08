using System;
using System.Net;
using System.Runtime.Caching;
using _3F.Model.Utils;
using Newtonsoft.Json;

namespace _3F.Model.Service
{
    public interface IRajceService
    {
        Album GetAlbum(string url);
    }

    public class RajceService : IRajceService
    {
        private static readonly MemoryCache Cache = new MemoryCache("RajceMemoryCache");
        private int expirationInSeconds = 12 * 60 * 60; // 12 hours

        public Album GetAlbum(string url)
        {
            var value = Cache.Get(url);
            if (value is Album variable)
            {
                return variable;
            }

            using (var client = new WebClient())
            {
                var data = client.DownloadString($"http://helpwebapp.drysavy.cz/api/fungujeme/album?url={url}");

                var album = JsonConvert.DeserializeObject<Album>(data);

                CacheItemPolicy policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(expirationInSeconds)
                };

                Cache.Set(url, album, policy);

                return album;
            }
        }
    }

    public class OldRajceService : IRajceService
    {
        public Album GetAlbum(string url)
        {
            var separator = new RajceSeparator();
            separator.Separate(url);

            return new Album(separator.Photos, separator.CoverPhoto);
        }
    }
}
