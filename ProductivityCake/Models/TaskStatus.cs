using System.Text.Json.Serialization;

namespace ProductivityCake.Models;

[JsonConverter(typeof(JsonStringEnumConverter<TaskStatus>))]
public enum TaskStatus
{
    ToDo = 0,
    Doing = 1,
    Done = 2
}
