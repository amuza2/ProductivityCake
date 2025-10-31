using System.Collections.Generic;
using System.Text.Json.Serialization;
using ProductivityCake.Models;

namespace ProductivityCake.Services;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<Project>))]
[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(List<TodoItem>))]
[JsonSerializable(typeof(TodoItem))]
[JsonSerializable(typeof(List<Category>))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(TaskStatus))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}
