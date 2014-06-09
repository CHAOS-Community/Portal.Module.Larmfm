namespace Chaos.Portal.Module.Larmfm
{
    using System;
    using Extensions;
    using Mcm;
    using Core;
    using View;

    public class LarmModule : ILarmModule
    {
		public LarmConfiguration Configuration { get; set; }

        public void Load(IPortalApplication portalApplication)
        {
            portalApplication.OnModuleLoaded += (o, args) =>
                {
                    var mcm = args.Module as IMcmModule;

                    if(mcm == null) return;

                    Configuration = new LarmConfiguration { UserProfileMetadataSchemaGuid = Guid.Parse("6EE5D41F-3A3F-254F-BA3E-3D9F80D5D49E"), UserProfileLanguageCode = "da", UserObjectTypeId = 55, UserFolderTypeId = 4, UsersFolder = "LARM/Users" };

                    portalApplication.MapRoute("/v6/Search", () => new Search(portalApplication));
                    portalApplication.MapRoute("/v6/WayfProfile", () => new WayfProfile(portalApplication, mcm.McmRepository, Configuration));

                    portalApplication.AddView(new SearchView(mcm.McmRepository), "larm-search");
                    portalApplication.AddView(new AnnotationView(mcm.McmRepository), "larm-annotation");
                    portalApplication.AddView(new ObjectView(mcm.PermissionManager), mcm.Configuration.ObjectCoreName, true);
                };
        }
    }
}