using System.IO;
using System.Web;
using System.Web.Mvc;
using _3F.Model.Repositories;

namespace _3F.Web.Controllers
{
    public class FileController : Controller
    {
        private readonly IFileUploadInfoRepository _fileUploadInfoRepository;

        public FileController(IFileUploadInfoRepository fileUploadInfoRepository)
        {
            _fileUploadInfoRepository = fileUploadInfoRepository;
        }

        public ActionResult Download(int id)
        {
            var fileInfo = _fileUploadInfoRepository.GetById(id);
            string path = Path.Combine(Server.MapPath("~/App_Data/PostUpload"), fileInfo.Name);

            var contentType = MimeMapping.GetMimeMapping(fileInfo.Name);

            return File(path, contentType, fileInfo.Name);
        }
    }
}