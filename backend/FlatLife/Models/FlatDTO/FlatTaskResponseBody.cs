using System;
using FlatLife.Database.Entities;

namespace FlatLife.Models.FlatDTO;

public class FlatTaskResponseBody
{
    public int Id { get; set; }
    public string TaskName { get; set; }
    public DateTime Deadline { get; set; }
    public TimeSpan Frequency { get; set; }
    public string AssignedToUserName { get; set; }


}
