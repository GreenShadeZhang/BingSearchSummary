using System.Net.Http.Json;
using BingSearchSummary.WinUI.Models;

namespace BingSearchSummary.WinUI;

public class BingSearchApiClient(HttpClient httpClient)
{
    public async Task<BingSearchItem[]> GetContentsAsync(string keyword, int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<BingSearchItem>? contentList = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<BingSearchItem>($"/search?keyword={keyword}", cancellationToken))
        {
            if (contentList?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                contentList ??= [];
                contentList.Add(forecast);
            }
        }

        return contentList?.ToArray() ?? [];
    }


    public async Task PostContentsAsync(BingSearchSummaryItem content, CancellationToken cancellationToken = default)
    {
        await httpClient.PostAsJsonAsync("/summary", content, cancellationToken);
    }
}
