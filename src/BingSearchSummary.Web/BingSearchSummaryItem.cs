namespace BingSearchSummary.Web;
public class BingSearchSummaryItem
{
    public string Title
    {
        get; set;
    } = string.Empty;

    public string Url
    {
        get; set;
    } = string.Empty;

    public string Summary
    {
        get; set;
    } = string.Empty;

    public DateTime CreateTime
    {
        get; set;
    }
}
