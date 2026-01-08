using Astralis_API.Services.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Astralis_API.Services.Implementations
{
    public class BlobStorageService : IUploadService
    {
        private readonly string _connectionString;

        public BlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"]!;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string containerName, string folderName = "")
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();

            var extension = Path.GetExtension(file.FileName);

            string blobName = string.IsNullOrEmpty(folderName)
                ? $"{Guid.NewGuid()}{extension}"
                : $"{folderName}/{Guid.NewGuid()}{extension}";

            var blobClient = containerClient.GetBlobClient(blobName);

            var blobHttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType };

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(string fileUrl, string containerName)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            try
            {
                var blobServiceClient = new BlobServiceClient(_connectionString);

                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                var uri = new Uri(fileUrl);

                string path = uri.AbsolutePath;

                string containerSegment = $"/{containerName}/";
                int index = path.IndexOf(containerSegment);

                string blobName = "";
                if (index >= 0)
                {
                    blobName = path.Substring(index + containerSegment.Length);
                }
                else
                {
                    blobName = Path.GetFileName(uri.LocalPath);
                }

                blobName = System.Net.WebUtility.UrlDecode(blobName);

                var blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur suppression Blob : {ex.Message}");
            }
        }
    }
}