namespace Chaos.Portal.Module.Larmfm.View
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Chaos.Mcm.Permission;
    using Chaos.Mcm.View;
    using Chaos.Portal.Core.Data.Model;
    using Chaos.Portal.Core.Indexing.View;

    using Object = Chaos.Mcm.Data.Dto.Object;

    public class ObjectView : AView
    {
        #region Initialization

        public ObjectView(IPermissionManager permissionManager) : base("Object")
        {
            PermissionManager = permissionManager;
        }

        #endregion
        #region Properties

        public IPermissionManager PermissionManager { get; set; }

        #endregion
        #region Overrides of AView

        public override IList<IViewData> Index(object objectsToIndex)
        {
            var obj = objectsToIndex as Object;

            if (obj == null) return new List<IViewData>();

            return new[] { new ObjectViewData(obj, PermissionManager) };
        }

        public override IPagedResult<IResult> Query(Core.Indexing.IQuery query)
        {
            var result = Core.Query(query);

            var foundCount = result.QueryResult.FoundCount;
            var startIndex = result.QueryResult.StartIndex;
            var keys = result.QueryResult.Results.Select(item => CreateKey(item.Id));
            var results = Cache.Get<ObjectViewData>(keys);

            return new PagedResult<IResult>(foundCount, startIndex, results);
        }

        #endregion
    }

    public class LarmObjectViewData : ObjectViewData
    {
        #region Initialization

        public LarmObjectViewData(Object obj, IPermissionManager permissionManager)
        {
            Object = obj;
            PermissionManager = permissionManager;
        }

        public LarmObjectViewData()
        {
            Object = new Object();
        }

        #endregion
        #region Properties


        
        #endregion
        #region Business Logic

        public override IEnumerable<KeyValuePair<string, string>> GetIndexableFields()
        {
            foreach (var baseField in base.GetIndexableFields())
            {
                yield return baseField;
            }

            var programMetadata = Object.Metadatas.FirstOrDefault(item => item.MetadataSchemaGuid == Guid.Parse("00000000-0000-0000-0000-0000df820000"));

            if (programMetadata != null)
            {
                var root = programMetadata.MetadataXml.Root;

                if (root != null && root.Element("PublicationDateTime") != null && root.Element("PublicationEndDateTime") != null)
                {
                    DateTime larmPubStartDate;
                    DateTime larmPubEndDate;

                    var dateTimeFormat1 = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
                    var dateTimeFormat2 = "dd'-'MM'-'yyyy HH':'mm':'ss";
                    var dateTimeFormat3 = "dd'/'MM'/'yyyy HH':'mm':'ss";
                    var dateTimeFormat4 = "yyyy'-'MM'-'dd HH':'mm':'ss";
                    var dateTimeFormat5 = "yyyy-MM-ddTHH:mm:ss.fffzzz";

                    var formatProvider         = CultureInfo.InvariantCulture;
                    var larmPubStartDateString = root.Element("PublicationDateTime").Value;
                    var larmPubEndDateString   = root.Element("PublicationEndDateTime").Value;

                    if (!DateTime.TryParseExact(larmPubStartDateString, dateTimeFormat1, formatProvider, DateTimeStyles.None, out larmPubStartDate) &&
                        !DateTime.TryParseExact(larmPubStartDateString, dateTimeFormat2, formatProvider, DateTimeStyles.None, out larmPubStartDate) &&
                        !DateTime.TryParseExact(larmPubStartDateString, dateTimeFormat3, formatProvider, DateTimeStyles.None, out larmPubStartDate) &&
                        !DateTime.TryParseExact(larmPubStartDateString, dateTimeFormat4, formatProvider, DateTimeStyles.None, out larmPubStartDate) &&
                        !DateTime.TryParseExact(larmPubStartDateString, dateTimeFormat5, formatProvider, DateTimeStyles.None, out larmPubStartDate))
                        yield break;

                    if (!DateTime.TryParseExact(larmPubEndDateString, dateTimeFormat1, formatProvider, DateTimeStyles.None, out larmPubEndDate) &&
                        !DateTime.TryParseExact(larmPubEndDateString, dateTimeFormat2, formatProvider, DateTimeStyles.None, out larmPubEndDate) &&
                        !DateTime.TryParseExact(larmPubEndDateString, dateTimeFormat3, formatProvider, DateTimeStyles.None, out larmPubEndDate) &&
                        !DateTime.TryParseExact(larmPubEndDateString, dateTimeFormat4, formatProvider, DateTimeStyles.None, out larmPubEndDate) &&
                        !DateTime.TryParseExact(larmPubEndDateString, dateTimeFormat5, formatProvider, DateTimeStyles.None, out larmPubEndDate))
                        yield break;

                    yield return new KeyValuePair<string, string>("LARM-PubStartDate", larmPubStartDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", formatProvider));
                    yield return new KeyValuePair<string, string>("LARM-PubEndDate", larmPubEndDate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", formatProvider));

                    if (larmPubEndDate.CompareTo(larmPubStartDate) > 0)
                        yield return new KeyValuePair<string, string>("LARM-Duration", ((uint)larmPubEndDate.Subtract(larmPubStartDate).TotalSeconds).ToString());
                    else
                        yield return new KeyValuePair<string, string>("LARM-Duration", ((uint)larmPubStartDate.Subtract(larmPubEndDate).TotalSeconds).ToString());

                    if (root.Element("PublicationChannel") != null)
                        yield return new KeyValuePair<string, string>("LARM-Channel", root.Element("PublicationChannel").Value);

                    if (root.Element("Title") != null)
                        yield return new KeyValuePair<string, string>("LARM-Title", root.Element("Title").Value.Trim().TrimStart('"').TrimEnd('"'));
                }
            }



            yield return new KeyValuePair<string, string>("LARM-Annotation-Count", ObjectRelationInfos.Count(item => item.Object2TypeID == 41 || item.Object2TypeID == 64).ToString());

            //Has related Annotation object
            if (ObjectRelationInfos.Any(obj => obj.Object2TypeID == 64))
                yield return new KeyValuePair<string, string>("LARM-Program-Contain", "Annotation");

            //Has related attached file object
            if (ObjectRelationInfos.Any(obj => obj.Object2TypeID == 89))
                yield return new KeyValuePair<string, string>("LARM-Program-Contain", "attachedfile");


        }

        #endregion

        #region Implementation of IResult

        #endregion
    }
}