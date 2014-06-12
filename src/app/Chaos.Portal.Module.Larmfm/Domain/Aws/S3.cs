namespace Chaos.Portal.Module.Larmfm.Domain.Aws
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Amazon;
    using Amazon.ElasticTranscoder.Model;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Core.Exceptions;

    public class S3 : IStorage
    {
        public LarmConfiguration.AwsSettings Settings { get; set; }

        public S3(LarmConfiguration.AwsSettings settings)
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

                var outKey = string.Format("{0}/{1}.mp3", DateTime.Now.ToString("yyyy'/'MM'/'dd"), Guid.NewGuid());

                using (var et = AWSClientFactory.CreateAmazonElasticTranscoderClient(Settings.AccessKey, Settings.SecretAccessKey, RegionEndpoint.EUWest1))
                {
                    var request = new CreateJobRequest
                        {
                            PipelineId = "1402593766282-xnnqcd",
                            Input = new JobInput
                                {
                                    Key = key
                                },
                            Output = new CreateJobOutput
                                {
                                    PresetId = "1351620000001-300040",
                                    Key = outKey
                                }
                        };
                    et.CreateJob(request);
                }

                using (var s3 = AWSClientFactory.CreateAmazonS3Client(Settings.AccessKey, Settings.SecretAccessKey, RegionEndpoint.EUWest1))
                {
                    var request = new PutACLRequest
                        { 
                            BucketName = Settings.UploadBucket,
                            CannedACL = S3CannedACL.PublicRead,
                            Key = outKey
                        };
                    s3.PutACL(request);
                }

                // Create file in mcm
            }
            catch (Amazon.S3.AmazonS3Exception e)
            {
                throw new UnhandledException("Upload failed", e);
            }
            catch (Amazon.ElasticTranscoder.AmazonElasticTranscoderException e)
            {
                throw new UnhandledException("Transcoding failed", e);
            }
        }
    }
}
