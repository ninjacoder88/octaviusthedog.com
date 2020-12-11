using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OctaviusTheDog.DataAccess.AzureStorage
{
    public interface IAzureStorageRepository
    {
        Task<string> UploadImageAsync(string containerName, string fileName, byte[] imageBytes);

        Task<List<ImageBlob>> GetImagesAsync(string containerName, string prefix, int segmentSize, int pageNumber);

        ImageBlob GetImage(string containerName, string blobName);
    }

    public class AzureStorageRepository : IAzureStorageRepository
    {
        public AzureStorageRepository(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadImageAsync(string containerName, string fileName, byte[] imageBytes)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            using(var memoryStream = new MemoryStream(imageBytes))
            {
                var response = await blobContainerClient.UploadBlobAsync(fileName, memoryStream);
                return response.Value.ETag.ToString();
            }
        }

        public ImageBlob GetImage(string containerName, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(blobName);
            return new ImageBlob(blobName, blobClient.Uri.ToString());
        }

        public async Task<List<ImageBlob>> GetImagesAsync(string containerName, string prefix, int segmentSize, int pageNumber)
        {
            string continuationToken = string.Empty;
            int currentPage = 1;
            //int? segmentSize = 12;

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            List<ImageBlob> images = new List<ImageBlob>();
            do
            {
                var blobs = blobContainerClient.GetBlobsAsync(prefix: prefix).AsPages(continuationToken, segmentSize);

                await foreach (Page<BlobItem> page in blobs)
                {
                    if(pageNumber == currentPage)
                    {
                        foreach(BlobItem blobItem in page.Values)
                        {
                            var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);

                            images.Add(new ImageBlob(blobItem.Name, blobClient.Uri.ToString()));
                        }
                    }
                    else
                    {
                        continuationToken = page.ContinuationToken;
                    }

                    currentPage++;
                }
            } while (continuationToken != string.Empty && currentPage < pageNumber);

            return images;
        }

        private BlobServiceClient _blobServiceClient;
    }

    public class ImageBlob
    {
        public ImageBlob(string blobName, string url)
        {
            BlobName = blobName;
            Url = url;
        }

        public string BlobName { get; }

        public string Url { get; }
    }
}
