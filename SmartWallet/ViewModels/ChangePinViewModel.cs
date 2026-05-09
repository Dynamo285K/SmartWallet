using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class ChangePinViewModel(
    AuthService authService,
    IDialogService dialogService,
    IWalletNavigationService navigationService) : ObservableObject
{
    private bool _isEnteringNewPin;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PinDisplay))]
    public partial string EnteredPin { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TitleText { get; set; } = "Enter CURRENT PIN";

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    public string PinDisplay
    {
        get
        {
            var filled = new string('⬤', EnteredPin.Length);
            var empty = new string('○', 6 - EnteredPin.Length);
            return string.Join(" ", (filled + empty).ToCharArray());
        }
    }

    [RelayCommand]
    private async Task AddDigitAsync(string digit)
    {
        if (IsBusy || EnteredPin.Length >= 6) return;

        EnteredPin += digit;

        if (EnteredPin.Length == 6)
        {
            await ProcessPinAsync();
        }
    }

    [RelayCommand]
    private void RemoveDigit()
    {
        if (IsBusy) return;
        if (EnteredPin.Length > 0)
        {
            EnteredPin = EnteredPin.Substring(0, EnteredPin.Length - 1);
        }
    }

    private async Task ProcessPinAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            if (!_isEnteringNewPin) 
            {
                var isCorrect = await authService.VerifyPinAsync(EnteredPin);
                if (isCorrect)
                {
                    _isEnteringNewPin = true; 
                    EnteredPin = string.Empty;
                    TitleText = "Enter NEW PIN"; 
                }
                else
                {
                    ErrorMessage = "Incorrect PIN. Try again.";
                    EnteredPin = string.Empty;
                }
            }
            else 
            {
                await authService.SetPinAsync(EnteredPin);
                await dialogService.ShowAlertAsync("Success", "Your PIN has been successfully changed.");
                await navigationService.GoBackAsync();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}