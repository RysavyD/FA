using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace _3F.Model.Utils
{
    internal class RajceSeparator
    {
        public Photo[] Photos { get; private set; }
        
        public Photo CoverPhoto { get; private set; }

        public RajceSeparator()
        {
            Photos = new Photo[] { };
            CoverPhoto = new Photo();
        }

        public void Separate(string link)
        {
            string htmlCode;
            using (WebClient webClient = new WebClient())
            {
                htmlCode = webClient.DownloadString(link);
            }

            if (htmlCode != string.Empty)
            {
                var regex = new Regex("storage = (.*?);");
                var storage = regex.Match(htmlCode).Groups[1].Value
                    .Trim('"')
                    .Replace(@"\", "");

                regex = new Regex(@"albumCoverID = (\d+?);");
                var albumCoverId = regex.Match(htmlCode).Groups[1].Value;

                regex = new Regex(@"fileName""(.*?)width");
                var imageMatches = regex.Matches(htmlCode);
                var imageList = new HashSet<string>();
                foreach (Match match in imageMatches)
                {
                    imageList.Add(storage + "images/" + match.Groups[1].Value.Trim('"', ':', ',') + "?ver=0");
                }

                regex = new Regex($@"""{albumCoverId}(.*?)fileName(.*?)width");
                var ma = regex.Match(htmlCode);
                var albumCoverImg = storage + "images/" + ma.Groups[2].Value
                                        .Trim('"', ':', ',')
                                    + "?ver=0";

                Photos = imageList
                    .Select(i => new Photo()
                    {
                        Image = i,
                        Thumbnail = i.Replace("/images", "/thumb")
                    })
                    .ToArray();

                CoverPhoto = new Photo()
                {
                    Image = albumCoverImg,
                    Thumbnail = albumCoverImg.Replace("/images", "/thumb")
                };
            }
        }
    }
}
