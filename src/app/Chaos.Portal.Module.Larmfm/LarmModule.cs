namespace Chaos.Portal.Module.Larmfm
{
    using System;
    using Extensions;
    using Mcm;
    using Core;
    using View;

    public class LarmModule : McmModule, ILarmModule
    {
		public LarmConfiguration Configuration { get; set; }

        #region Implementation of IModule

        protected override Core.Indexing.View.IView CreateObjectView()
        {
            return new ObjectView(PermissionManager);
        }

        public override void Load(IPortalApplication portalApplication)
        {
            base.Load(portalApplication);

			Configuration = new LarmConfiguration { UserProfileMetadataSchemaGuid = Guid.Parse("6EE5D41F-3A3F-254F-BA3E-3D9F80D5D49E"), UserProfileLanguageCode = "da", UserObjectTypeId = 55, UserFolderTypeId = 4, UsersFolder = "LARM/Users" };
            
            portalApplication.MapRoute("/v6/Search", () => new Search(portalApplication));
            portalApplication.MapRoute("/v6/WayfProfile", () => new WayfProfile(portalApplication, this));

            portalApplication.AddView(new SearchView(McmRepository), "larm-search");
            portalApplication.AddView(new AnnotationView(McmRepository), "larm-annotation");
        }

        #endregion
    }
}