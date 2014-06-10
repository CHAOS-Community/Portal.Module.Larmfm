namespace Chaos.Portal.Module.Larmfm
{
    using System;
    using Core.Module;
    using Domain.Aws;
    using Extensions;
    using Mcm;
    using Core;
    using View;

    public class LarmModule : IModuleConfig
    {
        public void Load(IPortalApplication portalApplication)
        {
            portalApplication.OnModuleLoaded += (o, args) =>
                {
                    var mcm = args.Module as IMcmModule;

                    if(mcm == null) return;

                    var configuration = new LarmConfiguration
                        {
                            UserProfileMetadataSchemaGuid = Guid.Parse("6EE5D41F-3A3F-254F-BA3E-3D9F80D5D49E"), 
                            UserProfileLanguageCode = "da", 
                            UserObjectTypeId = 55, 
                            UserFolderTypeId = 4, 
                            UsersFolder = "LARM/Users",
                            Aws = new LarmConfiguration.AwsSettings
                                {
                                    UploadBucket = "larm-upload",
                                    ApiKey = "",
                                    ApiSecret = ""
                                }
                        };
                    var s3 = new S3(configuration.Aws);

                    portalApplication.MapRoute("/v6/Search", () => new Search(portalApplication));
                    portalApplication.MapRoute("/v6/WayfProfile", () => new WayfProfile(portalApplication, mcm.McmRepository, configuration));
                    portalApplication.MapRoute("/v6/Upload", () => new Upload(portalApplication, mcm.McmRepository, s3));

                    portalApplication.AddView(new SearchView(mcm.McmRepository), "larm-search");
                    portalApplication.AddView(new AnnotationView(mcm.McmRepository), "larm-annotation");
                    portalApplication.AddView(new ObjectView(mcm.PermissionManager), mcm.Configuration.ObjectCoreName, true);
                };
        }
    }
}