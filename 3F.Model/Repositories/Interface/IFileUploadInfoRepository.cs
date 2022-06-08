using System.Collections.Generic;
using _3F.BusinessEntities;

namespace _3F.Model.Repositories
{
    public interface IFileUploadInfoRepository
    {
        IEnumerable<FileUploadInfo> GetFiles();
        FileUploadInfo GetById(int id);
        void Add(FileUploadInfo item);
        void Remove(int id);
    }
}
