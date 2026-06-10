using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatLife.Database.Entities;

public class Bill
{
    [Required]
    [Key]
    public int id { get; set; }
    [Required(AllowEmptyStrings = false)]
    public string title { get; set; }
    public string buyer { get; set; } = "";
    public decimal amount { get; set; } 
    public DateOnly monthOfPurchase { get; set; }
    public string imageBase64 { get; set; }
    public int userId { get; set; }
    public int flatId { get; set; }
}
