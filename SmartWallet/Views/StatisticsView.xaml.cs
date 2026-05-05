using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class StatisticsView : ContentPage
{
    private readonly StatisticsViewModel _viewModel;

    public StatisticsView(StatisticsViewModel viewModel)
    {
        BindingContext = _viewModel = viewModel;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }
}
