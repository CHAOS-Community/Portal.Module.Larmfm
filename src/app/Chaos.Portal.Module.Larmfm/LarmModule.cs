namespace Chaos.Portal.Module.Larmfm
{
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
            var settings = portalApplication.GetSettings<LarmSettings>("Larm");

            portalApplication.OnModuleLoaded += (o, args) =>
                {
                    var mcm = args.Module as IMcmModule;

                    if(mcm == null) return;
                    
                    var s3 = new S3(settings.Aws);
                    var transcoder = new ElasticTranscoder(settings.Aws);

                    portalApplication.MapRoute("/v6/Search", () => new Search(portalApplication));
                    portalApplication.MapRoute("/v6/WayfProfile", () => new WayfProfile(portalApplication, mcm.McmRepository, settings));
                    portalApplication.MapRoute("/v6/Upload", () => new Upload(portalApplication, mcm.McmRepository, s3, transcoder, settings));

                    portalApplication.AddView(new SearchView(mcm.McmRepository), "larm-search");
                    portalApplication.AddView(new AnnotationView(mcm.McmRepository), "larm-annotation");
                    portalApplication.AddView(new ObjectView(mcm.PermissionManager), mcm.Configuration.ObjectCoreName, true);
                };
        }
    }
}