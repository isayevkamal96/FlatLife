using System;

namespace FlatLifeFrontend.Entities;

public class HouseholdTask
{
    public int id { get; set; }
    public string taskName { get; set; } = "";
    public string assignedToUserName { get; set; } = "";
    public DateTime? deadline { get; set; }
    public string frequency { get; set;} = "";

}
