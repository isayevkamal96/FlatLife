using System;

namespace FlatLifeFrontend.Models.BillSplitterModels;

public class TransactionDTO
{
    public string From { get; set; }
    public string To { get; set; }
    public decimal Amount { get; set; }
}
