using BlobStorageZipArchive.Exceptions;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore.Internal;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace BlobStorageZipArchive.Services
{
    public class BlobStorageZipArchiveService : IBlobStorageZipArchiveService
    {
        private readonly CloudBlobClient _cloudBlobClient;

        public BlobStorageZipArchiveService(CloudBlobClient cloudBlobClient)
        {
            _cloudBlobClient = cloudBlobClient;
        }

        public async Task GenerateZipArchiveStream(Stream stream, string containerName, string relativeAddress)
        {
            var cloudBlobContainer = _cloudBlobClient.GetContainerReference(containerName);
            // make sure container exists
            var exists = await cloudBlobContainer.ExistsAsync();
            if (!exists)
            {
                throw new BlobStorageZipArchiveException("cannot find container");
            }

            var cloudBlobDirectory = cloudBlobContainer.GetDirectoryReference(relativeAddress);
            // make sure there are blobs under the relative address
            var results = await cloudBlobDirectory.ListBlobsSegmentedAsync(null);
            if (!results.Results.Any())
            {
                throw new BlobStorageZipArchiveException("cannot find any file under the relative address");
            }

            // generate zip archive stream
            using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                await AddZipArchiveEntryFromCloudBlobDirectory(cloudBlobDirectory, zipArchive);
            }

            // reset stream position
            stream.Seek(0, SeekOrigin.Begin);
        }

        private static async Task AddZipArchiveEntryFromCloudBlobDirectory(CloudBlobDirectory cloudBlobDirectory, ZipArchive zipArchive)
        {
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var blobResultSegment = await cloudBlobDirectory.ListBlobsSegmentedAsync(blobContinuationToken);
                blobContinuationToken = blobResultSegment.ContinuationToken;
                foreach (var listBlobItem in blobResultSegment.Results)
                {
                    switch (listBlobItem)
                    {
                        case CloudBlobDirectory subCloudBlobDirectory:
                            await AddZipArchiveEntryFromCloudBlobDirectory(subCloudBlobDirectory, zipArchive);
                            break;
                        case CloudBlockBlob cloudBlockBlob:
                        {
                            var entry = zipArchive.CreateEntry(cloudBlockBlob.Name);
                            if (cloudBlockBlob.Properties.LastModified.HasValue)
                            {
                                entry.LastWriteTime = cloudBlockBlob.Properties.LastModified.Value;
                            }
                            using (var entryStream = entry.Open())
                            {
                                await cloudBlockBlob.DownloadToStreamAsync(entryStream);
                            }

                            break;
                        }
                    }
                }
            } while (blobContinuationToken != null);
        }
    }
}
