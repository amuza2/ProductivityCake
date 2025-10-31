using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ProductivityCake.Models;

namespace ProductivityCake.Services;

public class ProjectService
{
    private readonly string _storagePath;
    private readonly IJsonDataService _dataService;
    private List<Project> _projects = new();
    private int _nextId = 1;
    
    public event EventHandler? ProjectsChanged;
    
    public ProjectService(IJsonDataService dataService)
    {
        _dataService = dataService;
        _storagePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProductivityCake",
            "projects.json");
        
        // Load projects synchronously in constructor to ensure they're available immediately
        LoadProjectsSync();
    }
    
    private void LoadProjectsSync()
    {
        try
        {
            if (File.Exists(_storagePath))
            {
                var json = File.ReadAllText(_storagePath);
                _projects = JsonSerializer.Deserialize(json, AppJsonSerializerContext.Default.ListProject) ?? new List<Project>();
                _nextId = _projects.Count > 0 ? _projects.Max(p => p.Id) + 1 : 1;
            }
        }
        catch (Exception ex)
        {
            // Log error or handle it appropriately
            Console.WriteLine($"Error loading projects: {ex.Message}");
            _projects = new List<Project>();
        }
    }
    
    private async Task SaveProjects()
    {
        try
        {
            var directory = Path.GetDirectoryName(_storagePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }
            
            var json = JsonSerializer.Serialize(_projects, AppJsonSerializerContext.Default.ListProject);
            await File.WriteAllTextAsync(_storagePath, json);
            
            ProjectsChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            // Log error or handle it appropriately
            Console.WriteLine($"Error saving projects: {ex.Message}");
        }
    }
    
    public async Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        // Load all tasks
        var allTasks = await _dataService.GetAllAsync();
        
        // Populate tasks for each project
        foreach (var project in _projects)
        {
            project.Tasks = allTasks.Where(t => t.ProjectId == project.Id).ToList();
        }
        
        return _projects;
    }
    
    public async Task<Project> GetProjectByIdAsync(int id)
    {
        await Task.CompletedTask;
        return _projects.FirstOrDefault(p => p.Id == id) ?? 
               throw new KeyNotFoundException($"Project with ID {id} not found");
    }
    
    public async Task<Project> CreateProjectAsync(Project project)
    {
        project.Id = _nextId++;
        project.CreatedAt = DateTimeOffset.UtcNow;
        _projects.Add(project);
        
        await SaveProjects();
        return project;
    }
    
    public async Task UpdateProjectAsync(Project project)
    {
        var existingProject = await GetProjectByIdAsync(project.Id);
        var index = _projects.IndexOf(existingProject);
        _projects[index] = project;
        
        await SaveProjects();
    }
    
    public async Task DeleteProjectAsync(int id)
    {
        var project = await GetProjectByIdAsync(id);
        _projects.Remove(project);
        
        await SaveProjects();
    }
    
    public async Task ArchiveProjectAsync(int id)
    {
        var project = await GetProjectByIdAsync(id);
        project.IsArchived = true;
        
        await SaveProjects();
    }
    
    public async Task UnarchiveProjectAsync(int id)
    {
        var project = await GetProjectByIdAsync(id);
        project.IsArchived = false;
        
        await SaveProjects();
    }
}
