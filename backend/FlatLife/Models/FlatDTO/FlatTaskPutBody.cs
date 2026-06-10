using System;
using System.Text.Json.Serialization;

namespace FlatLife.Models.FlatDTO;

public class FlatTaskPutBody
{
    public string? TaskName { get; set; }
    public TimeSpan? Frequency { get; set; }

}
