using SQLite;
using SmartWallet.Models.Entities;

namespace SmartWallet.Models.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _db;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_db != null)
            return _db;

        await _initLock.WaitAsync();
        try
        {
            if (_db != null)
                return _db;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "SmartWallet.db3");

            _db = new SQLiteAsyncConnection(dbPath);

            await _db.CreateTableAsync<User>();
            await _db.CreateTableAsync<Transaction>();

            // Migrate existing transactions that have no UserId assigned yet
            await _db.ExecuteAsync(
                "UPDATE Transactions SET UserId = (SELECT Id FROM Users LIMIT 1) WHERE UserId = 0");

            return _db;
        }
        finally
        {
            _initLock.Release();
        }
    }
}
