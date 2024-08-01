using BingSearchSummary.ApiService.Models;
using HtmlAgilityPack;

namespace BingSearchSummary.ApiService;

public class BingSearchHelper
{
    public static List<BingSearchItem> ParseHtmlToJson(string htmlContent)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        var results = new List<BingSearchItem>();

        foreach (var node in htmlDocument.DocumentNode.SelectNodes("//li[@class='b_algo']"))
        {
            var titleNode = node.SelectSingleNode(".//h2/a");
            var snippetNode = node.SelectSingleNode(".//p");
            var urlNode = node.SelectSingleNode(".//cite");

            var title = titleNode?.InnerText.Trim();
            var snippet = snippetNode?.InnerText.Trim();
            var url = urlNode?.InnerText.Trim();

            if (string.IsNullOrEmpty(title))
            {
                continue;
            }

            var searchItem = new BingSearchItem
            {
                Title = title,
                Snippet = snippet ?? "",
                Url = url ?? ""
            };

            results.Add(searchItem);
        }

        return results;
    }
}
