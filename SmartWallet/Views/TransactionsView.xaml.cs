using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class TransactionsView
{
    private readonly TransactionsViewModel _viewModel;

    public TransactionsView(TransactionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadTransactionsAsync();
    }
}
