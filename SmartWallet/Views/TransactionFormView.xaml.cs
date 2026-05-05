using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class TransactionFormView : ContentPage
{
    public TransactionFormView(TransactionFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
