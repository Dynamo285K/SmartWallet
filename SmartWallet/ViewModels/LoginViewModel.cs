using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private readonly IAppNavigationService _navigationService;

    private bool _isRegistering;
    private bool _isInitialized;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PinDisplay))]
    private string enteredPin = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string titleText = "Loading...";

    [ObservableProperty]
    private bool isBusy = true;

    public string PinDisplay
    {
        get
        {
            string filled = new string('⬤', EnteredPin.Length);
            string empty = new string('○', 6 - EnteredPin.Length);
            return string.Join(" ", (filled + empty).ToCharArray());
        }
    }

    public LoginViewModel(AuthService authService, IAppNavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var hasPin = await _authService.HasPinSetupAsync();
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
                await _authService.SetPinAsync(EnteredPin);
                _navigationService.ShowMainApp();
                return;
            }

            var isSuccess = await _authService.VerifyPinAsync(EnteredPin);

            if (isSuccess)
            {
                _navigationService.ShowMainApp();
                return;
            }

            ErrorMessage = "Wrong PIN.";
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