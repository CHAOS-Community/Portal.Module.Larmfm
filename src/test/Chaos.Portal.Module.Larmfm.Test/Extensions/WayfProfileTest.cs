using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Chaos.Mcm.Data.Dto;
using Chaos.Mcm.Data.Dto.Standard;
using Chaos.Portal.Core.Data.Model;
using Chaos.Portal.Module.Larmfm.Extensions;
using Moq;
using NUnit.Framework;

namespace Chaos.Portal.Module.Larmfm.Test.Extensions
{
	[TestFixture]
	public class WayfProfileTest : TestBase
	{
		private const string WAYF_ATTRIBUTES = "{ \"sn\":[\"Jensen\"], \"gn\":[\"Jens\"], \"cn\":[\"Jens farmer\"], \"eduPersonPrincipalName\":[\"jj@testidp.wayf.dk\"], \"mail\":[\"jens.jensen@institution.dk\"], \"organizationName\":[\"Institution\"], \"eduPersonAssurance\":[\"2\"], \"schacPersonalUniqueID\":[\"urn:mace:terena.org:schac:personalUniqueID:dk:CPR:0708741234\"], \"eduPersonScopedAffiliation\":[\"student@course1.testidp.wayf.dk\",\"staff@course1.testidp.wayf.dk\",\"staff@course1.testidp.wsayf.dk\"], \"preferredLanguage\":[\"en\"], \"eduPersonEntitlement\":[\"test\"], \"eduPersonPrimaryAffiliation\":[\"student\"], \"schacCountryOfCitizenship\":[\"DK\"], \"eduPersonTargetedID\":[\"WAYF-DK-315880b0f9ef14662c6cbee76b9db72ac82d200a\"], \"schacHomeOrganization\":[\"testidp.wayf.dk\"], \"urn:oid:2.5.4.4\":[\"Jensen\"], \"urn:oid:2.5.4.42\":[\"Jens\"], \"urn:oid:2.5.4.3\":[\"Jens farmer\"], \"urn:oid:1.3.6.1.4.1.5923.1.1.1.6\":[\"jj@testidp.wayf.dk\"], \"urn:oid:0.9.2342.19200300.100.1.3\":[\"jens.jensen@institution.dk\"], \"urn:oid:2.5.4.10\":[\"Institution\"], \"urn:oid:1.3.6.1.4.1.5923.1.1.1.11\":[\"2\"], \"urn:oid:1.3.6.1.4.1.25178.1.2.15\":[\"urn:mace:terena.org:schac:personalUniqueID:dk:CPR:0708741234\"], \"urn:oid:1.3.6.1.4.1.5923.1.1.1.9\":[\"student@course1.testidp.wayf.dk\",\"staff@course1.testidp.wayf.dk\",\"staff@course1.testidp.wsayf.dk\"], \"urn:oid:2.16.840.1.113730.3.1.39\":[\"en\"], \"urn:oid:1.3.6.1.4.1.5923.1.1.1.7\":[\"test\"], \"urn:oid:1.3.6.1.4.1.5923.1.1.1.5\":[\"student\"], \"urn:oid:1.3.6.1.4.1.25178.1.2.5\":[\"DK\"], \"urn:oid:1.3.6.1.4.1.5923.1.1.1.10\":[\"WAYF-DK-315880b0f9ef14662c6cbee76b9db72ac82d200a\"], \"urn:oid:1.3.6.1.4.1.25178.1.2.9\":[\"testidp.wayf.dk\"], \"groups\":[\"realm-testidp.wayf.dk\",\"users\",\"members\"] }";
		private const string CHAOS_PROFILE = "<CHAOS.Profile><Name>Jens farmer</Name><Title>student</Title><About></About><Organization>Institution</Organization><Emails><Email>jens.jensen@institution.dk</Email></Emails><Phonenumbers><Phonenumber></Phonenumber></Phonenumbers><Websites><Website></Website></Websites><Skype></Skype><LinkedIn></LinkedIn><Twitter></Twitter><Address></Address><City></City><Zipcode></Zipcode><Country>DK</Country></CHAOS.Profile>";

		[Test]
		public void Update_GivenNewUsersAttributes_CreateUserObjectReturnSuccess()
		{
			var extension = Make_WayfProfileExtension();
			var user = new UserInfo
			{
				Guid = new Guid("63a0348b-ab4b-8847-9c71-2d0b4771b0fe"),
				Email = "jens.jensen@institution.dk"
			};
			var userFolder = new Folder
			{
				ID = 34
			};
				
			PortalRequest.SetupGet(r => r.User).Returns(Make_UserManager());
			PortalRepository.Setup(r => r.UserInfoGet(user.Guid, null, null, null)).Returns(new List<UserInfo>{user});
			McmRepository.Setup(r => r.FolderGet(null, null, null)).Returns(Make_Folders());
			McmRepository.Setup(r => r.FolderCreate(user.Guid, null, user.Guid.ToString(), 90, Configuration.UserFolderTypeId)).Returns(userFolder.ID).Verifiable();
			McmRepository.Setup(r => r.ObjectCreate(user.Guid, Configuration.UserObjectTypeId, userFolder.ID)).Returns(1).Verifiable();
			McmRepository.Setup(r => r.MetadataSet(user.Guid, It.IsAny<Guid>(), Configuration.UserProfileMetadataSchemaGuid, Configuration.UserProfileLanguageCode, 0, It.Is<XDocument>(x => x.ToString(SaveOptions.DisableFormatting) == CHAOS_PROFILE), user.Guid)).Returns(1).Verifiable();

			var result = extension.Update(user.Guid, WAYF_ATTRIBUTES);

			McmRepository.Verify();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Value, Is.EqualTo(1));
		}

