using FlatLife.Database.Entities;
using FlatLife.Models;
using FlatLife.Models.UserDTO;

public class ToDoItem
{
    public int Id { get; set; }
    public string Task { get; set; }
    public bool IsChecked { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; }
    public int FlatID { get; set; }

    public string CreatedByUserName { get; set; }
}

public class CreateToDoItemDto
{
    public string Task { get; set; }
    public bool IsChecked { get; set; }
}

public class UpdateToDoItemDto
{
    public string Task { get; set; }
    public bool IsChecked { get; set; }
}
