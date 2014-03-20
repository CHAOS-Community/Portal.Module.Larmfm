using System;
using Chaos.Mcm.Data;
using Chaos.Portal.Core;
using Chaos.Portal.Core.Data;
using Chaos.Portal.Core.Request;
using Moq;
using NUnit.Framework;

namespace Chaos.Portal.Module.Larmfm.Test
{
	public class TestBase
	{
		protected Mock<IPortalApplication> PortalApplication { get; set; }
		protected Mock<IPortalRequest> PortalRequest { get; set; }
		protected Mock<IPortalRepository> PortalRepository { get; set; }
		protected Mock<IMcmRepository> McmRepository { get; set; }
		protected Mock<ILarmModule> LarmModule { get; set; }
		protected LarmConfiguration Configuration { get; set; }

		[SetUp]
		public void SetUp()
		{
			PortalApplication = new Mock<IPortalApplication>();
			PortalRequest = new Mock<IPortalRequest>();
			PortalRepository = new Mock<IPortalRepository>();
			McmRepository = new Mock<IMcmRepository>();
			LarmModule = new Mock<ILarmModule>();
			Configuration = Make_Configuration();

			PortalApplication.SetupGet(p => p.PortalRepository).Returns(PortalRepository.Object);
			LarmModule.SetupGet(m => m.Configuration).Returns(Configuration);
			LarmModule.SetupGet(m => m.McmRepository).Returns(McmRepository.Object);
		}

		private LarmConfiguration Make_Configuration()
		{
			return new LarmConfiguration
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