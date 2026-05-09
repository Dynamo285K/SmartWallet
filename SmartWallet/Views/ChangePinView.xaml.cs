using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class ChangePinView
{
    public ChangePinView(ChangePinViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}