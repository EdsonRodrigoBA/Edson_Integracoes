using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdsonRodrigoDev.Integracoes.Tecnologias.AwsServices.Configuracoes
{

    public class AwsAppSetingsConfiguration
    {


        /// <summary>
        /// Url HTTP da Queue SQS caso for utilizar SQS
        /// </summary>
        public Uri AwsQueueUrl { get; set; }

        /// <summary>
        /// Nome da Queue SQS
        /// </summary>
        public string AwsQueueName { get; set; }

        /// <summary>
        /// Secrete Key do usuario que tem acesso aos recusos aws: SQS, SNS, S3, etc..
        /// </summary>
        public string AwsSecretKey { get; set; }

        /// <summary>
        /// Chave de acesso do usuario(IAM) configurado através do aws cli que tem acesso aos recusos aws: SQS, SNS, S3
        /// </summary>
        public string AwsAccessKey { get; set; }

        /// <summary>
        /// Chave de acesso do usuario(IAM) que tem acesso aos recusos aws: SQS, SNS, S3
        /// </summary>
        public string AwsRegion { get; set; }

        /// <summary>
        /// Nome do bucket S3
        /// </summary>
        public string AwsBucketS3Name { get; set; }

        public string AwsBucketS3FolderNameSaveFile { get; set; }
        public string ServiceURLBucketS3 { get; set; }





    }

}