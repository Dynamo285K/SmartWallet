using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Services;
using SmartWallet.Models.Interfaces;

namespace SmartWallet.ViewModels;

public partial class ChangePinViewModel(
    AuthService authService,
    IDialogService dialogService,
    IWalletNavigationService navigationService) : ObservableObject
{
    private bool _isEnteringNewPin;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsPinDot1Filled))]
    [NotifyPropertyChangedFor(nameof(IsPinDot2Filled))]
    [NotifyPropertyChangedFor(nameof(IsPinDot3Filled))]
    [NotifyPropertyChangedFor(nameof(IsPinDot4Filled))]
    [NotifyPropertyChangedFor(nameof(IsPinDot5Filled))]
    [NotifyPropertyChangedFor(nameof(IsPinDot6Filled))]
    public partial string EnteredPin { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TitleText { get; set; } = "Enter CURRENT PIN";

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    public bool IsPinDot1Filled => EnteredPin.Length >= 1;
    public bool IsPinDot2Filled => EnteredPin.Length >= 2;
    public bool IsPinDot3Filled => EnteredPin.Length >= 3;
    public bool IsPinDot4Filled => EnteredPin.Length >= 4;
    public bool IsPinDot5Filled => EnteredPin.Length >= 5;
    public bool IsPinDot6Filled => EnteredPin.Length >= 6;

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
                EnteredPin = string.Empty;

                if (isCorrect)
                {
                    await Task.Delay(600);
                    _isEnteringNewPin = true;
                    TitleText = "Enter NEW PIN";
                }
                else
                {
                    ErrorMessage = "Incorrect PIN. Try again.";
                    await Task.Delay(600);
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
