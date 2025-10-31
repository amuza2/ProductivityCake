using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ProductivityCake.Models;

namespace ProductivityCake.Services;

public class JsonDataService : IJsonDataService
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new (1, 1);
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonDataService(string fileName)
    {
        var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        Directory.CreateDirectory(dataDirectory);
        
        _filePath = Path.Combine(dataDirectory, fileName);

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }
    
    public async Task<List<TodoItem>> GetAllAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return await LoadItemsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return new List<TodoItem>();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<List<TodoItem>> LoadItemsAsync()
    {
        if (!File.Exists(_filePath))
            return new List<TodoItem>();

        var json = await File.ReadAllTextAsync(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new List<TodoItem>();

        var items = JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.ListTodoItem);
        return items ?? new List<TodoItem>();
    }

    public async Task<TodoItem> GetByIdAsync(int id)
    {
        var items = await GetAllAsync();
        return items.FirstOrDefault(t => t.Id == id)!;
    }

    public async Task<bool> CreateAsync(TodoItem todoItem)
    {
        await  _semaphore.WaitAsync();
        try
        {
            var items = await LoadItemsAsync();

            if (todoItem.Id == 0)
            {
                var nextId = items.Count > 0 ? items.Max(t => t.Id) + 1 : 1;
                todoItem.Id = nextId;
            }
            
            items.Add(todoItem);
            return await SaveChangesAsync(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> UpdateAsync(TodoItem todoItem)
    {
        await _semaphore.WaitAsync();
        try
        {
            var items = await LoadItemsAsync();
            var index = items.FindIndex(t => t.Id == todoItem.Id);
            if (index == -1)
                return false;

            items[index] = todoItem;
            return await SaveChangesAsync(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
        
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _semaphore.WaitAsync();
        try
        {
            var items = await LoadItemsAsync();
            var index = items.FindIndex(t => t.Id == id);

            if (index == -1)
                return false;
            
            items.RemoveAt(index);
            return await SaveChangesAsync(items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task<bool> SaveChangesAsync(List<TodoItem> items)
    {
        try
        {
            var json = JsonSerializer.Serialize(items, AppJsonSerializerContext.Default.ListTodoItem);
            await File.WriteAllTextAsync(_filePath, json);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
}
