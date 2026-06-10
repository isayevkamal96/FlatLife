using System;

namespace FlatLife.Models.BillSplitterDTO;

public class TransactionDTO
{
    public string From { get; set; }
    public string To { get; set; }
    public decimal Amount { get; set; }
}
