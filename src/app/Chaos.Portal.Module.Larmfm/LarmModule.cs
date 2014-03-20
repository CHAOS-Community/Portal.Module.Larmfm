using System;
using Chaos.Portal.Module.Larmfm.Extensions;

namespace Chaos.Portal.Module.Larmfm
{
    using System.Collections.Generic;
    using System.Configuration;
    using CHAOS.Net;

    using Chaos.Mcm;
    using Chaos.Portal.Core;
    using Core.Extension;
    using Chaos.Portal.Core.Indexing.Solr;
    using Chaos.Portal.Module.Larmfm.View;

    public class LarmModule : McmModule, ILarmModule
    {
		public LarmConfiguration Configuration { get; set; }

        #region Implementation of IModule

        protected override Core.Indexing.View.IView CreateObjectView()
        {
            return new ObjectView(PermissionManager);
        }

        public override void Load(Core.IPortalApplication portalApplication)
        {
            base.Load(portalApplication);

			Configuration = new LarmConfiguration { UserProfileMetadataSchemaGuid = Guid.Parse("6EE5D41F-3A3F-254F-BA3E-3D9F80D5D49E"), UserProfileLanguageCode = "da", UserObjectTypeId = 55, UserFolderTypeId = 4, UsersFolder = "LARM/Users" };
            
            var searchView = new SearchView(base.PermissionManager);
            searchView.WithPortalApplication(portalApplication);
            searchView.WithCache(portalApplication.Cache);
            searchView.WithIndex(new SolrCore(new HttpConnection(ConfigurationManager.AppSettings["SOLR_URL"]), "larm-search"));

            portalApplication.ViewManager.AddView(searchView);

            var annotationView = new AnnotationView(base.PermissionManager);
            annotationView.WithPortalApplication(portalApplication);
            annotationView.WithCache(portalApplication.Cache);
            annotationView.WithIndex(new SolrCore(new HttpConnection(ConfigurationManager.AppSettings["SOLR_URL"]), "larm-annotation"));

            portalApplication.ViewManager.AddView(annotationView);
        }

        public override IExtension GetExtension(Protocol version, string name)
        {
            if (name == "Search")
                return new Search(PortalApplication);
			if (name == "WayfProfile")
				return new WayfProfile(PortalApplication, this);

            return base.GetExtension(version, name);
        }

        public override IEnumerable<string> GetExtensionNames(Protocol version)
        {
            foreach (var name in base.GetExtensionNames(version))
            {
                yield return name;
            }

            yield return "Search";
            yield return "WayfProfile";
        }

        #endregion
    }
}