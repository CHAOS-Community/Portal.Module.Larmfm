namespace Chaos.Portal.Module.Larmfm
{
    using System;
    using Core.Module;

    public class LarmSettings : IModuleSettings
	{
		public string UserProfileLanguageCode { get; set; }
		public Guid UserProfileMetadataSchemaGuid { get; set; }
		public string UsersFolder { get; set; }
		public uint UserFolderTypeId { get; set; } 
		public uint UserObjectTypeId { get; set; }

	    public AwsSettings Aws { get; set; }

	    public uint UploadDestinationId { get; set; }
        public uint UploadFormatId { get; set; }

	    public LarmSettings()
	    {
	        Aws = new AwsSettings();
	    }

	    public class AwsSettings
	    {
            public string UploadBucket { get; set; }
            public string AccessKey { get; set; }
            public string SecretAccessKey { get; set; }
            public string PipelineId { get; set; }
	        public string PresetId { get; set; }
	    }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(UserProfileLanguageCode) &&
                   !string.IsNullOrEmpty(UsersFolder) &&
                   UserProfileMetadataSchemaGuid != Guid.Empty &&
                   UserFolderTypeId != 0 &&
                   UserObjectTypeId != 0 &&
                   !string.IsNullOrEmpty(Aws.AccessKey) &&
                   !string.IsNullOrEmpty(Aws.UploadBucket) &&
                   !string.IsNullOrEmpty(Aws.PipelineId) &&
                   !string.IsNullOrEmpty(Aws.PresetId) &&
                   !string.IsNullOrEmpty(Aws.SecretAccessKey);
        }
	}
}