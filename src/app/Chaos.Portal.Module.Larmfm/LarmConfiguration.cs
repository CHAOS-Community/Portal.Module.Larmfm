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

	    public AwsSettings Aws { get; set; }

	    public LarmConfiguration()
	    {
	        Aws = new AwsSettings();
	    }

	    public class AwsSettings
	    {
            public string UploadBucket { get; set; }
            public string AccessKey { get; set; }
            public string SecretAccessKey { get; set; }
	    }
	}
}