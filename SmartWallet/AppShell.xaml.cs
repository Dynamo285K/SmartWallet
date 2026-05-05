namespace SmartWallet;
using SmartWallet.Views;
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute(nameof(ChangePinView), typeof(ChangePinView));
        
        Routing.RegisterRoute(nameof(TransactionFormView), typeof(TransactionFormView));
    }
}
