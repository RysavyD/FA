using _3F.BusinessEntities;
using System;

namespace _3F.Web.Models.Administration
{
    public class FileUploadInfoViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public DateTime CreationDate { get; set; }

        public string Length { get; set; }
        public string Description { get; set; }

        public FileUploadInfoViewModel(FileUploadInfo entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Path = entity.Path;
            CreationDate = entity.CreationDate;
            Description = entity.Description;
        }
    }
}