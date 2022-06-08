using System;

namespace _3F.BusinessEntities
{
    public class FileUploadInfo : AbstractEntity
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
    }
}
