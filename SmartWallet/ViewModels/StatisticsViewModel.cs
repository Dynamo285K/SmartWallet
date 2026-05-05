using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartWallet.Models;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Services;

namespace SmartWallet.ViewModels;

public partial class StatisticsViewModel : ObservableObject
{
    private readonly TransactionService _transactionService;

    private List<Transaction> _allTransactions = [];

    [ObservableProperty]
    private decimal _totalIncome;

    [ObservableProperty]
    private decimal _totalExpense;

    [ObservableProperty]
    private decimal _netBalance;

    [ObservableProperty]
    private bool _hasExpenses;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _selectedPeriod = string.Empty;

    public ObservableCollection<string> Periods { get; } = [];
    
    public ObservableCollection<ExpenseChartItem> ExpensesByCategory { get; } = [];

    public StatisticsViewModel(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    public async Task LoadDataAsync()
    {
        var data = await _transactionService.GetAllTransactionsAsync();
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
            .Where(t => new DateTime(t.Date.Year, t.Date.Month, 1).ToString("MMMM yyyy").ToUpper() == SelectedPeriod)
            .ToList();

        TotalIncome = currentMonthData.Where(t => t.IsIncome).Sum(t => t.Amount);
        TotalExpense = currentMonthData.Where(t => !t.IsIncome).Sum(t => t.Amount);
        NetBalance = TotalIncome - TotalExpense;

        ExpensesByCategory.Clear();

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
        IsEmpty = !HasExpenses;

        foreach (var expense in expensesGrouped)
        {
            ExpensesByCategory.Add(new ExpenseChartItem
            {
                Category = expense.Category,
                Amount = (double)expense.Total
            });
        }
    }
}
