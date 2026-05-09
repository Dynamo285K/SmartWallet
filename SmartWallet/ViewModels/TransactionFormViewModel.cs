using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class TransactionFormViewModel(
    TransactionService transactionService,
    IDialogService dialogService,
    IWalletNavigationService navigationService) : ObservableObject, IQueryAttributable
{
    private const string CustomCategoryOption = "➕ Add Custom...";

    private int? _editingTransactionId;
    private DateTime _originalDate = DateTime.Now;
    
    private string _categoryToSelectAfterLoad = string.Empty;

    [ObservableProperty]
    public partial bool IsIncome { get; set; }

    [ObservableProperty]
    public partial string Amount { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SelectedCategory { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Note { get; set; } = string.Empty;

    public ObservableCollection<string> Categories { get; } = new();

    public string TitleText => _editingTransactionId.HasValue 
        ? (IsIncome ? "Edit Income" : "Edit Expense") 
        : (IsIncome ? "New Income" : "New Expense");

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("TransactionToEdit", out var obj) && obj is Transaction existingTransaction)
        {
            _editingTransactionId = existingTransaction.Id;
            _originalDate = existingTransaction.Date;
            
            IsIncome = existingTransaction.IsIncome;
            Amount = existingTransaction.Amount.ToString("0.##");
            Note = existingTransaction.Note;
            
            _categoryToSelectAfterLoad = existingTransaction.Category;
        }
        else if (query.TryGetValue("IsIncome", out var isIncomeValue) &&
            bool.TryParse(isIncomeValue.ToString(), out var isIncome))
        {
            _editingTransactionId = null;
            _originalDate = DateTime.Now;
            
            IsIncome = isIncome;
            Amount = string.Empty;
            Note = string.Empty;
            
            _categoryToSelectAfterLoad = string.Empty;
        }
        
        OnPropertyChanged(nameof(TitleText));

        _ = LoadCategoriesAsync();
    }

    partial void OnIsIncomeChanged(bool value)
    {
        OnPropertyChanged(nameof(TitleText));
    }

    private async Task LoadCategoriesAsync()
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
        
        var allTransactions = await transactionService.GetAllTransactionsAsync();

        var usedCategories = allTransactions
            .Where(t => t.IsIncome == IsIncome)
            .Select(t => t.Category)
            .Distinct()
            .ToList();

        foreach (var customCat in usedCategories.Where(c => !Categories.Contains(c) && !string.IsNullOrWhiteSpace(c)))
        {
            Categories.Add(customCat);
        }

        Categories.Add(CustomCategoryOption);

        if (!string.IsNullOrEmpty(_categoryToSelectAfterLoad) && Categories.Contains(_categoryToSelectAfterLoad))
        {
            SelectedCategory = _categoryToSelectAfterLoad;
        }
        else
        {
            SelectedCategory = Categories.FirstOrDefault() ?? string.Empty;
        }
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
        var newCategory = await dialogService.ShowPromptAsync(
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
        if (!decimal.TryParse(Amount, out var parsedAmount) || parsedAmount <= 0)
        {
            await dialogService.ShowAlertAsync("Error", "Enter a valid amount.");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedCategory) || SelectedCategory == CustomCategoryOption)
        {
            await dialogService.ShowAlertAsync("Error", "Please select a valid category.");
            return;
        }

        var isConfirmed = await dialogService.ShowConfirmationAsync(
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
            await transactionService.UpdateTransactionAsync(transaction);
        }
        else
        {
            await transactionService.AddTransactionAsync(transaction);
        }

        await navigationService.GoBackAsync();
    }
}