using System.Collections.ObjectModel;

namespace SmartWallet.Models.Entities;

public class TransactionGroup(string name, IEnumerable<Transaction> transactions) : ObservableCollection<Transaction>(transactions)
{
    public string Name { get; } = name;
}
