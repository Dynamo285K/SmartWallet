using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class LoginViewModel(
    AuthService authService, 
    IAppNavigationService navigationService) : ObservableObject
{
    private bool _isRegistering;
    private bool _isInitialized;

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
    public partial string TitleText { get; set; } = "Loading...";

    [ObservableProperty]
    public partial bool IsBusy { get; set; } = true;

    public bool IsPinDot1Filled => EnteredPin.Length >= 1;
    public bool IsPinDot2Filled => EnteredPin.Length >= 2;
    public bool IsPinDot3Filled => EnteredPin.Length >= 3;
    public bool IsPinDot4Filled => EnteredPin.Length >= 4;
    public bool IsPinDot5Filled => EnteredPin.Length >= 5;
    public bool IsPinDot6Filled => EnteredPin.Length >= 6;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var hasPin = await authService.HasPinSetupAsync();
            _isRegistering = !hasPin;

            TitleText = _isRegistering
                ? "Create your 6-digit PIN"
                : "Enter your PIN";

            _isInitialized = true;
        }
        catch
        {
            TitleText = "SmartWallet";
            ErrorMessage = "Unable to load wallet data.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddDigitAsync(string digit)
    {
        if (IsBusy || EnteredPin.Length >= 6) return;

        EnteredPin += digit;

        if (EnteredPin.Length == 6)
        {
            await SubmitPinAsync();
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

    private async Task SubmitPinAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAsync();
            if (!_isInitialized)
                return;
        }

        if (EnteredPin.Length != 6) return;

        ErrorMessage = string.Empty;
        IsBusy = true;

        try
        {
            if (_isRegistering)
            {
                await authService.SetPinAsync(EnteredPin);
                navigationService.ShowMainApp();
                return;
            }

            var isSuccess = await authService.VerifyPinAsync(EnteredPin);

            if (isSuccess)
            {
                navigationService.ShowMainApp();
                return;
            }

            ErrorMessage = "Wrong PIN.";
            await Task.Delay(600);
            EnteredPin = string.Empty;
        }
        catch
        {
            ErrorMessage = "Something went wrong. Try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
