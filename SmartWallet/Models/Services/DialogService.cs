using Microsoft.Maui.Controls;

namespace SmartWallet.Models.Services;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");

    Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);

    Task<string?> ShowPromptAsync(string title, string message);
}

public class DialogService : IDialogService
{
    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        return CurrentPage.DisplayAlert(title, message, cancel);
    }

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
    {
        return CurrentPage.DisplayAlert(title, message, accept, cancel);
    }

    public Task<string?> ShowPromptAsync(string title, string message)
    {
        return CurrentPage.DisplayPromptAsync(title, message);
    }

    private static Page CurrentPage =>
        Shell.Current?.CurrentPage
        ?? Application.Current?.Windows.FirstOrDefault()?.Page
        ?? throw new InvalidOperationException("No active page is available for dialogs.");
}
