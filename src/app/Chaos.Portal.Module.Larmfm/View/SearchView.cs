using System;
using System.Collections.Generic;
using System.Linq;
using CHAOS.Serialization;
using Chaos.Portal.Core.Indexing.View;
using Chaos.Mcm.Data.Dto;
using System.Xml.Linq;

namespace Chaos.Portal.Module.Larmfm.View
{
    using CHAOS;
    using CHAOS.Extensions;

    public class SearchView : AView
    {
        private const int RadioObjectId    = 24;
        private const int ScheduleObjectId = 86;

        private static readonly Guid ProgramMetadataSchemaGuid  = Guid.Parse("00000000-0000-0000-0000-0000df820000");
        private static readonly Guid ScheduleMetadataSchemaGuid = new UUID("70c26faf-b1ee-41e8-b916-a5a16b25ca69").ToGuid();

        public SearchView() : base("Search")
        {
        }

        public override IList<IViewData> Index(object objectsToIndex)
        {
            var obj = objectsToIndex as Mcm.Data.Dto.Object;

            if (obj == null) return new List<IViewData>();

            var data = new SearchViewData();

            switch (obj.ObjectTypeID)
            {
                case RadioObjectId:
                    {
                        var metadata = obj.Metadatas.FirstOrDefault(item => item.MetadataSchemaGuid == ProgramMetadataSchemaGuid);
                        if (metadata == null) return new List<IViewData>();
                        data.Title = GetMetadata(metadata.MetadataXml, "Title");
                        data.Type = "Radio";
                        break;
                    }
                case ScheduleObjectId:
                    {
                        var metadata = obj.Metadatas.FirstOrDefault(item => item.MetadataSchemaGuid == ScheduleMetadataSchemaGuid);
                        if (metadata == null) return new List<IViewData>();
                        var title = GetMetadata(metadata.MetadataXml, "Title");
                        data.Title    = string.IsNullOrEmpty(title) ? GetMetadata(metadata.MetadataXml, "Filename") : title;
                        data.Type     = "Schedule";
                        data.FreeText = GetMetadata(metadata.MetadataXml, "AllText");
                        
                        break;
                    }
                default :
                    return new List<IViewData>();
            }

            data.Id = obj.Guid.ToString();

            return new[] { data };
        }

        private string GetMetadata(XDocument xroot, string field)
        {
            var elm = xroot.Descendants(field).FirstOrDefault();
            if (elm == null)
                return string.Empty;

            return elm.Value;
        }

        public override Core.Data.Model.IPagedResult<Core.Data.Model.IResult> Query(Core.Indexing.IQuery query)
        {
            if (query.Query != "*:*") 
                query.Query = string.Format("(Title:{0}*^5)OR(FreeText:{0}*)", query.Query);

            return Query<SearchViewData>(query);
        }
    }

    public class SearchViewData : IViewData
    {
        public IEnumerable<KeyValuePair<string, string>> GetIndexableFields()
        {
            yield return UniqueIdentifier;
            
            yield return new KeyValuePair<string, string>("Title", Title);
            yield return new KeyValuePair<string, string>("Type", Type);
            
            if (!string.IsNullOrEmpty(FreeText)) yield return new KeyValuePair<string, string>("FreeText", FreeText);
        }

        public KeyValuePair<string, string> UniqueIdentifier { get { return new KeyValuePair<string, string>("Id", Id); } }
        public string Fullname { get { return "Chaos.Portal.Module.Larm.Data.SearchViewData"; } }

        [Serialize]
        public string Id { get; set; }

        [Serialize]
        public string Title { get; set; }

        [Serialize]
        public string Type { get; set; }

        public string FreeText { get; set; }
    }
}