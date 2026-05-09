using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class SettingsViewModel(
    IExportService exportService,
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
        var wasExported = await exportService.ExportTransactionsToJsonAsync();

        if (!wasExported)
        {
            await dialogService.ShowAlertAsync("Export", "You have no transactions to export.");
        }
    }
}
