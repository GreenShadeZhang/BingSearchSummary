using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingSearchSummary.WinUI.Models;
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
}
