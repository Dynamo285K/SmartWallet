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
        _ = LoadTransactionsAsync();
    }

    private async Task LoadTransactionsAsync()
    {
        try
        {
            await _viewModel.LoadTransactionsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await DisplayAlertAsync("Error", "Unable to load transactions.", "OK");
        }
    }
}