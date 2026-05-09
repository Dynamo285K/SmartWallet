using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class SettingsView
{
    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}