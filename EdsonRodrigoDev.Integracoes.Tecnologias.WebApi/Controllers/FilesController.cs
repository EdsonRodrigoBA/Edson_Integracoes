using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EdsonRodrigoDev.Integracoes.Tecnologias.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {

        private readonly IHandleAwsBucketS3Services _handleAwsBucketS3Services;

        public FilesController(IHandleAwsBucketS3Services handleAwsBucketS3Services)
        {
            this._handleAwsBucketS3Services = handleAwsBucketS3Services;
        }

        [HttpPost("UploadDocumentBucketS3")]
        public async Task<IActionResult> UploadDocumentBucketS3(IFormFile formFile)
        {
            if (formFile is null || formFile.Length <= 0)
                return BadRequest(new { message = "file is required to upload" });

            await _handleAwsBucketS3Services.UploadFileAsync(formFile);

            return Ok("Upload success");
        }


        [HttpDelete("DeletetDocumentBucketS3/{fileKey}")]
        public IActionResult DeletetDocumentBucketS3(string fileKey)
        {
            try
            {
                if (string.IsNullOrEmpty(fileKey))
                    return BadRequest(new { message = "The 'fileKey' parameter is required" });


                _handleAwsBucketS3Services.DeleteFileAsync(fileKey);

                return Ok(string.Format("The document '{0}' is deleted successfully", fileKey));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("get-by-key/{fileKey}")]
        public async Task<IActionResult> GetFileByKeyAsync(string fileKey)
        {

            var document = await _handleAwsBucketS3Services.DownloadFileAsync(fileKey);
            return File(document, "application/octet-stream", fileKey);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllFilesAsync()
        {

            var documents = await _handleAwsBucketS3Services.GetAllFilesBucketS3Async();
            return Ok(documents);

        }

        [HttpGet("GenerateUrlDownloadFilePreSigned/{fileKey}")]
        public async Task<IActionResult> GetFilePreSignedUrlAsync(string fileKey)
        {


            var document = await _handleAwsBucketS3Services.DownloadFileGeneratePreSignedUrlAsync(fileKey);
            return Ok(document);



        }
        [HttpGet("GenerateUrlUploadFilePreSigned/{fileKey}")]
        public async Task<IActionResult> GenerateUrlUploadFilePreSigned(string fileKey)
        {


            var document = await _handleAwsBucketS3Services.UploadFileGeneratePreSignedUrlAsync(fileKey);
            return Ok(document);

        }
        [HttpPost("UploadToS3ByPreSignUrl/")]
        public async Task<IActionResult> UploadToS3ByPreSignUrl(Teste teste)
        {
            //var fileStreamResponse = await new HttpClient().PutAsync(
            //    new Uri(preSignedUrl),
            //    new StreamContent(formFile.OpenReadStream()));
            //return Ok( fileStreamResponse.EnsureSuccessStatusCode());

            HttpWebRequest httpRequest = WebRequest.Create(teste.preSignedUrl) as HttpWebRequest;
            httpRequest.Method = "PUT";
            using (Stream dataStream = httpRequest.GetRequestStream())
            {
                var buffer = new byte[8000];
                using (FileStream fileStream = new FileStream(teste.formFile.FileName, FileMode.Open, FileAccess.Read))
                {
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        dataStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            HttpWebResponse response = httpRequest.GetResponse() as HttpWebResponse;

            return Ok(response);
        }
    }

    public class Teste
    {
        public IFormFile formFile  { get; set; }
        public string preSignedUrl { get; set; }
    }
}
