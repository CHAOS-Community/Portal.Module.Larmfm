namespace Chaos.Portal.Module.Larmfm.Domain.Aws
{
    using System.IO;
    using Amazon;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Core.Exceptions;

    public class S3 : IStorage
    {
        public LarmSettings.AwsSettings Settings { get; set; }

        public S3(LarmSettings.AwsSettings settings)
        {
            Settings = settings;
        }

        public void Write(string key, Stream stream)
        {
            try
            {
                using (var s3 = AWSClientFactory.CreateAmazonS3Client(Settings.AccessKey, Settings.SecretAccessKey, RegionEndpoint.EUWest1))
                {
                    stream.Position = 0;

                    var request = new PutObjectRequest
                        {
                            BucketName = Settings.UploadBucket,
                            CannedACL = S3CannedACL.PublicRead,
                            InputStream = stream,
                            AutoCloseStream = true,
                            ContentType = "application/octet-stream",
                            Key = key
                        };

                    s3.PutObject(request);
                }
            }
            catch (AmazonS3Exception e)
            {
                throw new UnhandledException("Upload failed", e);
            }
        }
    }
}
