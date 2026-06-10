using System;
using FlatLife.Database.Entities;

namespace FlatLife.Models.FlatDTO;

public class FlatTaskRequestBody
{
    public string TaskName { get; set; }
    public TimeSpan? Frequency { get; set; }

}
