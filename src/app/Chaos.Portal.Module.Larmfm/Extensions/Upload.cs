using System.Xml.Linq;

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
        public ITranscoder Transcoder { get; set; }
        public LarmSettings Settings { get; set; }

        public Upload(IPortalApplication portalApplication, IMcmRepository repository, IStorage storage, ITranscoder transcoder, LarmSettings settings) : base(portalApplication)
        {
            Repository = repository;
            Storage = storage;
            Transcoder = transcoder;
            Settings = settings;
        }

        public EndpointResult Full(Guid objectGuid)
        {
            var file = Request.Files.FirstOrDefault();

            if(file == null)
                throw new IOException("No data found");

            var sourceKey = string.Format("upload/{0}", file.FileName);
            var folderPath = DateTime.Now.ToString("yyyy'/'MM'/'dd");
            var destinationFile = string.Format("{0}.mp3", Guid.NewGuid());
            Storage.Write(sourceKey, file.InputStream);
            Transcoder.Transcode(sourceKey, folderPath +  "/" + destinationFile);
            Repository.FileCreate(objectGuid, null, Settings.UploadDestinationId, destinationFile, file.FileName, folderPath, Settings.UploadFormatId);

            Repository.MetadataSet(objectGuid,
                Guid.NewGuid(),
                new Guid("00000000-0000-0000-0000-0000dd820000"),
                "da",
                1,
               XDocument.Parse("<Larm.FileInfos><Larm.FileInfo><StartOffSetMS>0</StartOffSetMS><EndOffSetMS>0</EndOffSetMS><FileName>"+destinationFile+"</FileName><Index>0</Index></Larm.FileInfo></Larm.FileInfos>"),
                new Guid("34613336-3836-6163-2D33-3562392D3131"));

            var obj = Repository.ObjectGet(objectGuid, true, true, true, true, true);
            ViewManager.Index(obj);

            return EndpointResult.Success();
        }
    }
}
