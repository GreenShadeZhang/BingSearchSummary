using BingSearchSummary.WinUI.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace BingSearchSummary.WinUI.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class BingSearchResultPage : Page
{
    public BingSearchResultViewModel ViewModel
    {
        get;
    }

    public BingSearchResultPage()
    {
        ViewModel = App.GetService<BingSearchResultViewModel>();
        InitializeComponent();
    }
}
