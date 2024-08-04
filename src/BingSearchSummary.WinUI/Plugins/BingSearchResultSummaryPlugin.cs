using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace BingSearchSummary.WinUI.Plugins;
/// <summary>
/// Summary BingSearch Result
/// </summary>
public class BingSearchResultSummaryPlugin
{
    /// <summary>
    /// Retrieves the current time in UTC.
    /// </summary>
    /// <returns>The current time in UTC. </returns>
    [KernelFunction, Description("Retrieves the current time")]
    public string GetCurrentTime()
        => DateTime.Now.ToString("s");
}