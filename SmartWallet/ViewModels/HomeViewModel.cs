using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class HomeViewModel(
    TransactionService transactionService, 
    IDialogService dialogService,
    IWalletNavigationService navigationService) : ObservableObject
{
    [ObservableProperty]
    public partial decimal Balance { get; set; }

    [ObservableProperty]
    public partial bool IsLoading { get; set; }
    
    public ObservableCollection<Transaction> RecentTransactions { get; set; } = [];

    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var allTransactions = await transactionService.GetAllTransactionsAsync();

            decimal currentBalance = 0;
            foreach (var t in allTransactions)
            {
                if (t.IsIncome)
                    currentBalance += t.Amount;
                else
                    currentBalance -= t.Amount;
            }
            Balance = currentBalance;

            RecentTransactions.Clear();
            var top5 = allTransactions.Take(5);
            foreach (var t in top5)
            {
                RecentTransactions.Add(t);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await dialogService.ShowAlertAsync("Error", "An error occurred while loading the data.");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task AddIncomeAsync()
    {
        await navigationService.GoToAddTransactionAsync(true);
    }

    [RelayCommand]
    private async Task AddExpenseAsync()
    {
        await navigationService.GoToAddTransactionAsync(false);
    }
}
