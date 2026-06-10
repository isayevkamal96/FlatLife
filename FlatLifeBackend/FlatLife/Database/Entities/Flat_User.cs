using System;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using FlatLife.Database.Entities;

public class FlatUser
{
    [Required]
    [Key]
    public int id { get; set; }

    [ForeignKey("flat.id")]
    public int flatID { get; set; }

    [ForeignKey("user.id")]
    public int userid { get; set; } 

    public bool IsActive { get; set; }
}
