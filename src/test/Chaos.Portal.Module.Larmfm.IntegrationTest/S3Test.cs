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
            var s3 = new S3(new LarmConfiguration.AwsSettings
                {
                    AccessKey = "",
                    SecretAccessKey = "",
                    UploadBucket = "integrationtests"
                });

            var stream = new FileStream("Ballad_of_Serenity.mp3", FileMode.Open);

            s3.Write("larm/Ballad_of_Serenity.mp3", stream);
        }
    }
}
