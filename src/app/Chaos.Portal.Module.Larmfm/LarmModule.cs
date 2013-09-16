namespace Chaos.Portal.Module.Larmfm
{
    using System.Configuration;

    using CHAOS.Net;

    using Chaos.Portal.Core.Indexing.Solr;
    using Chaos.Portal.Module.Larmfm.View;

    public class LarmModule : Mcm.McmModule
    {
        #region Implementation of IModule

        protected override Core.Indexing.View.IView CreateObjectView()
        {
            return new View.ObjectView(PermissionManager);
        }

        public override void Load(Core.IPortalApplication portalApplication)
        {
            base.Load(portalApplication);

            var searchView = new SearchView();
            searchView.WithPortalApplication(portalApplication);
            searchView.WithCache(portalApplication.Cache);
            searchView.WithIndex(new SolrCore(new HttpConnection(ConfigurationManager.AppSettings["SOLR_URL"]), "larm-search" ));

            portalApplication.ViewManager.AddView(searchView);
        }

        #endregion
    }
}