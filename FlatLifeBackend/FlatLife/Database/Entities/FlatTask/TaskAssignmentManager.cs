using System;

namespace FlatLife.Database.Entities;

public class TaskAssignmentManager
{
    public TimeSpan DefaultFrequency { get; set; } = TimeSpan.FromDays(7);
    public DateTime NextChangeDate { get; set; } = DateTime.UtcNow;
    public int CurrentResidentIndex { get; set; } = 0;
    
}
