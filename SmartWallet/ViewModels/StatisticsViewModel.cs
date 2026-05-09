using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class StatisticsViewModel(TransactionService transactionService) : ObservableObject
{
    private List<Transaction> _allTransactions = [];

    [ObservableProperty]
    public partial decimal TotalIncome { get; set; }

    [ObservableProperty]
    public partial decimal TotalExpense { get; set; }

    [ObservableProperty]
    public partial decimal NetBalance { get; set; }

    [ObservableProperty]
    public partial bool HasIncome { get; set; }
    
    [ObservableProperty]
    public partial bool HasExpenses { get; set; }

    [ObservableProperty]
    public partial bool IsExpenseEmpty { get; set; }
    
    [ObservableProperty]
    public partial bool IsIncomeEmpty { get; set; }

    [ObservableProperty]
    public partial string SelectedPeriod { get; set; } = string.Empty;

    public ObservableCollection<string> Periods { get; } = [];
    
    public ObservableCollection<ExpenseChartItem> ExpensesByCategory { get; } = [];
    
    public ObservableCollection<ExpenseChartItem> IncomeByCategory { get; } = [];

    public async Task LoadDataAsync()
    {
        var data = await transactionService.GetAllTransactionsAsync();
        _allTransactions = data.ToList();

        PopulatePeriods();
    }

    private void PopulatePeriods()
    {
        Periods.Clear();
        
        var uniquePeriods = _allTransactions
            .Select(t => new DateTime(t.Date.Year, t.Date.Month, 1))
            .Distinct()
            .OrderByDescending(d => d)
            .Select(d => d.ToString("MMMM yyyy").ToUpper())
            .ToList();

        var currentMonthStr = DateTime.Now.ToString("MMMM yyyy").ToUpper();
        if (!uniquePeriods.Contains(currentMonthStr))
        {
            uniquePeriods.Insert(0, currentMonthStr);
        }

        foreach (var p in uniquePeriods)
        {
            Periods.Add(p);
        }

        if (string.IsNullOrEmpty(SelectedPeriod) || !Periods.Contains(SelectedPeriod))
        {
            SelectedPeriod = currentMonthStr; 
        }
        else 
        {
            UpdateStatistics();
        }
    }

    partial void OnSelectedPeriodChanged(string value)
    {
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        if (string.IsNullOrEmpty(SelectedPeriod)) return;

        var currentMonthData = _allTransactions
            .Where(t => string.Equals(
                new DateTime(t.Date.Year, t.Date.Month, 1).ToString("MMMM yyyy"), 
                SelectedPeriod, 
                StringComparison.OrdinalIgnoreCase))
            .ToList();

        TotalIncome = currentMonthData.Where(t => t.IsIncome).Sum(t => t.Amount);
        TotalExpense = currentMonthData.Where(t => !t.IsIncome).Sum(t => t.Amount);
        NetBalance = TotalIncome - TotalExpense;
        
        ExpensesByCategory.Clear();
        IncomeByCategory.Clear();
        
        var expensesGrouped = currentMonthData
            .Where(t => !t.IsIncome)
            .GroupBy(t => t.Category)
            .Select(g => new
            {
                Category = string.IsNullOrWhiteSpace(g.Key) ? "Uncategorized" : g.Key,
                Total = g.Sum(t => t.Amount)
            })
            .OrderByDescending(expense => expense.Total)
            .ToList();

        HasExpenses = expensesGrouped.Count > 0;
        IsExpenseEmpty = !HasExpenses;

        foreach (var expense in expensesGrouped)
        {
            ExpensesByCategory.Add(new ExpenseChartItem
            {
                Category = expense.Category,
                Amount = (double)expense.Total
            });
        }
        
        var incomeGrouped = currentMonthData
            .Where(t => t.IsIncome)
            .GroupBy(t => t.Category)
            .Select(g => new
            {
                Category = string.IsNullOrWhiteSpace(g.Key) ? "Uncategorized" : g.Key,
                Total = g.Sum(t => t.Amount)
            })
            .OrderByDescending(income => income.Total)
            .ToList();

        HasIncome = incomeGrouped.Count > 0;
        IsIncomeEmpty = !HasIncome;

        foreach (var income in incomeGrouped)
        {
            IncomeByCategory.Add(new ExpenseChartItem
            {
                Category = income.Category,
                Amount = (double)income.Total
            });
        }
    }
}