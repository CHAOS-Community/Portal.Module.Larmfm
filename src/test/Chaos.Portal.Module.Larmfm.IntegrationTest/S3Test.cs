namespace Chaos.Portal.Module.Larmfm.IntegrationTest
{
    using System.IO;
    using Domain.Aws;
    using NUnit.Framework;

    [TestFixture]
    public class S3Test
    {
        [Test]
        public void Write__ResponseWithAckFromS3()
        {
            var s3 = new S3(new LarmSettings.AwsSettings
                {
                    AccessKey = "AKIAIYOKLADGSIR443GQ",
                    SecretAccessKey = "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",
                    UploadBucket = "integrationtests"
                });

            var stream = new FileStream("Ballad_of_Serenity.mp3", FileMode.Open);

            s3.Write("larm/Ballad_of_Serenity.mp3", stream);
        }

        [Test]
        public void Transcode__ResponseWithAckFromS3()
        {
            var aws = new LarmSettings.AwsSettings
                {
                    AccessKey = "AKIAIYOKLADGSIR443GQ", 
                    SecretAccessKey = "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", 
                    UploadBucket = "integrationtests",
                    PipelineId = "1402593766282-xnnqcd",
                    PresetId = "1351620000001-300040"
                };
            var s3 = new S3(aws);
            var transcode = new ElasticTranscoder(aws);
            var stream = new FileStream("Ballad_of_Serenity.mp3", FileMode.Open);
            s3.Write("larm/Ballad_of_Serenity.mp3", stream);

            transcode.Transcode("larm/Ballad_of_Serenity.mp3", "larm/result.mp3");
        }
    }
}
