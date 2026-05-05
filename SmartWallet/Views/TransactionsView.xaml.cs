using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class TransactionsView : ContentPage
{
    private readonly TransactionsViewModel _viewModel;

    public TransactionsView(TransactionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadTransactionsAsync();
    }
}