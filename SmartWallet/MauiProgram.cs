using Microsoft.Extensions.Logging;
using SmartWallet.Models.Services;
using SmartWallet.ViewModels;
using SmartWallet.Views;
using Syncfusion.Maui.Toolkit.Hosting;

namespace SmartWallet;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .ConfigureSyncfusionToolkit()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<TransactionService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<IAppNavigationService, AppNavigationService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IWalletNavigationService, WalletNavigationService>();
        builder.Services.AddSingleton<IExportService, ExportService>();

        builder.Services.AddTransient<AppShell>();
        
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginView>();
        
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomeView>();
        
        builder.Services.AddTransient<TransactionFormViewModel>();
        builder.Services.AddTransient<TransactionFormView>();
        
        builder.Services.AddTransient<TransactionsView>();
        builder.Services.AddTransient<TransactionsViewModel>();
        
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SettingsView>();

        builder.Services.AddTransient<ChangePinViewModel>();
        builder.Services.AddTransient<ChangePinView>();
        
        builder.Services.AddTransient<StatisticsViewModel>();
        builder.Services.AddTransient<StatisticsView>();
        
        return builder.Build();
    }
}
