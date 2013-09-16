using System;
using System.Collections.Generic;

namespace Chaos.Portal.Module.Larmfm.View
{
    public class SearchView : AView
    {
        private static int ProgramObjectId = 24;
        private static Guid ProgramMetadataSchemaGuid = Guid.Parse("00000000-0000-0000-0000-0000df820000");

        public SearchView() : base("Search")
        {
        }

        public override IList<IViewData> Index(object objectsToIndex)
        {
            var obj = objectsToIndex as Object;

            if(obj == null) new List<IViewData>();
            if(obj.ObjectTypeID != ProgramObjectId) new List<IViewData>();

            var metadata = obj.Metadatas.FirstOrDefault(item => item.MetadataSchemaGuid == ProgramMetadataSchemaGuid);

            if (metadata == null) new List<IViewData>();

            var data  = new SearchViewData();
            var title =  metadata.MetadataXml.Descendants("Title").FirstOrDefault();

            data.Id    = obj.Guid.ToString();
            data.Title = title.Value;

            return new[] {data};
        }
    }

    public class SearchViewData : IViewData
    {
        public IEnumerable<KeyValuePair<string, string>> GetIndexableFields()
        {
            yield return UniqueIdentifier;
            yield return new KeyValuePair<string, string>("Title", Title);
        }

        public KeyValuePair<string, string> UniqueIdentifier { get { return new KeyValuePair<string, string>("Id", Id); } }
        public string Fullname { get { return "Chaos.Portal.Module.Larm.Data.SearchViewData"; } }

        [Serialize]
        public string Id { get; set; }

        [Serialize]
        public string Title { get; set; }
    }
}