namespace BingSearchSummary.Web;

public class SummaryApiClient(HttpClient httpClient)
{
    public async Task<BingSearchSummaryItem[]> GetSummaryAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<BingSearchSummaryItem>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<BingSearchSummaryItem>("/summarys", cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }
}