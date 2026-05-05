using System.Collections.ObjectModel;

namespace SmartWallet.Models.Entities;

public class TransactionGroup : ObservableCollection<Transaction>
{
    public string Name { get; }

    public TransactionGroup(string name, IEnumerable<Transaction> transactions) : base(transactions)
    {
        Name = name;
    }
}
