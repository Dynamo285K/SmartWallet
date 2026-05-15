using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Interfaces;

namespace SmartWallet.ViewModels;

public partial class SettingsViewModel(
    IImportExportService importExportService,
    IDialogService dialogService,
    IWalletNavigationService navigationService) : ObservableObject
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
