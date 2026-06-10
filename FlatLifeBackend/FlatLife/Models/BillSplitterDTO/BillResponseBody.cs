using System;

namespace FlatLife.Models.BillSplitterDTO;

public class BillResponseBody
{
    public int id { get; set; }
    public string title { get; set; }
    public string buyer { get; set; } = "";
    public decimal amount { get; set; } 
    public DateOnly monthOfPurchase { get; set; }
}
