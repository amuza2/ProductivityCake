using System.Collections.Generic;
using System.Threading.Tasks;
using ProductivityCake.Models;

namespace ProductivityCake.Services;

public interface IJsonDataService
{
    Task<List<TodoItem>> GetAllAsync();
    Task<TodoItem> GetByIdAsync(int id);
    Task<bool> CreateAsync(TodoItem todoItem);
    Task<bool> UpdateAsync(TodoItem todoItem);
    Task<bool> DeleteAsync(int id);
}