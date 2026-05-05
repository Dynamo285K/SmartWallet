using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly TransactionService _transactionService;
    private readonly IDialogService _dialogService;
    private readonly IWalletNavigationService _navigationService;

    public SettingsViewModel(
        TransactionService transactionService,
        IDialogService dialogService,
        IWalletNavigationService navigationService)
    {
        _transactionService = transactionService;
        _dialogService = dialogService;
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task ChangePinAsync()
    {
        await _navigationService.GoToChangePinAsync();
    }

    [RelayCommand]
    private async Task ExportToJsonAsync()
    {
        var transactions = await _transactionService.GetAllTransactionsAsync();
        
        if (transactions.Count == 0)
        {
            await _dialogService.ShowAlertAsync("Export", "You have no transactions to export.");
            return;
        }

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        var jsonString = JsonSerializer.Serialize(transactions, jsonOptions);

        var fileName = $"SmartWallet_Export_{DateTime.Now:yyyyMMdd}.json";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllTextAsync(filePath, jsonString);

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Export Transactions",
            File = new ShareFile(filePath)
        });
    }
}
