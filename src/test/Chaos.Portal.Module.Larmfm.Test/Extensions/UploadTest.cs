namespace Chaos.Portal.Module.Larmfm.Test.Extensions
{
    using System.IO;
    using Larmfm.Extensions;
    using Mcm.Data.Dto;
    using Moq;
    using NUnit.Framework;
    using FileStream = Core.Request.FileStream;

    [TestFixture]
    public class UploadTest : TestBase
    {
        [Test]
        public void Full_UserHasPermission_SaveToS3()
        {
            var settings = Make_Configuration();
            var upload = Make_UploadExtension();
            var program = Make_ProgramObject();
            var file = new FileStream(null, "name.ext", "audio/mp3", 1000);
            PortalRequest.Setup(m => m.Files).Returns(new[]{file});
            PortalApplication.Setup(m => m.ViewManager.Index(null));

            upload.Full(program.Guid);

            StorageMock.Verify(m => m.Write("upload/name.ext", It.IsAny<Stream>()));
            TranscoderMock.Verify(m => m.Transcode("upload/name.ext", It.IsAny<string>()));
            McmRepository.Verify(m => m.FileCreate(program.Guid, null, settings.UploadDestinationId, It.IsAny<string>(), "name.ext", It.IsAny<string>(), settings.UploadFormatId));
        }

        private Upload Make_UploadExtension()
        {
            return (Upload) new Upload(PortalApplication.Object, McmRepository.Object, StorageMock.Object, TranscoderMock.Object, Make_Configuration()).WithPortalRequest(PortalRequest.Object);
        }

        private Object Make_ProgramObject()
        {
            return new Object();
        }
    }
}
