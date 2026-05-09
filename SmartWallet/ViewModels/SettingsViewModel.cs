using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class SettingsViewModel(
    TransactionService transactionService,
    IDialogService dialogService,
    IWalletNavigationService navigationService) : ObservableObject
{
    
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    [RelayCommand]
    private async Task ChangePinAsync()
    {
        await navigationService.GoToChangePinAsync();
    }

    [RelayCommand]
    private async Task ExportToJsonAsync()
    {
        var transactions = await transactionService.GetAllTransactionsAsync();
        
        if (transactions.Count == 0)
        {
            await dialogService.ShowAlertAsync("Export", "You have no transactions to export.");
            return;
        }
        
        var jsonString = JsonSerializer.Serialize(transactions, JsonOptions);

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
