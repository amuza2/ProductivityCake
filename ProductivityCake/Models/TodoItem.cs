using System;
using System.Text.Json.Serialization;
using ProductivityCake.Enums;

namespace ProductivityCake.Models;

public class TodoItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("isCompleted")]
    public bool IsComplete { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("dueDate")]
    public DateTimeOffset DueDate { get; set; }
    
    [JsonPropertyName("completedAt")]
    public DateTimeOffset? CompletedAt { get; set; }
    
    [JsonPropertyName("priority")]
    public TodoPriority Priority { get; set; }
}