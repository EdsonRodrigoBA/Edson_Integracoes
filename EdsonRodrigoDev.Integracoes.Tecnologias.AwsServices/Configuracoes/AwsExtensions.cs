using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Configuracoes
{
    public static class AwsExtensions
    {
        public static void AddAwsServices(this IServiceCollection services, IConfiguration configuration)
        {
           services.AddSingleton<IAmazonS3>(sc =>
            {
                var configuration = sc.GetRequiredService<IConfiguration>();
                var ServiceURLBucketS3 = configuration.GetSection("AwsAppSetingsConfiguration:ServiceURLBucketS3").Value;
                var AwsAccessKey = configuration.GetSection("AwsAppSetingsConfiguration:AwsAccessKey").Value;
                var AwsSecretKey = configuration.GetSection("AwsAppSetingsConfiguration:AwsSecretKey").Value;



                if (ServiceURLBucketS3 is null)
                {
                    return new AmazonS3Client();
                }
                else
                {
                    return new AmazonS3Client(AwsAccessKey, AwsSecretKey, new AmazonS3Config()
                    {
                        ForcePathStyle = true,
                        ServiceURL = ServiceURLBucketS3,

                    });
                }
            });
        }
    }
}
