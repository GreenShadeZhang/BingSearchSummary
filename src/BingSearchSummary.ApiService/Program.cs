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

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

// ...
app.MapGet("/search", async (string keyword) =>
{
    var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    var page = await browser.NewPageAsync();

    // 设置 User-Agent 和视口大小
    var js = @"Object.defineProperties(navigator, {webdriver:{get:()=>false}});";
    await page.AddInitScriptAsync(js);

    await page.GotoAsync("https://www.bing.com");

    // 模拟用户输入搜索关键词
    await page.FillAsync("input[name=q]", keyword);
    await page.Keyboard.PressAsync("Enter");

    // 等待搜索结果加载
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    // 获取搜索结果内容
    var content = await page.ContentAsync();
    var dataList = BingSearchHelper.ParseHtmlToJson(content);
    var result = new List<BingSearchItem>();

    foreach (var data in dataList)
    {
        if (result.Count >= 3)
        {
            break;
        }//只处理三条数据
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

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
