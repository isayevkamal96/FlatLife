using System;
using System.Text.Json.Serialization;

public class TodoItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("task")]
    public string Task { get; set; } = string.Empty;

    [JsonPropertyName("isChecked")]
    public bool IsChecked { get; set; }

    public bool IsEditing { get; set; }
    public string EditingText { get; set; } = string.Empty;

    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }

    [JsonPropertyName("updatedDate")]
    public DateTime UpdatedDate { get; set; }

    [JsonPropertyName("flatID")]
    public int FlatID { get; set; }

    [JsonPropertyName("createdByUserName")]
    public string CreatedByUserName { get; set; } = string.Empty;
}
