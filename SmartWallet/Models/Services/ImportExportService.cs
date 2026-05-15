using System.Text.Json;
using SmartWallet.Models.Entities;
using SmartWallet.Models.Interfaces;

namespace SmartWallet.Models.Services;

public class ImportExportService(
    TransactionService transactionService) : IImportExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public async Task<bool> ExportTransactionsToJsonAsync()
    {
        var transactions = await transactionService.GetAllTransactionsAsync();

        if (transactions.Count == 0)
            return false;

        var jsonString = JsonSerializer.Serialize(transactions, JsonOptions);
        var fileName = $"SmartWallet_Export_{DateTime.Now:yyyyMMdd}.json";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        await File.WriteAllTextAsync(filePath, jsonString);

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Export Transactions",
            File = new ShareFile(filePath)
        });

        return true;
    }

    public async Task<int> ImportTransactionsFromJsonAsync()
    {
        var fileResult = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select SmartWallet JSON file",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, ["public.json"] },
                { DevicePlatform.Android, ["application/json"] },
                { DevicePlatform.WinUI, [".json"] },
                { DevicePlatform.macOS, ["json"] },
            })
        });

        if (fileResult is null)
            return 0;

        var json = await File.ReadAllTextAsync(fileResult.FullPath);
        var imported = JsonSerializer.Deserialize<List<Transaction>>(json, JsonOptions);

        if (imported is null || imported.Count == 0)
            return 0;

        var addedCount = 0;

        foreach (var t in imported)
        {
            if (t.Amount <= 0 || string.IsNullOrWhiteSpace(t.Category))
                continue;

            var transaction = new Transaction
            {
                Amount = t.Amount,
                IsIncome = t.IsIncome,
                Category = t.Category,
                Note = t.Note,
                Date = t.Date
            };
            await transactionService.AddTransactionAsync(transaction);
            addedCount++;
        }

        return addedCount;
    }
}
