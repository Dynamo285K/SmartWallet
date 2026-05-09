using SQLite;

namespace SmartWallet.Models.Entities;

[Table("Transactions")]
public class Transaction
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; init; }

    public decimal Amount { get; set; }

    public bool IsIncome { get; set; }

    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Note { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}
