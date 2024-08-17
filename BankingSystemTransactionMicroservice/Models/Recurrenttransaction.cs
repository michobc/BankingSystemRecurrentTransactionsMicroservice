using System;
using System.Collections.Generic;

namespace BankingSystemTransactionMicroservice.Models;

public partial class Recurrenttransaction
{
    public int Recurrenttransactionidmicro { get; set; }

    public int Recurrenttransactionid { get; set; }

    public int Accountid { get; set; }

    public decimal Amount { get; set; }

    public string Transactiontype { get; set; } = null!;

    public DateTime? Createdat { get; set; }

    public string Frequency { get; set; } = null!;

    public DateTime Nexttransactiondate { get; set; }

    public string Branchid { get; set; } = null!;
}
