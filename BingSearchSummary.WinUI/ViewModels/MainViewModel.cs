using System.Collections.ObjectModel;
using System.ComponentModel;
using BingSearchSummary.WinUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace BingSearchSummary.WinUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly BingSearchApiClient _apiClient;

    private readonly Kernel _kernel;

    private IChatCompletionService _chatCompletionService;

    // Enable auto function calling
    OpenAIPromptExecutionSettings _openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        Temperature = 0.9,
        TopP = 0.9,
    };
    public MainViewModel(BingSearchApiClient apiClient, Kernel kernel)
    {
        _apiClient = apiClient;
        _kernel = kernel;

        _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    }
    [ObservableProperty]
    private string _question = string.Empty;


    [ObservableProperty]
    private ObservableCollection<BingSearchItem> _bingSearchList = new();

    [ObservableProperty]
    private ObservableCollection<string> _aiResultList = new();

    ChatHistory chatHistory = [];

    [RelayCommand]
    private async Task SearchAsync()
    {
        BingSearchList.Clear();

        var list = await _apiClient.GetContentsAsync(Question);

        foreach (var item in list)
        {
            BingSearchList.Add(item);
        }

        chatHistory.AddUserMessage(Question);

        var chatResult = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, _openAIPromptExecutionSettings, _kernel);

        AiResultList.Add(chatResult.ToString());
        //BingSearchList.Add(new BingSearchItem
        //{
        //    Title = Question,
        //    Url = "https://www.bing.com/search?q=" + Question,
        //    Snippet = "This is a snippet"
        //});
    }
}

/// <summary>
/// A plugin that returns the current time.
/// </summary>
public class TimeInformationPlugin
{
    /// <summary>
    /// Retrieves the current time in UTC.
    /// </summary>
    /// <returns>The current time in UTC. </returns>
    [KernelFunction, Description("Retrieves the current time")]
    public string GetCurrentTime()
        => DateTime.Now.ToString("s");
}

