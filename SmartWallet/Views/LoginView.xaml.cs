using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class LoginView
{
    private readonly LoginViewModel _viewModel;

    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        _ = _viewModel.InitializeAsync();
    }
}
