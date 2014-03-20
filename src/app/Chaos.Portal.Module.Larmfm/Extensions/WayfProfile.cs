using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Chaos.Mcm.Data;
using Chaos.Portal.Core;
using Chaos.Portal.Core.Data.Model;
using Chaos.Portal.Core.Exceptions;
using Chaos.Portal.Core.Extension;
using Newtonsoft.Json;
using Object = Chaos.Mcm.Data.Dto.Object;

namespace Chaos.Portal.Module.Larmfm.Extensions
{
	public class WayfProfile : AExtension
	{
		public IMcmRepository McmRepository { get; set; }
		public LarmConfiguration Configuration { get; set; }

		public WayfProfile(IPortalApplication portalApplication, ILarmModule larmModule) : base(portalApplication)
		{
			McmRepository = larmModule.McmRepository;
			Configuration = larmModule.Configuration;
		}

		public ScalarResult Update(Guid userGuid, string attributes)
		{
			if (!Request.User.HasPermission(SystemPermissons.Manage)) throw new InsufficientPermissionsException("Only managers can authenticate sessions");

			var user = PortalRepository.UserInfoGet(userGuid, null, null, null).FirstOrDefault();

			if(user == null) throw new ArgumentException(string.Format("User with guid {0} not found", userGuid));

			var userObject = GetUserObject(userGuid);

			var attributesObject = JsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(attributes);

			var metadata = GetProfileMetadata(user.Email, attributesObject["cn"][0], attributesObject["organizationName"][0], attributesObject["eduPersonPrimaryAffiliation"][0], attributesObject["schacCountryOfCitizenship"][0]);
			var existingMetadata = userObject.Metadatas == null ? null : userObject.Metadatas.FirstOrDefault(m => m.MetadataSchemaGuid == Configuration.UserProfileMetadataSchemaGuid);

			if (existingMetadata == null || existingMetadata.MetadataXml.ToString(SaveOptions.DisableFormatting) != metadata)
			{
				var metadataGuid = existingMetadata == null ? Guid.NewGuid() : existingMetadata.Guid;
				var revisionId = existingMetadata == null ? 0 : existingMetadata.RevisionID + 1;

				if (McmRepository.MetadataSet(userObject.Guid, metadataGuid, Configuration.UserProfileMetadataSchemaGuid, Configuration.UserProfileLanguageCode, revisionId, XDocument.Parse(metadata), user.Guid) != 1)
					throw new Exception("Failed to create user profile metadata");
			}

			return new ScalarResult(1);
		}

		private string GetProfileMetadata(string email, string name, string organization, string title, string country)
		{
			return string.Format("<CHAOS.Profile><Name>{0}</Name><Title>{1}</Title><About></About><Organization>{2}</Organization><Emails><Email>{3}</Email></Emails><Phonenumbers><Phonenumber></Phonenumber></Phonenumbers><Websites><Website></Website></Websites><Skype></Skype><LinkedIn></LinkedIn><Twitter></Twitter><Address></Address><City></City><Zipcode></Zipcode><Country>{4}</Country></CHAOS.Profile>",
				name, title, organization, email, country);
		}

		private Object GetUserObject(Guid userGuid)
		{
			var userObject = McmRepository.ObjectGet(userGuid, true);

			if (userObject == null)
			{
				var folders = McmRepository.FolderGet();

				if(folders == null)
					throw new Exception("Failed to get folders");

				var usersFolder = GetFolderFromPath(null, Configuration.UsersFolder.Split('/').ToList(), folders);

				if(usersFolder == null)
					throw new Exception("Failed to find users folder");

				var folderId = McmRepository.FolderCreate(userGuid, null, userGuid.ToString(), usersFolder.ID, Configuration.UserFolderTypeId);

				if(McmRepository.ObjectCreate(userGuid, Configuration.UserObjectTypeId, folderId) != 1)
					throw new Exception("Failed to create user object");

				userObject = new Object {Guid = userGuid, ObjectTypeID = Configuration.UserObjectTypeId};
			}

			return userObject;
		}

		private static Mcm.Data.Dto.Standard.Folder GetFolderFromPath(uint? parentId, IList<string> path, IList<Mcm.Data.Dto.Standard.Folder> folders)
		{
			foreach (var folder in folders)
			{
				if (folder.ParentID == parentId && folder.Name == path[0])
				{
					if (path.Count == 1)
						return folder;

					path.RemoveAt(0);
					folders.Remove(folder);

					return GetFolderFromPath(folder.ID, path, folders);
				}
			}

			return null;
		}
	}
}