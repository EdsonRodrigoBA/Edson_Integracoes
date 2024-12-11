

using EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.ViewModels;
using Microsoft.AspNetCore.Http;

namespace EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Interfaces
{
    public interface IHandleAwsBucketS3Services
    {

        Task<bool> CreateBucketS3Async(string bucketName);
        Task<IEnumerable<string>> GetAllBuckeS3tAsync();
        Task<bool> DeleteBucketS3Async(string bucketName);


        Task<bool> UploadFileAsync(IFormFile file);

        Task<bool> DeleteFileAsync(string fileName, string versionId = "");
        Task<byte[]> DownloadFileAsync(string file);

        Task<FilesBucketS3ViewModel> DownloadFileGeneratePreSignedUrlAsync(string file);

        Task<IEnumerable<FilesBucketS3ViewModel>> GetAllFilesBucketS3Async();
        Task<FilesBucketS3ViewModel> UploadFileGeneratePreSignedUrlAsync(string file);
    }
}
