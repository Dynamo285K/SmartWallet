using System.Text.Json;

namespace SmartWallet.Models.Services;

public interface IExportService
{
    Task<bool> ExportTransactionsToJsonAsync();
}

public class ExportService(TransactionService transactionService) : IExportService
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
}
