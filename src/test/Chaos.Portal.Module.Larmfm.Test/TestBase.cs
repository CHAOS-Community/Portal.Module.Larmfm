using System;
using Chaos.Mcm.Data;
using Chaos.Portal.Core;
using Chaos.Portal.Core.Data;
using Chaos.Portal.Core.Request;
using Chaos.Portal.Module.Larmfm.Domain;
using Moq;
using NUnit.Framework;

namespace Chaos.Portal.Module.Larmfm.Test
{
    public class TestBase
	{
		protected Mock<IPortalApplication> PortalApplication { get; set; }
        protected Mock<IPortalRepository> PortalRepository { get; set; }
        protected Mock<IPortalRequest> PortalRequest { get; set; }
        protected Mock<IMcmRepository> McmRepository { get; set; }
        protected Mock<ITranscoder> TranscoderMock { get; set; }
        protected Mock<IStorage> StorageMock { get; set; }
        protected LarmSettings Settings { get; set; }

		[SetUp]
		public void SetUp()
		{
			PortalApplication = new Mock<IPortalApplication>();
		    PortalRepository = new Mock<IPortalRepository>();
		    PortalRequest = new Mock<IPortalRequest>();
		    McmRepository = new Mock<IMcmRepository>();
		    TranscoderMock = new Mock<ITranscoder>();
		    StorageMock = new Mock<IStorage>();
			Settings = Make_Configuration();

			PortalApplication.SetupGet(p => p.PortalRepository).Returns(PortalRepository.Object);
		}

	    public LarmSettings Make_Configuration()
		{
			return new LarmSettings
			{
				UserProfileMetadataSchemaGuid = Guid.Parse("6EE5D41F-3A3F-254F-BA3E-3D9F80D5D49E"),
				UserProfileLanguageCode = "da",
				UserObjectTypeId = 55,
				UserFolderTypeId = 4,
				UsersFolder = "LARM/Users"
			};
		}
	}
}