		[Test]
		public void Update_GivenExistingUsersAttributesWithChanges_UpdateUserObjectReturnSuccess()
		{
			var extension = Make_WayfProfileExtension();
			var user = new UserInfo
			{
				Guid = new Guid("63a0348b-ab4b-8847-9c71-2d0b4771b0fe"),
				Email = "jens.jensen@institution.dk"
			};
			var existingMetadata = new Metadata
			{
				Guid = new Guid("70a0348b-ab4b-8847-9c71-2d0b4771b0ff"),
				MetadataSchemaGuid = Configuration.UserProfileMetadataSchemaGuid,
				MetadataXml = XDocument.Parse("<CHAOS.Profile></CHAOS.Profile>"),
				RevisionID = 3
			};
			var userObject = new Mcm.Data.Dto.Object
			{
				Guid = user.Guid,
				Metadatas = new List<Metadata>
				{
					new Metadata{Guid = new Guid("70a0348b-ab4b-8847-0000-2d0b4771b0ff")},
					existingMetadata
				}
			};

			PortalRequest.SetupGet(r => r.User).Returns(Make_UserManager());
			PortalRepository.Setup(r => r.UserInfoGet(user.Guid, null, null, null)).Returns(new List<UserInfo> { user });
			McmRepository.Setup(r => r.ObjectGet(user.Guid, true, false, false, false, false)).Returns(userObject).Verifiable();
			McmRepository.Setup(r => r.MetadataSet(user.Guid, existingMetadata.Guid, Configuration.UserProfileMetadataSchemaGuid, Configuration.UserProfileLanguageCode, 4, It.Is<XDocument>(x => x.ToString(SaveOptions.DisableFormatting) == CHAOS_PROFILE), user.Guid)).Returns(1).Verifiable();

			var result = extension.Update(user.Guid, WAYF_ATTRIBUTES);

			McmRepository.Verify();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Value, Is.EqualTo(1));
		}

		[Test]
		public void Update_GivenExistingUsersAttributesWithoutChanges_UpdateUserObjectReturnSuccess()
		{
			var extension = Make_WayfProfileExtension();
			var user = new UserInfo
			{
				Guid = new Guid("63a0348b-ab4b-8847-9c71-2d0b4771b0fe"),
				Email = "jens.jensen@institution.dk"
			};
			var existingMetadata = new Metadata
			{
				Guid = new Guid("70a0348b-ab4b-8847-9c71-2d0b4771b0ff"),
				MetadataSchemaGuid = Configuration.UserProfileMetadataSchemaGuid,
				MetadataXml = XDocument.Parse(CHAOS_PROFILE),
				RevisionID = 3
			};
			var userObject = new Mcm.Data.Dto.Object
			{
				Guid = user.Guid,
				Metadatas = new List<Metadata>
				{
					new Metadata{Guid = new Guid("70a0348b-ab4b-8847-0000-2d0b4771b0ff")},
					existingMetadata
				}
			};

			PortalRequest.SetupGet(r => r.User).Returns(Make_UserManager());
			PortalRepository.Setup(r => r.UserInfoGet(user.Guid, null, null, null)).Returns(new List<UserInfo> { user });
			McmRepository.Setup(r => r.ObjectGet(user.Guid, true, false, false, false, false)).Returns(userObject).Verifiable();

			var result = extension.Update(user.Guid, WAYF_ATTRIBUTES);

			McmRepository.Verify();

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Value, Is.EqualTo(1));
		}

		private IList<Folder> Make_Folders()
		{
			return new List<Folder>
			{
				new Folder { Name = "Some Folder", ID = 3},
				new Folder { Name = "LARM", ID = 45},
				new Folder { Name = "Blah blah", ParentID = 3},
				new Folder { Name = "Users", ID = 90, ParentID = 45},
				new Folder { Name = "PeterUser", ID = 10, ParentID = 90},
				new Folder { Name = "Something else", ID = 23}
			};
		}

		private UserInfo Make_UserManager()
		{
			return new UserInfo { SystemPermissonsEnum = SystemPermissons.All };
		}

		private WayfProfile Make_WayfProfileExtension()
		{
			return (WayfProfile)new WayfProfile(PortalApplication.Object, McmRepository.Object, Configuration).WithPortalRequest(PortalRequest.Object);
		}
	}
}