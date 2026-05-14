using SmartWallet.Models.Interfaces;

namespace SmartWallet.Models.Services;

public class AppNavigationService(IServiceProvider serviceProvider) : IAppNavigationService
{
    public void ShowMainApp()
    {
        var shell = serviceProvider.GetRequiredService<AppShell>();
        
        var window = Application.Current?.Windows is [var firstWindow, ..] ? firstWindow : null;

        if (window is null)
            throw new InvalidOperationException("The application window is not available yet.");

        window.Page = shell;
    }
}
