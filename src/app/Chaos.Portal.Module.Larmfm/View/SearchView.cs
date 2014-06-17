namespace Chaos.Portal.Module.Larmfm.View
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using CHAOS;
    using CHAOS.Extensions;
    using CHAOS.Serialization;
    using Core.Indexing.View;
    using Helpers;
    using System.Globalization;
    using Mcm.Data;
    using Mcm.Data.Dto;

    public class SearchView : AView
    {
        public IMcmRepository Repository { get; set; }
        private const int RadioObjectId    = 24;
        private const int ScheduleObjectId = 86;
        private const int ScheduleNoteObjectId = 87;
        private const int AnnotationObjectId = 64;
        private const int AttachedFileObjectId = 89;

        private static readonly Guid ProgramMetadataSchemaGuid  = Guid.Parse("00000000-0000-0000-0000-0000df820000");
        private static readonly Guid LarmMetadataSchemaGuid = Guid.Parse("17d59e41-13fb-469a-a138-bb691f13f2ba");
        private static readonly Guid ScheduleMetadataSchemaGuid = new UUID("70c26faf-b1ee-41e8-b916-a5a16b25ca69").ToGuid();
        private static readonly Guid ScheduleNoteMetadataSchemaGuid = new UUID("70c26faf-b1ee-41e8-b916-a5a16b25ca69").ToGuid();

        public SearchView(IMcmRepository repository) : base("Search")
        {
            Repository = repository;
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
                    data = CreateRadioSearchViewData(obj, data);

                    if (data == null) return new List<IViewData>();

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

                case AnnotationObjectId:
                {
                    //Get Radio object
                    var radioObjectGuid = obj.ObjectRelationInfos.First(o => o.Object1TypeID == RadioObjectId).Object1Guid;
                    var radioObject = Repository.ObjectGet(radioObjectGuid, true, true, true);

                    //Index RadioObject
                    data = CreateRadioSearchViewData(radioObject, data);

                    break;
                }
                default :
                    return new List<IViewData>();
            }

            data.ObjectFolders = obj.ObjectFolders;
            data.Id = obj.Guid.ToString();

            return new[] { data };
        }

        private SearchViewData CreateRadioSearchViewData(Mcm.Data.Dto.Object obj, SearchViewData data)
        {
            var metadata = obj.Metadatas.FirstOrDefault(item => item.MetadataSchemaGuid == ProgramMetadataSchemaGuid);

            var larmmetadata = obj.Metadatas.FirstOrDefault(item => item.MetadataSchemaGuid == LarmMetadataSchemaGuid);

            if (metadata == null) return null;

            var larmmetadataString = "";

            if (larmmetadata != null) larmmetadataString = MetadataHelper.GetXmlContent(larmmetadata.MetadataXml);

            data.HasLarmMetadata = !string.IsNullOrEmpty(larmmetadataString);

            data.Title = GetMetadata(metadata.MetadataXml, "Title");
            data.Channel = GetMetadata(metadata.MetadataXml, "PublicationChannel");
            data.Type = "Radio";
            data.FreeText = metadata.MetadataXml.Root.Value + " " + larmmetadataString;
            data.PubStartDate = DateTimeHelper.ParseAndFormatDate(GetMetadata(metadata.MetadataXml, "PublicationDateTime"));
            data.PubEndDate = DateTimeHelper.ParseAndFormatDate(GetMetadata(metadata.MetadataXml, "PublicationEndDateTime"));
            data.Duration = TimeCodeHelper.ConvertToTimeCode(data.PubStartDate, data.PubEndDate);
            data.DurationSec = TimeCodeHelper.ConvertToDurationInSec(data.PubStartDate, data.PubEndDate).ToString();
            data.ThumbUrl = MetadataHelper.GetUrl(obj, "Thumbnail");
            if (obj.ObjectRelationInfos != null)
            {
                data.AnnotationCount = obj.ObjectRelationInfos.Count(robj => robj.Object2TypeID == AnnotationObjectId).ToString();
                data.AttachedFilesCount = obj.ObjectRelationInfos.Count(robj => robj.Object2TypeID == AttachedFileObjectId).ToString();
                data.FreeTextAnnotation = GetRelatedObjectsFreeText(obj);
            }

            return data;
        }

        private string GetRelatedObjectsFreeText(Chaos.Mcm.Data.Dto.Object obj)
        {
            if (obj.ObjectRelationInfos == null)
                return string.Empty;

            var output = "";

            foreach (var relatedObj in obj.ObjectRelationInfos.Where(robj => robj.Object2TypeID == AnnotationObjectId || robj.Object2TypeID == AttachedFileObjectId))
            {
                output = output + GetAnnotationMetadata(relatedObj.Object2Guid);
            }

            //TODO: Implement method

            return string.Empty;
        }

        private void FillSchedule(Mcm.Data.Dto.Object obj, SearchViewData data, Metadata metadata, string type)
        {
            var title = GetMetadata(metadata.MetadataXml, "Title");
            data.Title = string.IsNullOrEmpty(title) ? GetMetadata(metadata.MetadataXml, "Filename") : title;
            data.Type = type;
            data.FreeText = GetMetadata(metadata.MetadataXml, "AllText");
            data.Url = MetadataHelper.GetUrl(obj, "PDF");
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

        private string GetAnnotationMetadata(Guid guid)
        {
            return MetadataHelper.GetXmlContent(Repository.ObjectGet(guid, true, true, true, true, true).Metadatas.First().MetadataXml);
        }

        public override Core.Data.Model.IPagedResult<Core.Data.Model.IResult> Query(Core.Indexing.IQuery query)
        {
            // TODO: split query.Query. Support AND, OR and NOT operators and double qoutes.
            if (query.Query != "*:*")
                query.Query = string.Format("(FreeText:({0}))", query.Query);
                //query.Query = string.Format("(Title:\"{0}\"^5)OR(Title:{0}*^2)OR(FreeText:{0}*)", query.Query);

            return Query<SearchViewData>(query);
        }
    }

    public class SearchViewData : IViewData
    {
        public IEnumerable<KeyValuePair<string, string>> GetIndexableFields()
        {
            yield return UniqueIdentifier;
            
            //Title
            if (!string.IsNullOrEmpty(FreeText)) yield return new KeyValuePair<string, string>("Title", Title);

            //Type. ex. Radio
            if (!string.IsNullOrEmpty(FreeText)) yield return new KeyValuePair<string, string>("Type", Type);

            if (!string.IsNullOrEmpty(PubStartDate))
                yield return new KeyValuePair<string, string>("PubStartDate", PubStartDate);

            if (!string.IsNullOrEmpty(PubEndDate))
                yield return new KeyValuePair<string, string>("PubEndDate", PubEndDate);
            
            if (!string.IsNullOrEmpty(FreeText)) yield return new KeyValuePair<string, string>("FreeText", FreeText + " " + FreeTextAnnotation);

            if (!string.IsNullOrEmpty(Channel)) yield return new KeyValuePair<string, string>("Channel", Channel);

            if (!string.IsNullOrEmpty(DurationSec)) yield return new KeyValuePair<string, string>("Duration", DurationSec);

            if (!string.IsNullOrEmpty(AnnotationCount)) yield return new KeyValuePair<string, string>("AnnotationCount", AnnotationCount);

            if (!string.IsNullOrEmpty(AttachedFilesCount)) yield return new KeyValuePair<string, string>("AttachedFilesCount", AttachedFilesCount);

            //UserContentToken -------------------------------------------------------------------------------------------->

            var aCount = 0;
            var aFileCount = 0;

            if((!string.IsNullOrEmpty(AnnotationCount)))
                if(int.TryParse(AnnotationCount, out aCount))
                    if(aCount > 0)
                        yield return new KeyValuePair<string, string>("UserContentToken", "Annotation");

            if ((!string.IsNullOrEmpty(AttachedFilesCount)))
                if (int.TryParse(AttachedFilesCount, out aFileCount))
                    if (aFileCount > 0)
                        yield return new KeyValuePair<string, string>("UserContentToken", "Attached file");

            if ((HasLarmMetadata)) yield return new KeyValuePair<string, string>("UserContentToken", "LARM Metadata");

            // ------------------------------------------------------------------------------------------------------------->

            if (ObjectFolders != null)
                foreach (var objectFolder in ObjectFolders)
                {
                    yield return new KeyValuePair<string, string>("FolderID", objectFolder.ID.ToString(CultureInfo.InvariantCulture));
                }
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
        public string Channel { get; set; }

        [Serialize]
        public string Duration { get; set; }

        public string DurationSec { get; set; }

        [Serialize]
        public string Url { get; set; }

        [Serialize]
        public string ThumbUrl { get; set; }

        [Serialize]
        public string AnnotationCount { get; set; }

        [Serialize]
        public string AttachedFilesCount { get; set; }

        public string UserContentToken { get; set; }

        public bool HasLarmMetadata { get; set; }

        public string FreeText { get; set; }

        public string FreeTextAnnotation { get; set; }

        [Serialize]
        public string PubStartDate { get; set; }

        [Serialize]
        public string PubEndDate { get; set; }

        public IList<ObjectFolder> ObjectFolders { get; set; }
    }
}