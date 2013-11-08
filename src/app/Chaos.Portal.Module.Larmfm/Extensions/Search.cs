using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaos.Portal.Core;
using Chaos.Portal.Core.Data.Model;
using Chaos.Portal.Core.Extension;
using Chaos.Portal.Core.Indexing.Solr.Request;

namespace Chaos.Portal.Module.Larmfm.Extensions
{
    public class Search : AExtension
    {
        public Search(IPortalApplication portalApplication) : base(portalApplication)
        {
        }

        public IPagedResult<IResult> Simple(string query)
        {
            return ViewManager.GetView("Search").Query(new SolrQuery{Query = query});
        }

        public IPagedResult<IResult> DateRange(string query, DateTime start, DateTime end)
        {
            return ViewManager.GetView("Search").Query(new SolrQuery { Query = query /* more advanced query, Filter = ... */ });
        }
    }
}
