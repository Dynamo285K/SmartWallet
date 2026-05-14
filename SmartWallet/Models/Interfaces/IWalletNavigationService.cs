using SmartWallet.Models.Entities;

namespace SmartWallet.Models.Interfaces;

public interface IWalletNavigationService
{
    Task GoToAddTransactionAsync(bool isIncome);

    Task GoToEditTransactionAsync(Transaction transaction);

    Task GoToChangePinAsync();

    Task GoBackAsync();
}
