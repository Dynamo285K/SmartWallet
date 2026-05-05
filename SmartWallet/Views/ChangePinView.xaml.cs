using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartWallet.ViewModels;

namespace SmartWallet.Views;

public partial class ChangePinView : ContentPage
{
    public ChangePinView(ChangePinViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}