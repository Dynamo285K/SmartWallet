using SmartWallet.Models.Entities;

namespace SmartWallet.Models.Services;

public class TransactionService(DatabaseService databaseService, UserService userService)
{
    private const string DefaultUsername = "WalletOwner";

    private async Task<int> GetCurrentUserIdAsync()
    {
        var user = await userService.GetUserByUsernameAsync(DefaultUsername);
        return user?.Id ?? 0;
    }

    public async Task<int> AddTransactionAsync(Transaction transaction)
    {
        var db = await databaseService.GetDatabaseAsync();
        transaction.UserId = await GetCurrentUserIdAsync();
        return await db.InsertAsync(transaction);
    }

    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        var db = await databaseService.GetDatabaseAsync();
        var userId = await GetCurrentUserIdAsync();

        return await db.Table<Transaction>()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<int> UpdateTransactionAsync(Transaction transaction)
    {
        var db = await databaseService.GetDatabaseAsync();
        transaction.UserId = await GetCurrentUserIdAsync();
        return await db.UpdateAsync(transaction);
    }

    public async Task<int> DeleteTransactionAsync(Transaction transaction)
    {
        var db = await databaseService.GetDatabaseAsync();
        return await db.DeleteAsync(transaction);
    }

    public async Task DeleteAllTransactionsAsync()
    {
        var db = await databaseService.GetDatabaseAsync();
        var userId = await GetCurrentUserIdAsync();
        await db.ExecuteAsync("DELETE FROM Transactions WHERE UserId = ?", userId);
    }
}
