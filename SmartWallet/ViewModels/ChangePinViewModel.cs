using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class ChangePinViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly IDialogService _dialogService;
    private readonly IWalletNavigationService _navigationService;

    private bool _isEnteringNewPin;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PinDisplay))]
    private string enteredPin = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string titleText = "Enter CURRENT PIN";

    [ObservableProperty]
    private bool isBusy;

    public string PinDisplay
    {
        get
        {
            string filled = new string('⬤', EnteredPin.Length);
            string empty = new string('○', 6 - EnteredPin.Length);
            return string.Join(" ", (filled + empty).ToCharArray());
        }
    }

    public ChangePinViewModel(
        AuthService authService,
        IDialogService dialogService,
        IWalletNavigationService navigationService)
    {
        _authService = authService;
        _dialogService = dialogService;
        _navigationService = navigationService;
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
                var isCorrect = await _authService.VerifyPinAsync(EnteredPin);
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
                await _authService.SetPinAsync(EnteredPin);
                await _dialogService.ShowAlertAsync("Success", "Your PIN has been successfully changed.");
                await _navigationService.GoBackAsync();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
}
