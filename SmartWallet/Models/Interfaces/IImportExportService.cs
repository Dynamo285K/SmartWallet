namespace SmartWallet.Models.Interfaces;

public interface IImportExportService
{
    Task<bool> ExportTransactionsToJsonAsync();
    Task<int> ImportTransactionsFromJsonAsync();
}
