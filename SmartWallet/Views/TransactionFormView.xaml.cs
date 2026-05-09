using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class TransactionFormView
{
    public TransactionFormView(TransactionFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
