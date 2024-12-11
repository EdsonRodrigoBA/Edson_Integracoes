using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Configuracoes;
using EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Interfaces;
using EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net;


namespace EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Services
{
    public class HandleAwsBucketS3Services : IHandleAwsBucketS3Services
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsAppSetingsConfiguration _awsAppSetings;
        public HandleAwsBucketS3Services(IAmazonS3 s3Client, IOptions<AwsAppSetingsConfiguration> settings)
        {
            _s3Client = s3Client;
            _awsAppSetings = settings.Value;
        }


        /// <summary>
        /// Cria um bucket S3
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        /// <exception cref="AmazonS3Exception"></exception>
        public async Task<bool> CreateBucketS3Async(string bucketName)
        {

            if (!await ExistsBucketS3Async(bucketName)) throw new AmazonS3Exception($"Bucket {bucketName} already exists.");

            var result = await _s3Client.PutBucketAsync(bucketName);
            return result.HttpStatusCode == HttpStatusCode.Created || result.HttpStatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> ExistsBucketS3Async(string bucketName)
        {
            return await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        }

        /// <summary>
        /// Lista todos os bucket s3 da conta
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetAllBuckeS3tAsync()
        {
            var data = await _s3Client.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => { return b.BucketName; });
            return buckets;
        }

        /// <summary>
        /// Apaga um bucket s3 pelo nome
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteBucketS3Async(string bucketName)
        {
            var result = await _s3Client.DeleteBucketAsync(bucketName);
            return result.HttpStatusCode == HttpStatusCode.Created || result.HttpStatusCode == HttpStatusCode.NoContent || result.HttpStatusCode == HttpStatusCode.OK;
        }


        public async Task<bool> DeleteFileAsync(string fileName, string versionId = "")
        {
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = _awsAppSetings.AwsBucketS3Name,
                Key = fileName
            };

            if (!string.IsNullOrEmpty(versionId))
                request.VersionId = versionId;

            var result = await _s3Client.DeleteObjectAsync(request);
            return true;
        }

        /// <summary>
        /// Realiza o donwload direto pelo bucket s3
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public async Task<byte[]> DownloadFileAsync(string file)
        {
            MemoryStream ms = null;

            try
            {
                GetObjectRequest getObjectRequest = new GetObjectRequest
                {
                    BucketName = _awsAppSetings.AwsBucketS3Name,
                    Key = file,

                };

                using (var response = await _s3Client.GetObjectAsync(getObjectRequest))
                {
                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        using (ms = new MemoryStream())
                        {
                            await response.ResponseStream.CopyToAsync(ms);
                        }
                    }
                }

                if (ms is null || ms.ToArray().Length < 1)
                    throw new FileNotFoundException(string.Format("The document '{0}' is not found", file));

                return ms.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Retorna uma url pre assinada com tempo de expiração para download do arquivo
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<FilesBucketS3ViewModel> DownloadFileGeneratePreSignedUrlAsync(string file)
        {
            var urlRequest = new GetPreSignedUrlRequest()
            {
                BucketName = _awsAppSetings.AwsBucketS3Name,
                Key = file,
                Expires = DateTime.UtcNow.AddMinutes(1),
                Verb = HttpVerb.GET,
                Protocol = Protocol.HTTP
            };
            return new FilesBucketS3ViewModel()
            {
                Name = file,
                PresignedUrl = new Uri(_s3Client.GetPreSignedURL(urlRequest)),
            };
        }
        /// <summary>
        /// Retorna uma url pre assinada com tempo de expiração para download do arquivo
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<FilesBucketS3ViewModel> UploadFileGeneratePreSignedUrlAsync(string file)
        {
            var urlRequest = new GetPreSignedUrlRequest()
            {
                BucketName = _awsAppSetings.AwsBucketS3Name,
                Key = file,
                Expires = DateTime.UtcNow.AddMinutes(1),
                Verb = HttpVerb.PUT,
                Protocol = Protocol.HTTP
            };
            return new FilesBucketS3ViewModel()
            {
                Name = file,
                PresignedUrl = new Uri( _s3Client.GetPreSignedURL(urlRequest)),
            };
        }

        /// <summary>
        /// Realizar o upload de um arquivos no bucket s3
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<bool> UploadFileAsync(IFormFile file)
        {
            try
            {

                if (file is null || file.Length <= 0)
                {
                    throw new FileNotFoundException("Informe o arquivo para armazenar");
                }
                if (!await ExistsBucketS3Async(_awsAppSetings.AwsBucketS3Name)) throw new AmazonS3Exception($"Bucket {_awsAppSetings.AwsBucketS3Name} not found.");

                var request = new PutObjectRequest()
                {
                    BucketName = _awsAppSetings.AwsBucketS3Name,
                    Key = string.IsNullOrEmpty(_awsAppSetings.AwsBucketS3FolderNameSaveFile) ? file.FileName : $"{_awsAppSetings.AwsBucketS3FolderNameSaveFile?.TrimEnd('/')}/{file.FileName}",
                    InputStream = file.OpenReadStream(),
                    
                };
                request.Metadata.Add("Content-Type", file.ContentType);
                var result = await _s3Client.PutObjectAsync(request);
                return result.HttpStatusCode == HttpStatusCode.Created || result.HttpStatusCode == HttpStatusCode.NoContent || result.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        

        public async Task<IEnumerable<FilesBucketS3ViewModel>> GetAllFilesBucketS3Async()
        {
            if (!await ExistsBucketS3Async(_awsAppSetings.AwsBucketS3Name)) throw new AmazonS3Exception($"Bucket {_awsAppSetings.AwsBucketS3Name} not found.");

            var request = new ListObjectsV2Request()
            {
                BucketName = _awsAppSetings.AwsBucketS3Name,
                Prefix = _awsAppSetings.AwsBucketS3FolderNameSaveFile
            };
            var result = await _s3Client.ListObjectsV2Async(request);
            var s3Objects = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = _awsAppSetings.AwsBucketS3Name,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };
                return new FilesBucketS3ViewModel()
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = new Uri(_s3Client.GetPreSignedURL(urlRequest)),
                };
            });

            return s3Objects;
        }
    }


}
