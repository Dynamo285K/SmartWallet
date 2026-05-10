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
        _ = _viewModel.LoadDataAsync();
    }
    
}
