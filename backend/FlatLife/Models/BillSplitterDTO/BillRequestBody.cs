using System;
using System.ComponentModel.DataAnnotations;

namespace FlatLife.Models.BillSplitterDTO;

public class BillRequestBody
{
    [Required]
    public string title { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive")]
    public decimal amount { get; set; }

    [Required]
    public DateOnly monthOfPurchase { get; set; }
}
