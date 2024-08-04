using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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

    private readonly IChatCompletionService _chatCompletionService;

    // Enable auto function calling
    private readonly OpenAIPromptExecutionSettings _openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        Temperature = 0.9,
        TopP = 0.9,
    };
    private readonly IPromptTemplateFactory _promptTemplateFactory;
    public MainViewModel(BingSearchApiClient apiClient, Kernel kernel, IPromptTemplateFactory promptTemplateFactory)
    {
        _apiClient = apiClient;
        _kernel = kernel;

        _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        _promptTemplateFactory = promptTemplateFactory;

        //BingSearchList.Add(new BingSearchItem
        //{
        //    Title = Question,
        //    Url = "https://www.bing.com/search?q=" + Question,
        //    Snippet = "This is a snippet"
        //});
    }

    [ObservableProperty]
    private string _question = string.Empty;

    [ObservableProperty]
    private string _summaryResult = string.Empty;

    [ObservableProperty]
    private bool _processRingStatus;

    [ObservableProperty]
    private bool _summaryProcessRingStatus;


    [ObservableProperty]
    private ObservableCollection<BingSearchItem> _bingSearchList = new();

    [ObservableProperty]
    private ObservableCollection<string> _aiResultList = new();

    readonly ChatHistory _chatHistory = [];

    readonly string _systemPromptTemplate = """
            You are an AI assistant that You can summarize what users are sending.
            The chat started at: {{ startTime }}
            """;

    readonly string _userPromptTemplate = """
            User Content:
            {{ userMessage }}
            """;

    [RelayCommand]
    private async Task SearchAsync()
    {
        ProcessRingStatus = true;
        BingSearchList.Clear();

        var list = await _apiClient.GetContentsAsync(Question);

        foreach (var item in list)
        {
            BingSearchList.Add(item);
        }

        ProcessRingStatus = false;
    }

    [RelayCommand]
    private async Task SummaryAndUploadAsync(BingSearchItem item)
    {
        _chatHistory.Clear();

        SummaryProcessRingStatus = true;

        var arguments = new KernelArguments
        {
            ["startTime"] = DateTimeOffset.Now.ToString("hh:mm:ss tt zz", CultureInfo.CurrentCulture),

            ["userMessage"] = item.Snippet
        };

        var systemMessage = await _promptTemplateFactory.Create(new PromptTemplateConfig(_systemPromptTemplate)
        {
            TemplateFormat = "liquid",
        }).RenderAsync(_kernel, arguments);

        var userMessage = await _promptTemplateFactory.Create(new PromptTemplateConfig(_userPromptTemplate)
        {
            TemplateFormat = "liquid",
        }).RenderAsync(_kernel, arguments);

        _chatHistory.AddSystemMessage(systemMessage);

        _chatHistory.AddUserMessage(userMessage);


        var chatResult = await _chatCompletionService.GetChatMessageContentAsync(_chatHistory, _openAIPromptExecutionSettings, _kernel);

        SummaryResult = chatResult.ToString();

        await _apiClient.PostContentsAsync(new BingSearchSummaryItem
        {
            Title = item.Title,
            Summary = chatResult.ToString(),
            Url = item.Url
        });

        SummaryProcessRingStatus = false;
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

