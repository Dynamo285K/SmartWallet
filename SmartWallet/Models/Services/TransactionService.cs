using SmartWallet.Models.Entities;

namespace SmartWallet.Models.Services;

public class TransactionService(DatabaseService databaseService)
{
    public async Task<int> AddTransactionAsync(Transaction transaction)
    {
        var db = await databaseService.GetDatabaseAsync();
        return await db.InsertAsync(transaction);
    }

    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        var db = await databaseService.GetDatabaseAsync();

        return await db.Table<Transaction>()
            .OrderByDescending(transaction => transaction.Date)
            .ToListAsync();
    }

    public async Task<int> DeleteTransactionAsync(Transaction transaction)
    {
        var db = await databaseService.GetDatabaseAsync();
        return await db.DeleteAsync(transaction);
    }

    public async Task<int> UpdateTransactionAsync(Transaction transaction)
    {
        var db = await databaseService.GetDatabaseAsync();
        return await db.UpdateAsync(transaction);
    }
}
