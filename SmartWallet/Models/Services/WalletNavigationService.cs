using SmartWallet.Models.Entities;
using SmartWallet.Models.Interfaces;
using SmartWallet.Views;

namespace SmartWallet.Models.Services;

public class WalletNavigationService : IWalletNavigationService
{
    public Task GoToAddTransactionAsync(bool isIncome)
    {
        return Shell.Current.GoToAsync($"{nameof(TransactionFormView)}?IsIncome={isIncome}");
    }

    public Task GoToEditTransactionAsync(Transaction transaction)
    {
        var navParams = new Dictionary<string, object>
        {
            { "TransactionToEdit", transaction }
        };

        return Shell.Current.GoToAsync(nameof(TransactionFormView), navParams);
    }

    public Task GoToChangePinAsync()
    {
        return Shell.Current.GoToAsync(nameof(ChangePinView));
    }

    public Task GoBackAsync()
    {
        return Shell.Current.GoToAsync("..");
    }
}
