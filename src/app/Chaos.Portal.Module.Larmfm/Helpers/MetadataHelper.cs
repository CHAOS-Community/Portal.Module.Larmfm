using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Chaos.Portal.Module.Larmfm.Helpers
{
    public static class MetadataHelper
    {
        public static string GetXmlContent(XContainer xml)
        {
            var sb = new StringBuilder();

            foreach (var node in xml.Descendants())
            {
                if (!node.HasElements)
                    sb.AppendLine(node.Value);
            }

            return sb.ToString();
        }

        public static string GetUrl(Mcm.Data.Dto.Object obj, string formatCategory)
        {
            var fileinfo = obj.Files.FirstOrDefault(item => item.FormatCategory == formatCategory);
            return fileinfo == null ? string.Empty : fileinfo.URL;
        }
    }
}
