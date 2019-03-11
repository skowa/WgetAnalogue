using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace WgetAnalogue
{
    internal static class HtmlDocumentParser
    {
        internal static IEnumerable<string> GetLinksFromTagsA(this HtmlDocument document)
        {
            return document.DocumentNode.Descendants("a")
                .Where(d => d.Attributes["href"] != null)
                .Select(d => d.Attributes["href"].Value);
        }

        internal static IEnumerable<string> GetAllLinksExceptTagALinks(this HtmlDocument document)
        {
            IEnumerable<string> links = document.DocumentNode.Descendants()
                .Where(d => d.Attributes["src"] != null)
                .Select(d => d.Attributes["src"].Value);

            return links.Union(document.DocumentNode.Descendants()
                .Where(d => d.Name != "a" && d.Attributes["href"] != null)
                .Select(d => d.Attributes["href"].Value));
        }
    }
}