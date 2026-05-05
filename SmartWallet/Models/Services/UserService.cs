using SmartWallet.Models.Entities;

namespace SmartWallet.Models.Services;

public class UserService(DatabaseService databaseService)
{
    public async Task<int> SaveUserAsync(User user)
    {
        var db = await databaseService.GetDatabaseAsync();
        var existingUser = await GetUserByUsernameAsync(user.Username);

        if (existingUser is null)
            return await db.InsertAsync(user);

        user.Id = existingUser.Id;
        return await db.UpdateAsync(user);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var db = await databaseService.GetDatabaseAsync();
        return await db.Table<User>().Where(user => user.Username == username).FirstOrDefaultAsync();
    }
}
