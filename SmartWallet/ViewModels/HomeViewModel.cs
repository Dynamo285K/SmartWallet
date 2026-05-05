using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly TransactionService _transactionService;
    private readonly IWalletNavigationService _navigationService;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private bool _isLoading;
    
    public ObservableCollection<Transaction> RecentTransactions { get; set; } = new();

    public HomeViewModel(TransactionService transactionService, IWalletNavigationService navigationService)
    {
        _transactionService = transactionService;
        _navigationService = navigationService;
    }

    public async Task LoadDataAsync()
    {
        IsLoading = true;

        var allTransactions = await _transactionService.GetAllTransactionsAsync();

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

        IsLoading = false;
    }
    
    [RelayCommand]
    private async Task AddIncomeAsync()
    {
        await _navigationService.GoToAddTransactionAsync(true);
    }

    [RelayCommand]
    private async Task AddExpenseAsync()
    {
        await _navigationService.GoToAddTransactionAsync(false);
    }
}
