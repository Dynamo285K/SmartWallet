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
        
        _ = InitializeOnAppearingAsync();
    }

    private async Task InitializeOnAppearingAsync()
    {
        try
        {
            await _viewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Error", "Unable to initialize login.", "OK");
        }
    }
}