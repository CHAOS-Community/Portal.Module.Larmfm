namespace Chaos.Portal.Module.Larmfm.Extensions
{
    using System;
    using System.Linq;

    using Core;
    using Core.Data.Model;
    using Core.Extension;
    using Core.Indexing.Solr.Request;

    public class Search : AExtension
    {
        public Search(IPortalApplication portalApplication) : base(portalApplication)
        {
        }

        public IPagedResult<IResult> Simple(string query)
        {
            return ViewManager.GetView("Search").Query(new SolrQuery { Query = query });
        }

        public IPagedResult<IResult> DateRange(string query, DateTime start, DateTime end)
        {
            return ViewManager.GetView("Search").Query(new SolrQuery { Query = query /* more advanced query, Filter = ... */ });
        }

        public QueryResult Facet(string query, string facetField)
        {
            var q = new SolrQuery
                {
                    Query = query,
                    Facet = string.Format("field:{0}", facetField)
                };
            
            var result = ViewManager.GetView("Search").FacetedQuery(q);
            var fieldFacets = result.FacetFieldsResult.Select(item => new FieldFacet(item.Value, item.Facets.Select(facet => new Facet(facet.Value, facet.Count)).ToList()));

            return new QueryResult { FieldFacets = fieldFacets.ToList() };
        }

        public IPagedResult<IResult> Users(string query)
        {

            var q = new SolrQuery
                {
                    Query = ""
                };
            var results = ViewManager.GetView("Users").Query(q);

            return new PagedResult<IResult>(0,0, null);
        }
    }
}
