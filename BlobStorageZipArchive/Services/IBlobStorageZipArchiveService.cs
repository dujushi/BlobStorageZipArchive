using System.IO;
using System.Threading.Tasks;

namespace BlobStorageZipArchive.Services
{
    public interface IBlobStorageZipArchiveService
    {
        Task GenerateZipArchiveStream(Stream stream, string containerName, string relativeAddress);
    }
}
