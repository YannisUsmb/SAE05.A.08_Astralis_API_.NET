namespace Astralis_API.Services.Interfaces
{
    public interface IUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string containerName, string folderName = "");
        Task DeleteFileAsync(string fileUrl, string containerName);
    }
}
