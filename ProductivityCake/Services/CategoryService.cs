using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ProductivityCake.Models;

namespace ProductivityCake.Services;

public class CategoryService
{
    private readonly string _filePath = "categories.json";
    private List<Category> _categories = new();
    private int _nextId = 1;
    
    public event EventHandler? CategoriesChanged;
    
    public CategoryService()
    {
        _ = LoadCategoriesAsync();
    }
    
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        if (_categories.Count == 0)
        {
            await LoadCategoriesAsync();
        }
        return _categories.ToList();
    }
    
    public async Task<Category> CreateCategoryAsync(string name, string color)
    {
        var category = new Category
        {
            Id = _nextId++,
            Name = name,
            Color = color
        };
        
        _categories.Add(category);
        await SaveCategoriesAsync();
        CategoriesChanged?.Invoke(this, EventArgs.Empty);
        
        return category;
    }
    
    public async Task UpdateCategoryAsync(Category category)
    {
        var existing = _categories.FirstOrDefault(c => c.Id == category.Id);
        if (existing != null)
        {
            existing.Name = category.Name;
            existing.Color = category.Color;
            await SaveCategoriesAsync();
            CategoriesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public async Task DeleteCategoryAsync(int categoryId)
    {
        var category = _categories.FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
        {
            _categories.Remove(category);
            await SaveCategoriesAsync();
            CategoriesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public Category? GetCategoryById(int categoryId)
    {
        return _categories.FirstOrDefault(c => c.Id == categoryId);
    }
    
    private async Task LoadCategoriesAsync()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = await File.ReadAllTextAsync(_filePath);
                _categories = JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.ListCategory) ?? new List<Category>();
                
                if (_categories.Any())
                {
                    _nextId = _categories.Max(c => c.Id) + 1;
                }
            }
            else
            {
                // Create default categories
                _categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Work", Color = "#2196F3" },
                    new Category { Id = 2, Name = "Personal", Color = "#4CAF50" },
                    new Category { Id = 3, Name = "Urgent", Color = "#F44336" }
                };
                _nextId = 4;
                await SaveCategoriesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading categories: {ex.Message}");
            _categories = new List<Category>();
        }
    }
    
    private async Task SaveCategoriesAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_categories, AppJsonSerializerContext.Default.ListCategory);
            await File.WriteAllTextAsync(_filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving categories: {ex.Message}");
        }
    }
}
