using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatLife.Database.Entities;

public class FlatTask
{
    [Key]
    public int Id { get; set; }
    public string TaskName { get; set; }
    public TimeSpan Frequency { get; set; }
    public DateTime LastRotationDate { get; set; } = DateTime.UtcNow;
    
    [NotMapped]
    public DateTime Deadline => TimeZoneInfo.ConvertTimeFromUtc(LastRotationDate + Frequency, TimeZoneInfo.Local);

    public int CurrentUserId { get; set; }
    public User CurrentUser { get; set; }
    public int FlatId { get; set; }

    [ForeignKey("FlatId")]
    public Flat Flat { get; set; }

}
