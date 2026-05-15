using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class StatisticsView
{
    private readonly StatisticsViewModel _viewModel;

    public StatisticsView(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadDataAsync();
    }

}
