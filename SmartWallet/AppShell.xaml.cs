using SmartWallet.Views;

namespace SmartWallet;

public partial class AppShell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(ChangePinView), typeof(ChangePinView));
        
        Routing.RegisterRoute(nameof(TransactionFormView), typeof(TransactionFormView));
    }
}
