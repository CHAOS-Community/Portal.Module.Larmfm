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
            var upload = Make_UploadExtension();
            var program = Make_ProgramObject();
            var file = new FileStream(null, "name.ext", "audio/mp3", 1000);
            PortalRequest.Setup(m => m.Files).Returns(new[]{file});

            upload.Full(program.Guid);

            StorageMock.Verify(m => m.Write("upload/name.ext", It.IsAny<Stream>()));
        }

        private Upload Make_UploadExtension()
        {
            return (Upload) new Upload(PortalApplication.Object, McmRepository.Object, StorageMock.Object).WithPortalRequest(PortalRequest.Object);
        }

        private Object Make_ProgramObject()
        {
            return new Object();
        }
    }
}
