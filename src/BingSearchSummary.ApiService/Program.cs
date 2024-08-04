using BingSearchSummary.ApiService;
using BingSearchSummary.ApiService.Models;
using Microsoft.Playwright;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/summarys", () =>
{
    var summarys = Enumerable.Range(1, 5).Select(index =>
        new BingSearchSummaryItem
        {
            CreateTime = DateTime.Now,
            Summary = summaries[Random.Shared.Next(summaries.Length)],
            Title = summaries[Random.Shared.Next(summaries.Length)],
            Url = "https://www.bing.com"
        })
        .ToArray();
    return summarys;
});

// ...
app.MapGet("/search", async (string keyword) =>
{
    var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    var page = await browser.NewPageAsync();

    // ���� User-Agent ���ӿڴ�С
    var js = @"Object.defineProperties(navigator, {webdriver:{get:()=>false}});";
    await page.AddInitScriptAsync(js);

    await page.GotoAsync("https://www.bing.com");

    // ģ���û����������ؼ���
    await page.FillAsync("input[name=q]", keyword);
    await page.Keyboard.PressAsync("Enter");

    // �ȴ������������
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    // ��ȡ�����������
    var content = await page.ContentAsync();
    var dataList = BingSearchHelper.ParseHtmlToJson(content);
    var result = new List<BingSearchItem>();

    foreach (var data in dataList)
    {
        if (result.Count >= 3)
        {
            break;
        }//ֻ������������
        await page.GotoAsync(data.Url);

        var divContent = await page.QuerySelectorAsync(".content");

        divContent ??= await page.QuerySelectorAsync("body");

        if (divContent != null)
        {
            var pageContent = await divContent.InnerTextAsync();

            result.Add(new BingSearchItem
            {
                Title = data.Title,
                Url = data.Url,
                Snippet = data.Snippet,
                PageContent = pageContent
            });
        }
    }
    await browser.CloseAsync();

    return result;
});


// ...

app.MapPost("/summary", (BingSearchSummaryItem summary) =>
{
    // Save the summary to the database or perform any other necessary actions
    // ...
});

app.MapDefaultEndpoints();

app.Run();