namespace Chaos.Portal.Module.Larmfm.View
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    
    using CHAOS.Serialization;
    using Core.Indexing.View;
    using Helpers;
    using Mcm.Data;

    public class AnnotationView : AView 
    {
        public IMcmRepository Repository { get; set; }
        private uint annotationObjectTypeId = 64;

        public AnnotationView(IMcmRepository repository) : base("Annotation")
        {
            Repository = repository;
        }

        public override IList<IViewData> Index(object objectsToIndex)
        {
            var obj = objectsToIndex as Mcm.Data.Dto.Object;

            if (obj == null) return new List<IViewData>();

            if (obj.ObjectTypeID != annotationObjectTypeId) return new List<IViewData>();

            //Check if there is no relations -> No annotation. Then continue
            if (obj.ObjectRelationInfos.Count == 0) return new List<IViewData>();

            if (obj.Metadatas == null) return new List<IViewData>();

            var metadata = obj.Metadatas.FirstOrDefault();

            if (metadata == null) return new List<IViewData>();

            var title = GetMetadata(metadata.MetadataXml, "Title");

            //If sound source schema
            if(title == "")
                title = GetMetadata(metadata.MetadataXml, "SoundSourceName");

            var data = new AnnotationViewData
            {
                Title = title,
                MetadataSchemaGUID = metadata.MetadataSchemaGuid.ToString(),
                StartTime = GetMetadataAttribute(metadata.MetadataXml, "StartTime"),
                EndTime = GetMetadataAttribute(metadata.MetadataXml, "EndTime"),
                LanguageCode = metadata.LanguageCode,
                ProgramGUID = obj.ObjectRelationInfos.First().Object1Guid.ToString(),
                DateCreated = obj.DateCreated.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture),
                DateModified = metadata.DateCreated.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture),
                EditingUserGUID = metadata.EditingUserGuid.ToString()
            };

            data.EditingUser = GetUsername(data.EditingUserGUID);
            data.FreeText  = MetadataHelper.GetXmlContent(metadata.MetadataXml);
                        
                        data.Identifier = obj.Guid.ToString();

                        return new IViewData[] { data };
        }

        public override Core.Data.Model.IPagedResult<Core.Data.Model.IResult> Query(Core.Indexing.IQuery query)
        {
            // TODO: split query.Query. Support AND, OR and NOT operators and double qoutes.
            //if (query.Query != "*:*")
            //    query.Query = string.Format("(Title:\"{0}\"^5)OR(Title:{0}*^2)OR(FreeText:{0}*)", query.Query);

            return Query<AnnotationViewData>(query);
        }

        private string GetMetadata(XDocument xroot, string field)
        {
            if (!xroot.Descendants(field).Any())
                return "";

            var elm = xroot.Descendants(field).FirstOrDefault();
            if (elm == null)
                return "";

            return elm.Value;
        }

        private string GetMetadataAttribute(XDocument xroot, string attributeName)
        {
            var returnValue = "";

            try { returnValue = xroot.Root.Attribute(attributeName).Value; }
            catch (Exception ex) {  }

            return returnValue;
        }


        private string GetUsername(string guid)
        {
            Guid userGuid = Guid.Empty;

            Guid.TryParse(guid, out userGuid);

            if (userGuid == Guid.Empty) return "";

            var userObject = Repository.ObjectGet(userGuid, true, true, true, true, true);

            if (userObject == null) return "";

            if (userObject.Metadatas.Count == 0) return "";

            return userObject.Metadatas.First().MetadataXml.Descendants("Name").First().Value;
        }
    }

    public class AnnotationViewData : IViewData
    {
        public IEnumerable<KeyValuePair<string, string>> GetIndexableFields()
        {
            yield return UniqueIdentifier;

            yield return new KeyValuePair<string, string>("Title", Title);
            
            yield return new KeyValuePair<string, string>("MetadataSchemaGUID", MetadataSchemaGUID);

            yield return new KeyValuePair<string, string>("ProgramGUID", ProgramGUID);

            yield return new KeyValuePair<string, string>("EditingUserGUID", EditingUserGUID);

            yield return new KeyValuePair<string, string>("StartTime", TimeCodeHelper.ConvertTimeCodeToSec(StartTime).ToString());

            if (!string.IsNullOrEmpty(FreeText)) yield return new KeyValuePair<string, string>("FreeText", FreeText);
        }

        public KeyValuePair<string, string> UniqueIdentifier { get { return new KeyValuePair<string, string>("Id", Identifier); } }
        public string Fullname { get { return "Chaos.Portal.Module.Larm.Data.AnnotationViewData"; } }

        [Serialize]
        public string ProgramGUID { get; set; }

        [Serialize("Id")]
        public string Identifier { get; set; }

        [Serialize]
        public string Title { get; set; }

        [Serialize]
        public string MetadataSchemaGUID { get; set; }

        [Serialize]
        public string EditingUserGUID { get; set; }

        [Serialize]
        public string EditingUser { get; set; }

        public string FreeText { get; set; }

        [Serialize]
        public string DateCreated { get; set; }

        [Serialize]
        public string DateModified { get; set; }

        [Serialize]
        public string LanguageCode { get; set; }

        [Serialize]
        public string StartTime { get; set; }

        [Serialize]
        public string EndTime { get; set; }
    }
}
