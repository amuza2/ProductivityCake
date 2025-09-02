using System.Collections.Generic;
using System.Threading.Tasks;
using ProductivityCake.Enums;
using ProductivityCake.Models;

namespace ProductivityCake.Services;

public interface ITodoService
{
    Task<List<TodoItem>> GetFilteredTodosAsync(FilterSelection filter, string searchText = "");
    Task<bool> ToggleCompleteAsync(int id);
    Task<bool> CreateTodoAsync(TodoItem todo);
}