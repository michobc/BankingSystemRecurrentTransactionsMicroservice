using BankingSystemTransactionMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemTransactionMicroservice.Services;

public class RecurrentTransactionProcess
{
    private readonly BankingSystemMicroContext _context;
    private readonly CalculatNextTransaction _calculat;

    public RecurrentTransactionProcess(BankingSystemMicroContext context, CalculatNextTransaction calculat)
    {
        _context = context;
        _calculat = calculat;
    }

    public async Task ProcessRecurrentTransactions()
    {
        var now = DateTime.Now;

        var dueTransactions = await _context.Recurrenttransactions
            .Where(rt => rt.Nexttransactiondate <= now)
            .ToListAsync();

        foreach (var transaction in dueTransactions)
        {
            // Update the existing transaction with new details
            transaction.Nexttransactiondate = _calculat.CalculateNextTransactionDate(transaction.Frequency, transaction.Nexttransactiondate);
            // Save the changes
            _context.Recurrenttransactions.Update(transaction);
        }

        await _context.SaveChangesAsync();
    }
}
