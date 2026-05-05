using SmartWallet.Models.Entities;
using SmartWallet.Views;

namespace SmartWallet.Models.Services;

public interface IWalletNavigationService
{
    Task GoToAddTransactionAsync(bool isIncome);

    Task GoToEditTransactionAsync(Transaction transaction);

    Task GoToChangePinAsync();

    Task GoBackAsync();
}

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
