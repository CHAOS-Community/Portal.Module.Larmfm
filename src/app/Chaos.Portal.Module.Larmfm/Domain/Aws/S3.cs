namespace Chaos.Portal.Module.Larmfm.Domain.Aws
{
    using System.IO;
    using Amazon.S3.Model;

    public class S3 : IStorage
    {
        public LarmConfiguration.AwsSettings Settings { get; set; }

        public S3(LarmConfiguration.AwsSettings settings)
        {
            Settings = settings;
        }

        public void Write(string path, Stream stream)
        {
            using (var s3 = Amazon.AWSClientFactory.CreateAmazonS3Client(Settings.ApiKey, Settings.ApiSecret))
            {
                var request = new PutBucketRequest
                    {
                        BucketName = Settings.UploadBucket
                    };
                s3.PutBucket(request);
            }
        }
    }
}
