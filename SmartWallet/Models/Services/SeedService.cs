using SmartWallet.Models.Entities;

namespace SmartWallet.Models.Services;

public class SeedService(TransactionService transactionService)
{
    public async Task<bool> SeedAsync()
    {
        var existing = await transactionService.GetAllTransactionsAsync();
        if (existing.Count > 0)
            return false;

        foreach (var transaction in GetSeedTransactions())
            await transactionService.AddTransactionAsync(transaction);

        return true;
    }

    private static List<Transaction> GetSeedTransactions()
    {
        var now = DateTime.Now;

        return
        [
            // ── January ──────────────────────────────────────────
            new() { Amount = 2500, IsIncome = true,  Category = "Salary",                  Note = "Monthly salary",         Date = new DateTime(now.Year, 1, 1)  },
            new() { Amount = 650,  IsIncome = false, Category = "Housing & Bills",          Note = "Rent",                   Date = new DateTime(now.Year, 1, 3)  },
            new() { Amount = 110,  IsIncome = false, Category = "Food & Groceries",         Note = "Weekly shopping",        Date = new DateTime(now.Year, 1, 8)  },
            new() { Amount = 55,   IsIncome = false, Category = "Restaurants & Coffee",     Note = "Dinner with friends",    Date = new DateTime(now.Year, 1, 14) },
            new() { Amount = 150,  IsIncome = false, Category = "Entertainment & Leisure",  Note = "Cinema, games",          Date = new DateTime(now.Year, 1, 20) },
            new() { Amount = 45,   IsIncome = false, Category = "Transport & Car",          Note = "Bus pass",               Date = new DateTime(now.Year, 1, 25) },

            // ── February ─────────────────────────────────────────
            new() { Amount = 2500, IsIncome = true,  Category = "Salary",                  Note = "Monthly salary",         Date = new DateTime(now.Year, 2, 1)  },
            new() { Amount = 200,  IsIncome = true,  Category = "Pocket Money / Gift",      Note = "Birthday gift",          Date = new DateTime(now.Year, 2, 10) },
            new() { Amount = 650,  IsIncome = false, Category = "Housing & Bills",          Note = "Rent",                   Date = new DateTime(now.Year, 2, 3)  },
            new() { Amount = 90,   IsIncome = false, Category = "Food & Groceries",         Note = "Weekly shopping",        Date = new DateTime(now.Year, 2, 9)  },
            new() { Amount = 45,   IsIncome = false, Category = "Transport & Car",          Note = "Fuel",                   Date = new DateTime(now.Year, 2, 15) },
            new() { Amount = 30,   IsIncome = false, Category = "Restaurants & Coffee",     Note = "Coffee with colleague",  Date = new DateTime(now.Year, 2, 22) },

            // ── March ────────────────────────────────────────────
            new() { Amount = 2500, IsIncome = true,  Category = "Salary",                  Note = "Monthly salary",         Date = new DateTime(now.Year, 3, 1)  },
            new() { Amount = 650,  IsIncome = false, Category = "Housing & Bills",          Note = "Rent",                   Date = new DateTime(now.Year, 3, 3)  },
            new() { Amount = 78,   IsIncome = false, Category = "Food & Groceries",         Note = "Weekly shopping",        Date = new DateTime(now.Year, 3, 7)  },
            new() { Amount = 65,   IsIncome = false, Category = "Restaurants & Coffee",     Note = "Team lunch",             Date = new DateTime(now.Year, 3, 12) },
            new() { Amount = 80,   IsIncome = false, Category = "Entertainment & Leisure",  Note = "Concert tickets",        Date = new DateTime(now.Year, 3, 18) },
            new() { Amount = 35,   IsIncome = false, Category = "Transport & Car",          Note = "Bus pass",               Date = new DateTime(now.Year, 3, 26) },

            // ── April ────────────────────────────────────────────
            new() { Amount = 2500, IsIncome = true,  Category = "Salary",                  Note = "Monthly salary",         Date = new DateTime(now.Year, 4, 1)  },
            new() { Amount = 150,  IsIncome = true,  Category = "Selling Items",            Note = "Sold old laptop",        Date = new DateTime(now.Year, 4, 5)  },
            new() { Amount = 650,  IsIncome = false, Category = "Housing & Bills",          Note = "Rent",                   Date = new DateTime(now.Year, 4, 3)  },
            new() { Amount = 95,   IsIncome = false, Category = "Food & Groceries",         Note = "Weekly shopping",        Date = new DateTime(now.Year, 4, 10) },
            new() { Amount = 120,  IsIncome = false, Category = "Entertainment & Leisure",  Note = "Gym membership",         Date = new DateTime(now.Year, 4, 15) },
            new() { Amount = 55,   IsIncome = false, Category = "Transport & Car",          Note = "Fuel",                   Date = new DateTime(now.Year, 4, 20) },
            new() { Amount = 40,   IsIncome = false, Category = "Restaurants & Coffee",     Note = "Lunch",                  Date = new DateTime(now.Year, 4, 25) },

            // ── May (current) ────────────────────────────────────
            new() { Amount = 2500, IsIncome = true,  Category = "Salary",                  Note = "Monthly salary",         Date = new DateTime(now.Year, 5, 1)  },
            new() { Amount = 650,  IsIncome = false, Category = "Housing & Bills",          Note = "Rent",                   Date = new DateTime(now.Year, 5, 3)  },
            new() { Amount = 85,   IsIncome = false, Category = "Food & Groceries",         Note = "Weekly shopping",        Date = new DateTime(now.Year, 5, 7)  },
            new() { Amount = 45,   IsIncome = false, Category = "Restaurants & Coffee",     Note = "Dinner",                 Date = new DateTime(now.Year, 5, 9)  },
            new() { Amount = 60,   IsIncome = false, Category = "Transport & Car",          Note = "Bus pass",               Date = new DateTime(now.Year, 5, 10) },
        ];
    }
}
