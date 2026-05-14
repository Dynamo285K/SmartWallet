using Microsoft.Maui.Controls;
using SmartWallet.Models.Interfaces;

namespace SmartWallet.Models.Services;

public class DialogService : IDialogService
{
    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        return CurrentPage.DisplayAlertAsync(title, message, cancel);
    }

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
    {
        return CurrentPage.DisplayAlertAsync(title, message, accept, cancel);
    }

    public Task<string?> ShowPromptAsync(string title, string message)
    {
        return CurrentPage.DisplayPromptAsync(title, message);
    }

    private static Page CurrentPage =>
        Shell.Current?.CurrentPage
        ?? (Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0].Page : null)
        ?? throw new InvalidOperationException("No active page is available for dialogs.");
}
