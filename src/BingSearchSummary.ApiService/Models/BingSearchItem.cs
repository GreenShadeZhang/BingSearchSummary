namespace BingSearchSummary.ApiService.Models;
public class BingSearchItem
{
    public string Title
    {
        get; set;
    } = string.Empty;

    public string Url
    {
        get; set;
    } = string.Empty;

    public string Snippet
    {
        get; set;
    } = string.Empty;

    public string? PageContent
    {
        get; set;
    }
}
