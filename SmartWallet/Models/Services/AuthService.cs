using SmartWallet.Models.Entities;

namespace SmartWallet.Models.Services;

public class AuthService(UserService userService)
{
    private const string DefaultUsername = "WalletOwner";

    public async Task<bool> HasPinSetupAsync()
    {
        var user = await userService.GetUserByUsernameAsync(DefaultUsername);
        return user != null;
    }

    public async Task SetPinAsync(string pin)
    {
        if (!IsValidPin(pin))
            throw new ArgumentException("PIN must contain exactly 6 digits.", nameof(pin));

        var hashedPin = BCrypt.Net.BCrypt.HashPassword(pin);
                
        var newUser = new User 
        { 
            Username = DefaultUsername,
            PasswordHash = hashedPin 
        };

        await userService.SaveUserAsync(newUser);
    }

    public async Task<bool> VerifyPinAsync(string enteredPin)
    {
        if (!IsValidPin(enteredPin))
            return false;

        var user = await userService.GetUserByUsernameAsync(DefaultUsername);

        return user != null && BCrypt.Net.BCrypt.Verify(enteredPin, user.PasswordHash);
    }

    private static bool IsValidPin(string pin)
    {
        return pin.Length == 6 && pin.All(char.IsDigit);
    }
}
