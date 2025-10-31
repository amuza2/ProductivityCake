using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProductivityCake.Models;

public class TodoItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("isCompleted")]
    public bool IsComplete { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("dueDate")]
    public DateTimeOffset? DueDate { get; set; }
    
    [JsonPropertyName("completedAt")]
    public DateTimeOffset? CompletedAt { get; set; }
    
    [JsonPropertyName("projectId")]
    public int? ProjectId { get; set; }
    
    [JsonIgnore]
    public Project? Project { get; set; }
    
    [JsonPropertyName("estimatedPomodoros")]
    public int EstimatedPomodoros { get; set; } = 1;
    
    [JsonPropertyName("completedPomodoros")]
    public int CompletedPomodoros { get; set; }
    
    [JsonPropertyName("status")]
    public TaskStatus Status { get; set; } = TaskStatus.ToDo;
    
    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }
    
    [JsonIgnore]
    public Category? Category { get; set; }
    
    [JsonPropertyName("tagIds")]
    public List<int> TagIds { get; set; } = new();
    
    [JsonIgnore]
    public List<Category> Tags { get; set; } = new();
}
