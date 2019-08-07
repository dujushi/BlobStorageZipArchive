using BlobStorageZipArchive.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlobStorageZipArchive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageZipArchiveController : ControllerBase
    {
        private readonly IBlobStorageZipArchiveService _blobStorageZipArchiveService;
        public BlobStorageZipArchiveController(IBlobStorageZipArchiveService blobStorageZipArchiveService)
        {
            _blobStorageZipArchiveService = blobStorageZipArchiveService;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var memoryStream = new MemoryStream();
            const string containerName = "containername";
            const string relativeAddress = "";
            await _blobStorageZipArchiveService.GenerateZipArchiveStream(memoryStream, containerName, relativeAddress);
            return File(memoryStream, "application/octet-stream", "fileDownloadName.zip");
        }
    }
}
