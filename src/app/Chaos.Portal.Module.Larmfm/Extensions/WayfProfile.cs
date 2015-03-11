using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Chaos.Mcm.Data;
using Chaos.Mcm.Data.Dto;
using Chaos.Portal.Core;
using Chaos.Portal.Core.Data.Model;
using Chaos.Portal.Core.Exceptions;
using Chaos.Portal.Core.Extension;
using CHAOS.Serialization.Standard.XML;
using Newtonsoft.Json;
using Object = Chaos.Mcm.Data.Dto.Object;

namespace Chaos.Portal.Module.Larmfm.Extensions
{
	public class WayfProfile : AExtension
	{
		public IMcmRepository McmRepository { get; set; }
		public LarmSettings Settings { get; set; }

        public WayfProfile(IPortalApplication portalApplication, IMcmRepository repository, LarmSettings settings) : base(portalApplication)
		{
			McmRepository = repository;
            Settings = settings;
		}

		public ScalarResult Update(Guid userGuid, string attributes)
		{
			if (!Request.User.HasPermission(SystemPermissons.Manage)) throw new InsufficientPermissionsException("Only managers can update wayfprofiles");

			var user = PortalRepository.UserInfoGet(userGuid, null, null, null).FirstOrDefault();

			if(user == null) throw new ArgumentException(string.Format("User with guid {0} not found", userGuid));

			var userObject = GetUserObject(userGuid);

			var attributesObject = JsonConvert.DeserializeObject<IDictionary<string, IList<string>>>(attributes);

			var email = user.Email == null || user.Email.IndexOf("@") == -1 ? "" : user.Email;
			var name = !attributesObject.ContainsKey("cn") || attributesObject["cn"].Count == 0 ? "" : attributesObject["cn"][0];
			var organization = !attributesObject.ContainsKey("organizationName") || attributesObject["organizationName"].Count == 0 ? "" : attributesObject["organizationName"][0];
			var title = !attributesObject.ContainsKey("eduPersonPrimaryAffiliation") || attributesObject["eduPersonPrimaryAffiliation"].Count == 0 ? "" : attributesObject["eduPersonPrimaryAffiliation"][0];
			var country = !attributesObject.ContainsKey("schacCountryOfCitizenship") || attributesObject["schacCountryOfCitizenship"].Count == 0 ? "" : attributesObject["schacCountryOfCitizenship"][0];

			var newProfile = new Domain.WayfProfile.Profile(email, name, organization, title, country);

			var existingMetadata = userObject.Metadatas == null ? null : userObject.Metadatas.FirstOrDefault(m => m.MetadataSchemaGuid == Settings.UserProfileMetadataSchemaGuid);
			var existingProfile = existingMetadata != null ? new Domain.WayfProfile.Profile(existingMetadata.MetadataXml) : null;


			if (existingProfile == null || !existingProfile.Equals(newProfile))
			{
				var metadataGuid = existingMetadata == null ? Guid.NewGuid() : existingMetadata.Guid;
				var revisionId = existingMetadata == null ? 0 : existingMetadata.RevisionID + 1;

				if(existingProfile != null) newProfile.FillEmptyDataFrom(existingProfile);

				if (McmRepository.MetadataSet(userObject.Guid, metadataGuid, Settings.UserProfileMetadataSchemaGuid, Settings.UserProfileLanguageCode, revisionId, newProfile.ToXml(), user.Guid) != 1)
					throw new Exception("Failed to create user profile metadata");
			}

			return new ScalarResult(1);
		}

		private Object GetUserObject(Guid userGuid)
		{
			var userObject = McmRepository.ObjectGet(userGuid, true);

			if (userObject == null)
			{
				var folders = McmRepository.FolderGet();

				if(folders == null)
					throw new Exception("Failed to get folders");

				if(folders.Count == 0)
					throw new Exception("No folders exist");

				if(string.IsNullOrWhiteSpace(Settings.UsersFolder))
					throw new Exception("Users folder is not set in settings");

				var usersFolder = GetFolderFromPath(null, Settings.UsersFolder.Split('/').ToList(), folders);

				if(usersFolder == null)
					throw new Exception(string.Format("Failed to find users folder: \"{0}\"", Settings.UsersFolder));

				var folderId = McmRepository.FolderCreate(userGuid, null, userGuid.ToString(), usersFolder.ID, Settings.UserFolderTypeId);

				if(McmRepository.ObjectCreate(userGuid, Settings.UserObjectTypeId, folderId) != 1)
					throw new Exception("Failed to create user object");

				userObject = new Object {Guid = userGuid, ObjectTypeID = Settings.UserObjectTypeId};
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