using System.Collections.ObjectModel;

using BingSearchSummary.WinUI.Contracts.ViewModels;
using BingSearchSummary.WinUI.Core.Contracts.Services;
using BingSearchSummary.WinUI.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BingSearchSummary.WinUI.ViewModels;

public partial class BingSearchResultViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public BingSearchResultViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await _sampleDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
