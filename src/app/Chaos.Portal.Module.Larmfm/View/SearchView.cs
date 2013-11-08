using System;
using System.Collections.Generic;
using System.Linq;
using CHAOS.Serialization;
using Chaos.Mcm.Permission;
using Chaos.Portal.Core.Indexing.View;
using Chaos.Mcm.Data.Dto;
using System.Xml.Linq;

namespace Chaos.Portal.Module.Larmfm.View
{
    using CHAOS;
    using CHAOS.Extensions;

    public class SearchView : AView
    {
        private IPermissionManager PermissionManager { get; set; }
        private const int RadioObjectId    = 24;
        private const int ScheduleObjectId = 86;
        private const int ScheduleNoteObjectId = 87;

        private static readonly Guid ProgramMetadataSchemaGuid  = Guid.Parse("00000000-0000-0000-0000-0000df820000");
        private static readonly Guid ScheduleMetadataSchemaGuid = new UUID("70c26faf-b1ee-41e8-b916-a5a16b25ca69").ToGuid();
        private static readonly Guid ScheduleNoteMetadataSchemaGuid = new UUID("70c26faf-b1ee-41e8-b916-a5a16b25ca69").ToGuid();

        public SearchView(IPermissionManager permissionManager) : base("Search")
        {
            PermissionManager = permissionManager;
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
                        data.PubStartDate = Helpers.DateTimeHelper.ParseAndFormatDate(GetMetadata(metadata.MetadataXml, "PublicationDateTime"));
                        data.PubEndDate = Helpers.DateTimeHelper.ParseAndFormatDate(GetMetadata(metadata.MetadataXml, "PublicationEndDateTime"));
                        break;
                    }
                case ScheduleObjectId:
                    {
                        var metadata = obj.Metadatas.FirstOrDefault(item => item.MetadataSchemaGuid == ScheduleMetadataSchemaGuid);
                        if (metadata == null) return new List<IViewData>();
                        FillSchedule(obj, data, metadata, "Schedule");
                        break;
                    }
                case ScheduleNoteObjectId:
                    {
                        var metadata = obj.Metadatas.FirstOrDefault(item => item.MetadataSchemaGuid == ScheduleNoteMetadataSchemaGuid);
                        if (metadata == null) return new List<IViewData>();
                        FillSchedule(obj, data, metadata, "ScheduleNote");
                        break;
                    }
                default :
                    return new List<IViewData>();
            }

            data.Id = obj.Guid.ToString();

            return new[] { data };
        }

        private void FillSchedule(Mcm.Data.Dto.Object obj, SearchViewData data, Metadata metadata, string type)
        {
            var title = GetMetadata(metadata.MetadataXml, "Title");
            data.Title = string.IsNullOrEmpty(title) ? GetMetadata(metadata.MetadataXml, "Filename") : title;
            data.Type = type;
            data.FreeText = GetMetadata(metadata.MetadataXml, "AllText");
            data.Url = GetUrl(obj, "PDF");
            data.PubStartDate = Helpers.DateTimeHelper.ParseAndFormatDate(GetMetadata(metadata.MetadataXml, "Date"));
            data.PubEndDate = string.Empty;
        }

        private string GetMetadata(XDocument xroot, string field)
        {
            var elm = xroot.Descendants(field).FirstOrDefault();
            if (elm == null)
                return string.Empty;

            return elm.Value;
        }

        private string GetUrl(Mcm.Data.Dto.Object obj, string formatCategory)
        {
            var fileinfo = obj.Files.FirstOrDefault(item => item.FormatCategory == formatCategory);
            return fileinfo == null ? string.Empty : fileinfo.URL;
        }

        public override Core.Data.Model.IPagedResult<Core.Data.Model.IResult> Query(Core.Indexing.IQuery query)
        {
            if (query.Query != "*:*")
                query.Query = string.Format("(Title:\"{0}\"^5)OR(Title:{0}*^2)OR(FreeText:{0}*)", query.Query);

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

            if (!string.IsNullOrEmpty(PubStartDate))
                yield return new KeyValuePair<string, string>("PubStartDate", PubStartDate);
            if (!string.IsNullOrEmpty(PubEndDate))
                yield return new KeyValuePair<string, string>("PubEndDate", PubEndDate);
            
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

        [Serialize]
        public string Url { get; set; }

        public string FreeText { get; set; }

        [Serialize]
        public string PubStartDate { get; set; }

        [Serialize]
        public string PubEndDate { get; set; }

    }
}