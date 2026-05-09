using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class StatisticsView
{
    private readonly StatisticsViewModel _viewModel;

    public StatisticsView(StatisticsViewModel viewModel)
    {
        BindingContext = _viewModel = viewModel;
        InitializeComponent();
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
            await DisplayAlertAsync("Error", "Unable to load statistics.", "OK");
        }
    }

}
