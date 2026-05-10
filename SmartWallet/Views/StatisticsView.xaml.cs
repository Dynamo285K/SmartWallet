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
        _ = _viewModel.LoadDataAsync();
    }

}
