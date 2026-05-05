using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class TransactionsViewModel : ObservableObject
{
    private readonly TransactionService _transactionService;
    private readonly IDialogService _dialogService;
    private readonly IWalletNavigationService _navigationService;

    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private bool _isRefreshing;
    
    private List<Transaction> _allTransactionsBackup = [];
    
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty]
    private string _selectedCategory = "All Categories";

    [ObservableProperty]
    private string _selectedPeriod = "All Time";

    public ObservableCollection<string> CategoriesFilter { get; } = ["All Categories"];
    public ObservableCollection<string> PeriodsFilter { get; } = ["All Time"];
    public ObservableCollection<TransactionGroup> GroupedTransactions { get; set; } = [];
    
    public TransactionsViewModel(
        TransactionService transactionService,
        IDialogService dialogService,
        IWalletNavigationService navigationService)
    {
        _transactionService = transactionService;
        _dialogService = dialogService;
        _navigationService = navigationService;
    }

    public async Task LoadTransactionsAsync()
    {
        IsLoading = true;

        var data = await _transactionService.GetAllTransactionsAsync();
        _allTransactionsBackup = data.ToList();
        
        // TOTO TU CHÝBALO: Musíme naplniť roletky a aplikovať filtre pri načítaní
        PopulateFilters();
        ApplyFilters();

        IsLoading = false;
    }

    [RelayCommand]
    private async Task RefreshTransactionsAsync()
    {
        IsRefreshing = true;
        await LoadTransactionsAsync();
        IsRefreshing = false;
    }
    
    private void PopulateFilters()
    {
        CategoriesFilter.Clear();
        CategoriesFilter.Add("All Categories");
        var uniqueCategories = _allTransactionsBackup.Select(t => t.Category).Distinct().OrderBy(c => c);
        foreach (var c in uniqueCategories) CategoriesFilter.Add(c);

        PeriodsFilter.Clear();
        PeriodsFilter.Add("All Time");
        var uniquePeriods = _allTransactionsBackup
            .Select(t => new DateTime(t.Date.Year, t.Date.Month, 1).ToString("MMMM yyyy").ToUpper())
            .Distinct();
        foreach (var p in uniquePeriods) PeriodsFilter.Add(p);

        if (!CategoriesFilter.Contains(SelectedCategory)) SelectedCategory = "All Categories";
        if (!PeriodsFilter.Contains(SelectedPeriod)) SelectedPeriod = "All Time";
    }

    // Tieto tri riadky automaticky reagujú na zmenu v UI a zavolajú filter
    partial void OnSearchTextChanged(string value) => ApplyFilters();
    partial void OnSelectedCategoryChanged(string value) => ApplyFilters();
    partial void OnSelectedPeriodChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        var filtered = _allTransactionsBackup.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(t => 
                t.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                t.Note.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedCategory != "All Categories" && !string.IsNullOrEmpty(SelectedCategory))
        {
            filtered = filtered.Where(t => t.Category == SelectedCategory);
        }

        if (SelectedPeriod != "All Time" && !string.IsNullOrEmpty(SelectedPeriod))
        {
            filtered = filtered.Where(t => new DateTime(t.Date.Year, t.Date.Month, 1).ToString("MMMM yyyy").ToUpper() == SelectedPeriod);
        }

        UpdateGroupedList(filtered.ToList());
    }
    
    private void UpdateGroupedList(List<Transaction> transactions)
    {
        GroupedTransactions.Clear();

        var groupedData = transactions
            .GroupBy(t => new { t.Date.Year, t.Date.Month })
            .OrderByDescending(g => g.Key.Year)
            .ThenByDescending(g => g.Key.Month)
            .Select(g => new TransactionGroup(
                name: new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy").ToUpper(),
                transactions: g.OrderByDescending(t => t.Date).ToList()
            ));

        foreach (var group in groupedData)
        {
            GroupedTransactions.Add(group);
        }
    }

    [RelayCommand]
    private async Task DeleteTransactionAsync(Transaction transaction)
    {
        if (transaction == null) return;

        var isConfirmed = await _dialogService.ShowConfirmationAsync(
            "Delete transaction?", 
            $"Do you really want to delete transaction '{transaction.Category}' with amount {transaction.Amount:C2}?", 
            "Delete", "Cancel");

        if (isConfirmed)
        {
            await _transactionService.DeleteTransactionAsync(transaction);
            _allTransactionsBackup.Remove(transaction);
            
            // Namiesto starého volania OnSearchTextChanged teraz voláme univerzálny filter
            ApplyFilters();
        }
    }

    [RelayCommand]
    private async Task EditTransactionAsync(Transaction transaction)
    {
        if (transaction == null) return;

        await _navigationService.GoToEditTransactionAsync(transaction);
    }
}
