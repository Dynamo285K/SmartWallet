using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class TransactionFormViewModel : ObservableObject, IQueryAttributable
{
    private const string CustomCategoryOption = "➕ Add Custom...";

    private readonly TransactionService _transactionService;
    private readonly IDialogService _dialogService;
    private readonly IWalletNavigationService _navigationService;

    private int? _editingTransactionId;
    private DateTime _originalDate = DateTime.Now;

    [ObservableProperty]
    private bool _isIncome;

    [ObservableProperty]
    private string _amount = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = string.Empty;

    [ObservableProperty]
    private string _note = string.Empty;

    public ObservableCollection<string> Categories { get; } = new();

    public string TitleText => _editingTransactionId.HasValue 
        ? (IsIncome ? "Edit Income" : "Edit Expense") 
        : (IsIncome ? "New Income" : "New Expense");

    public TransactionFormViewModel(
        TransactionService transactionService,
        IDialogService dialogService,
        IWalletNavigationService navigationService)
    {
        _transactionService = transactionService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        LoadCategories();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("TransactionToEdit", out var obj) && obj is Transaction existingTransaction)
        {
            _editingTransactionId = existingTransaction.Id;
            _originalDate = existingTransaction.Date;
            
            IsIncome = existingTransaction.IsIncome;
            Amount = existingTransaction.Amount.ToString("0.##");
            Note = existingTransaction.Note;
            
            LoadCategories();
            
            if (!Categories.Contains(existingTransaction.Category))
            {
                Categories.Insert(Categories.Count - 1, existingTransaction.Category);
            }
            SelectedCategory = existingTransaction.Category;
        }
        else if (query.TryGetValue("IsIncome", out var isIncomeValue) &&
            bool.TryParse(isIncomeValue?.ToString(), out var isIncome))
        {
            _editingTransactionId = null;
            _originalDate = DateTime.Now;
            
            IsIncome = isIncome;
            Amount = string.Empty;
            Note = string.Empty;
            
            LoadCategories();
        }
        
        OnPropertyChanged(nameof(TitleText));
    }

    partial void OnIsIncomeChanged(bool value)
    {
        OnPropertyChanged(nameof(TitleText));
        if (!_editingTransactionId.HasValue) 
        {
            LoadCategories();
        }
    }

    private void LoadCategories()
    {
        Categories.Clear();
        SelectedCategory = string.Empty;

        if (IsIncome)
        {
            Categories.Add("Salary");
            Categories.Add("Pocket Money / Gift");
            Categories.Add("Selling Items");
        }
        else
        {
            Categories.Add("Food & Groceries");
            Categories.Add("Restaurants & Coffee");
            Categories.Add("Transport & Car");
            Categories.Add("Entertainment & Leisure");
            Categories.Add("Housing & Bills");
        }
        
        Categories.Add(CustomCategoryOption);
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        if (value == CustomCategoryOption)
        {
            _ = AskForCustomCategoryAsync();
        }
    }

    private async Task AskForCustomCategoryAsync()
    {
        string? newCategory = await _dialogService.ShowPromptAsync(
            "New Category",
            "Enter the name of your custom category:");

        if (!string.IsNullOrWhiteSpace(newCategory))
        {
            Categories.Insert(Categories.Count - 1, newCategory);
            SelectedCategory = newCategory;
            return;
        }

        SelectedCategory = Categories.FirstOrDefault() ?? string.Empty;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!decimal.TryParse(Amount, out decimal parsedAmount) || parsedAmount <= 0)
        {
            await _dialogService.ShowAlertAsync("Error", "Enter a valid amount.");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedCategory) || SelectedCategory == CustomCategoryOption)
        {
            await _dialogService.ShowAlertAsync("Error", "Please select a valid category.");
            return;
        }

        bool isConfirmed = await _dialogService.ShowConfirmationAsync(
            "Confirmation", 
            $"Do you really want to save this transaction in the amount of {parsedAmount:C2}?", 
            "Yes, save", 
            "Cancel");

        if (!isConfirmed)
            return;

        var transaction = new Transaction
        {
            Id = _editingTransactionId ?? 0,
            Amount = parsedAmount,
            IsIncome = IsIncome,
            Category = SelectedCategory,
            Note = Note,
            Date = _originalDate
        };

        if (_editingTransactionId.HasValue)
        {
            await _transactionService.UpdateTransactionAsync(transaction);
        }
        else
        {
            await _transactionService.AddTransactionAsync(transaction);
        }

        await _navigationService.GoBackAsync();
    }
}
