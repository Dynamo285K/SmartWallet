using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class HomeView
{
    private readonly HomeViewModel _viewModel;

    public HomeView(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = LoadDataOnAppearingAsync();
    }

    private async Task LoadDataOnAppearingAsync()
    {
        try
        {
            await _viewModel.LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Error", "An error occurred while loading the data", "OK");
        }
    }
    
}