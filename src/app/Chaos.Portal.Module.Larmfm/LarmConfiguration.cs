using System;

namespace Chaos.Portal.Module.Larmfm
{
	public class LarmConfiguration
	{
		public string UserProfileLanguageCode { get; set; }
		public Guid UserProfileMetadataSchemaGuid { get; set; }
		public string UsersFolder { get; set; }
		public uint UserFolderTypeId { get; set; } 
		public uint UserObjectTypeId { get; set; } 
	}
}