using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class TransactionsViewModel(
    TransactionService transactionService,
    IDialogService dialogService,
    IWalletNavigationService navigationService) : ObservableObject
{
    [ObservableProperty]
    public partial bool IsLoading { get; set; }
    
    [ObservableProperty]
    public partial bool IsRefreshing { get; set; }
    
    private List<Transaction> _allTransactionsBackup = [];
    
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial string SelectedCategory { get; set; } = "All Categories";

    [ObservableProperty]
    public partial string SelectedPeriod { get; set; } = "All Time";

    public ObservableCollection<string> CategoriesFilter { get; } = ["All Categories"];
    public ObservableCollection<string> PeriodsFilter { get; } = ["All Time"];
    public ObservableCollection<TransactionGroup> GroupedTransactions { get; set; } = [];

    public async Task LoadTransactionsAsync()
    {
        IsLoading = true;
        try
        {
            var data = await transactionService.GetAllTransactionsAsync();
            _allTransactionsBackup = data.ToList();
            
            PopulateFilters();
            ApplyFilters();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await dialogService.ShowAlertAsync("Error", "Unable to load transactions.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshTransactionsAsync()
    {
        IsRefreshing = true;
        try
        {
            await LoadTransactionsAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
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

    partial void OnSearchTextChanged(string _) => ApplyFilters();
    partial void OnSelectedCategoryChanged(string _) => ApplyFilters();
    partial void OnSelectedPeriodChanged(string _) => ApplyFilters();

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
    private async Task DeleteTransactionAsync(Transaction? transaction)
    {
        if (transaction == null) return;

        var isConfirmed = await dialogService.ShowConfirmationAsync(
            "Delete transaction?", 
            $"Do you really want to delete transaction '{transaction.Category}' with amount {transaction.Amount:C2}?", 
            "Delete", "Cancel");

        if (isConfirmed)
        {
            await transactionService.DeleteTransactionAsync(transaction);
            _allTransactionsBackup.RemoveAll(t => t.Id == transaction.Id);
            
            ApplyFilters();
        }
    }

    [RelayCommand]
    private async Task EditTransactionAsync(Transaction? transaction)
    {
        if (transaction == null) return;

        await navigationService.GoToEditTransactionAsync(transaction);
    }
}
