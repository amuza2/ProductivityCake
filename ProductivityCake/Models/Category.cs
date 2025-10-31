using System.Text.Json.Serialization;

namespace ProductivityCake.Models;

public class Category
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("color")]
    public string Color { get; set; } = "#2196F3"; // Default blue
    
    public override string ToString() => Name;
}