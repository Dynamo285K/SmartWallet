namespace SmartWallet.Models.Interfaces;

public interface IDialogService
{
    Task ShowAlertAsync(string title, string message, string cancel = "OK");

    Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);

    Task<string?> ShowPromptAsync(string title, string message);
}
