namespace Chaos.Portal.Module.Larmfm.View
{
    using System.Collections.Generic;
    using System.Linq;
    using CHAOS;
    using CHAOS.Extensions;
    using CHAOS.Serialization;
    using Core.Indexing.View;
    using Mcm.Data.Dto;

    public class UserSearchView : AView
    {
        public UserSearchView() : base("UserSearch")
        {
        }

        public override IList<IViewData> Index(object objectsToIndex)
        {
            var obj = objectsToIndex as Object;

            if (obj == null) return new List<IViewData>();
            if (obj.ObjectTypeID != 55) return new List<IViewData>();

            var metadata = obj.Metadatas.FirstOrDefault(data => data.MetadataSchemaGuid == new UUID("1FD4E56E-3F3A-4F25-BA3E-3D9F80D5D49E").ToGuid());

            if(metadata == null) return new List<IViewData>();

            var name = metadata.MetadataXml.Root.Element("Name").Value;

            return new []
                {
                    new UserSearchViewData
                        {
                            Id = obj.Guid.ToString(),
                            Name = name
                        }
                };
        }

        public override Core.Data.Model.IPagedResult<Core.Data.Model.IResult> Query(Core.Indexing.IQuery query)
        {
            return Query<UserSearchViewData>(query);
        }
    }

    public class UserSearchViewData : IViewData
    {
        [Serialize]
        public string Id { get; set; }

        [Serialize]
        public string Name { get; set; }

        public IEnumerable<KeyValuePair<string, string>> GetIndexableFields()
        {
            yield return UniqueIdentifier;
            yield return new KeyValuePair<string, string>("Name", Name.ToLower());
        }

        public KeyValuePair<string, string> UniqueIdentifier { get{return new KeyValuePair<string, string>("Id", Id);} }
        public string Fullname { get; private set; }
        
    }
}