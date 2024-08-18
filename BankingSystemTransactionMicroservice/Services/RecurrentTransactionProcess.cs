using System.Text;
using BankingSystemTransactionMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace BankingSystemTransactionMicroservice.Services;

public class RecurrentTransactionProcess
{
    private readonly BankingSystemMicroContext _context;
    private readonly CalculatNextTransaction _calculat;
    private readonly RabbitMqServiceEvent _rabbitMqService;

    public RecurrentTransactionProcess(BankingSystemMicroContext context, CalculatNextTransaction calculat, RabbitMqServiceEvent rabbitMqService)
    {
        _context = context;
        _calculat = calculat;
        _rabbitMqService = rabbitMqService;
    }

    public async Task ProcessRecurrentTransactions()
    {
        var now = DateTime.Now;

        var dueTransactions = await _context.Recurrenttransactions
            .Where(rt => rt.Nexttransactiondate <= now)
            .ToListAsync();

        var channel = _rabbitMqService.GetChannel();

        foreach (var transaction in dueTransactions)
        {
            transaction.Nexttransactiondate = _calculat.CalculateNextTransactionDate(transaction.Frequency, transaction.Nexttransactiondate);
                
            var eventMessage = new
            {
                RecurrentTransactionId = transaction.Recurrenttransactionid,
                AccountId = transaction.Accountid,
                TransactionType = transaction.Transactiontype,
                BranchId = transaction.Branchid,
                Amount = transaction.Amount,
                NextTransactionDate = transaction.Nexttransactiondate,
            };

            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventMessage));

            channel.BasicPublish(exchange: "",
                routingKey: "recurrent_transactions_exchange",
                basicProperties: null,
                body: messageBody);

            _context.Recurrenttransactions.Update(transaction);
        }

        await _context.SaveChangesAsync();
    }
}