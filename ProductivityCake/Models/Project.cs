using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ProductivityCake.Models;

public class Project
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    [JsonPropertyName("isArchived")]
    public bool IsArchived { get; set; } = false;
    
    [JsonIgnore]
    public ICollection<TodoItem> Tasks { get; set; } = new List<TodoItem>();
    
    // Computed properties for task counts
    [JsonIgnore]
    public int TodoTasksCount => Tasks.Count(t => t.Status == TaskStatus.ToDo);
    
    [JsonIgnore]
    public int DoingTasksCount => Tasks.Count(t => t.Status == TaskStatus.Doing);
    
    [JsonIgnore]
    public int DoneTasksCount => Tasks.Count(t => t.Status == TaskStatus.Done);
    
    [JsonIgnore]
    public int TotalTasksCount => Tasks.Count;
    
    // Computed properties for project stats
    [JsonIgnore]
    public double CompletionPercentage => TotalTasksCount > 0 
        ? Math.Round((double)DoneTasksCount / TotalTasksCount * 100, 0) 
        : 0;
    
    [JsonIgnore]
    public DateTimeOffset? LastUpdated => Tasks.Any() 
        ? Tasks.Max(t => t.CompletedAt ?? t.CreatedAt) 
        : null;
}
