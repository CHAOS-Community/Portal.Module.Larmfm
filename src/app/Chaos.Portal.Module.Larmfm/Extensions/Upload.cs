namespace Chaos.Portal.Module.Larmfm.Extensions
{
    using System;
    using System.IO;
    using System.Linq;
    using Core;
    using Core.Extension;
    using Domain;
    using Mcm.Data;
    using v5.Extension.Result;

    public class Upload : AExtension
    {
        public IMcmRepository Repository { get; set; }
        public IStorage Storage { get; set; }

        public Upload(IPortalApplication portalApplication, IMcmRepository repository, IStorage storage) : base(portalApplication)
        {
            Repository = repository;
            Storage = storage;
        }

        public EndpointResult Full(Guid objectGuid)
        {
            var file = Request.Files.FirstOrDefault();

            if(file == null)
                throw new IOException("No data found");

            var path = string.Format("upload/{0}", file.FileName);
            Storage.Write(path, file.InputStream);

            return EndpointResult.Success();
        }
    }
}
