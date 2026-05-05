using SQLite;

namespace SmartWallet.Models.Entities;

[Table("Users")]
public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
}
