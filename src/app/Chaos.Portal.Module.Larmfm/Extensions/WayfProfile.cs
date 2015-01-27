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

			var email = user.Email ?? "Unkown";
			var name = attributesObject["cn"] == null || attributesObject["cn"].Count == 0 ? "Unkown" : attributesObject["cn"][0];
			var organization = attributesObject["organizationName"] == null || attributesObject["organizationName"].Count == 0 ? "Unkown" : attributesObject["organizationName"][0];
			var title = attributesObject["eduPersonPrimaryAffiliation"] == null || attributesObject["eduPersonPrimaryAffiliation"].Count == 0 ? "Unkown" : attributesObject["eduPersonPrimaryAffiliation"][0];
			var country = attributesObject["schacCountryOfCitizenship"] == null || attributesObject["schacCountryOfCitizenship"].Count == 0 ? "Unkown" : attributesObject["schacCountryOfCitizenship"][0];

			var metadata = GetProfileMetadata(email, name, organization, title, country);
			var existingMetadata = userObject.Metadatas == null ? null : userObject.Metadatas.FirstOrDefault(m => m.MetadataSchemaGuid == Settings.UserProfileMetadataSchemaGuid);

			if (existingMetadata == null || existingMetadata.MetadataXml.ToString(SaveOptions.DisableFormatting) != metadata)
			{
				var metadataGuid = existingMetadata == null ? Guid.NewGuid() : existingMetadata.Guid;
				var revisionId = existingMetadata == null ? 0 : existingMetadata.RevisionID + 1;

				if (McmRepository.MetadataSet(userObject.Guid, metadataGuid, Settings.UserProfileMetadataSchemaGuid, Settings.UserProfileLanguageCode, revisionId, XDocument.Parse(metadata), user.Guid) != 1)
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