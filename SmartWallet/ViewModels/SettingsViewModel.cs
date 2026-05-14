using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Services;
using SmartWallet.Models.Interfaces;

namespace SmartWallet.ViewModels;

public partial class SettingsViewModel(
    IImportExportService importExportService,
    IDialogService dialogService,
    IWalletNavigationService navigationService,
    SeedService seedService,
    TransactionService transactionService) : ObservableObject
{
    [RelayCommand]
    private async Task ChangePinAsync()
    {
        await navigationService.GoToChangePinAsync();
    }

    [RelayCommand]
    private async Task ExportToJsonAsync()
    {
        var wasExported = await importExportService.ExportTransactionsToJsonAsync();

        if (!wasExported)
        {
            await dialogService.ShowAlertAsync("Export", "You have no transactions to export.");
        }
    }

    [RelayCommand]
    private async Task LoadDemoDataAsync()
    {
        var loaded = await seedService.SeedAsync();

        if (!loaded)
            await dialogService.ShowAlertAsync("Demo Data", "Demo data already exists. Delete all transactions first.");
        else
            await dialogService.ShowAlertAsync("Demo Data", "30 demo transactions loaded across 5 months.");
    }

    [RelayCommand]
    private async Task ClearAllTransactionsAsync()
    {
        var confirmed = await dialogService.ShowConfirmationAsync(
            "Clear All Transactions",
            "This will permanently delete all your transactions. Are you sure?",
            "Delete All", "Cancel");

        if (!confirmed) return;

        await transactionService.DeleteAllTransactionsAsync();
        await dialogService.ShowAlertAsync("Done", "All transactions have been deleted.");
    }

    [RelayCommand]
    private async Task ImportFromJsonAsync()
    {
        try
        {
            var count = await importExportService.ImportTransactionsFromJsonAsync();

            if (count == 0)
                await dialogService.ShowAlertAsync("Import", "No transactions were imported.");
            else
                await dialogService.ShowAlertAsync("Import", $"{count} transaction(s) imported successfully.");
        }
        catch (Exception)
        {
            await dialogService.ShowAlertAsync("Import", "Invalid file. Make sure you select a SmartWallet JSON export.");
        }
    }
}
