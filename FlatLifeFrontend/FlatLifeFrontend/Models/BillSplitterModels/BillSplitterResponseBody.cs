using System;

namespace FlatLifeFrontend.Models.BillSplitterModels;

public class BillSplitterResponseBody
{
    public int id { get; set; }
    public string title { get; set; }
    public string buyer { get; set; } = "";
    public decimal amount { get; set; }
    public DateOnly monthOfPurchase { get; set; }
}
