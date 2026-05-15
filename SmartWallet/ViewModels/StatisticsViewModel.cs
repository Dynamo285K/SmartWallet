using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Services;
using SmartWallet.Models.Interfaces;

namespace SmartWallet.ViewModels;

public partial class StatisticsViewModel(
    TransactionService transactionService,
    IDialogService dialogService) : ObservableObject
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

    [ObservableProperty]
    public partial IEnumerable<ISeries> ExpenseSeries { get; set; } = [];

    [ObservableProperty]
    public partial IEnumerable<ISeries> IncomeSeries { get; set; } = [];

    public ObservableCollection<string> Periods { get; } = [];

    public async Task LoadDataAsync()
    {
        try
        {
            var data = await transactionService.GetAllTransactionsAsync();
            _allTransactions = data.ToList();

            PopulatePeriods();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await dialogService.ShowAlertAsync("Error", "Unable to load statistics.");
        }
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
            uniquePeriods.Insert(0, currentMonthStr);

        foreach (var p in uniquePeriods)
            Periods.Add(p);

        if (string.IsNullOrEmpty(SelectedPeriod) || !Periods.Contains(SelectedPeriod))
            SelectedPeriod = currentMonthStr;

        UpdateStatistics();
    }

    // Has to be value so IDE will not throw warning:
    // Parameter name differs between partial member declarations
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

        var expensesGrouped = currentMonthData
            .Where(t => !t.IsIncome)
            .GroupBy(t => t.Category)
            .Select(g => new
            {
                Category = string.IsNullOrWhiteSpace(g.Key) ? "Uncategorized" : g.Key,
                Total = g.Sum(t => t.Amount)
            })
            .OrderByDescending(e => e.Total)
            .ToList();

        HasExpenses = expensesGrouped.Count > 0;
        IsExpenseEmpty = !HasExpenses;

        ExpenseSeries = expensesGrouped.Select(e => new PieSeries<double>
        {
            Values = [(double)e.Total],
            Name = e.Category,
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsSize = 14,
            DataLabelsPosition = PolarLabelsPosition.Middle,
            DataLabelsFormatter = p => $"€{p.Coordinate.PrimaryValue:0.##}",
            ToolTipLabelFormatter = p => $"{e.Category}: €{p.Coordinate.PrimaryValue:0.##}"
        }).ToList();

        var incomeGrouped = currentMonthData
            .Where(t => t.IsIncome)
            .GroupBy(t => t.Category)
            .Select(g => new
            {
                Category = string.IsNullOrWhiteSpace(g.Key) ? "Uncategorized" : g.Key,
                Total = g.Sum(t => t.Amount)
            })
            .OrderByDescending(i => i.Total)
            .ToList();

        HasIncome = incomeGrouped.Count > 0;
        IsIncomeEmpty = !HasIncome;

        IncomeSeries = incomeGrouped.Select(i => new PieSeries<double>
        {
            Values = [(double)i.Total],
            Name = i.Category,
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsSize = 14,
            DataLabelsPosition = PolarLabelsPosition.Middle,
            DataLabelsFormatter = p => $"€{p.Coordinate.PrimaryValue:0.##}",
            ToolTipLabelFormatter = p => $"{i.Category}: €{p.Coordinate.PrimaryValue:0.##}"
        }).ToList();
    }
}
