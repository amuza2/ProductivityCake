using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductivityCake.Models;
using ProductivityCake.Services;

namespace ProductivityCake.ViewModels;

public partial class ProjectDetailsViewModel : ViewModelBase
{
    private readonly IJsonDataService? _dataService;
    private readonly INavigationService? _navigationService;
    private readonly CategoryService? _categoryService;
    
    [ObservableProperty]
    private Project? _currentProject;
    
    [ObservableProperty]
    private ObservableCollection<TodoItem> _tasks = new();
    
    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();
    
    [ObservableProperty]
    private bool _isTodoExpanded = true;
    
    [ObservableProperty]
    private bool _isDoingExpanded = true;
    
    [ObservableProperty]
    private bool _isDoneExpanded = true;
    
    // Filtered task collections for Trello view
    public ObservableCollection<TodoItem> TodoTasks => 
        new(Tasks.Where(t => t.Status == Models.TaskStatus.ToDo));
    
    public ObservableCollection<TodoItem> DoingTasks => 
        new(Tasks.Where(t => t.Status == Models.TaskStatus.Doing));
    
    public ObservableCollection<TodoItem> DoneTasks => 
        new(Tasks.Where(t => t.Status == Models.TaskStatus.Done).OrderByDescending(t => t.CompletedAt));
    
    [ObservableProperty]
    private bool _isAddTaskDialogOpen;
    
    [ObservableProperty]
    private bool _isEditTaskDialogOpen;
    
    [ObservableProperty]
    private bool _isAddCategoryDialogOpen;
    
    [ObservableProperty]
    private bool _isTaskDetailsDialogOpen;
    
    [ObservableProperty]
    private TodoItem? _selectedTaskForDetails;
    
    [ObservableProperty]
    private string _taskTitle = string.Empty;
    
    [ObservableProperty]
    private string? _taskDescription;
    
    [ObservableProperty]
    private DateTimeOffset? _taskDueDate;
    
    [ObservableProperty]
    private Models.TaskStatus _taskStatus = Models.TaskStatus.ToDo;
    
    [ObservableProperty]
    private Category? _selectedCategory;
    
    [ObservableProperty]
    private ObservableCollection<Category> _selectedTags = new();
    
    [ObservableProperty]
    private TodoItem? _selectedTask;
    
    [ObservableProperty]
    private string _newCategoryName = string.Empty;
    
    [ObservableProperty]
    private string _newCategoryColor = "#2196F3";
    
    public ProjectDetailsViewModel(IJsonDataService dataService, INavigationService navigationService, CategoryService categoryService)
    {
        _dataService = dataService;
        _navigationService = navigationService;
        _categoryService = categoryService;
        _ = LoadCategoriesAsync();
    }
    
    public ProjectDetailsViewModel() : this(null!, null!, null!)
    {
    }
    
    public void Initialize(Project project)
    {
        CurrentProject = project;
        _ = LoadTasksAsync();
    }
    
    [RelayCommand]
    private async Task LoadTasksAsync()
    {
        if (_dataService == null || CurrentProject == null) return;
        
        try
        {
            var allTasks = await _dataService.GetAllAsync();
            var projectTasks = allTasks.Where(t => t.ProjectId == CurrentProject.Id).ToList();
            
            // Load categories and tags for each task
            foreach (var task in projectTasks)
            {
                if (task.CategoryId.HasValue && _categoryService != null)
                {
                    task.Category = _categoryService.GetCategoryById(task.CategoryId.Value);
                }
                
                // Load tags
                if (_categoryService != null && task.TagIds != null && task.TagIds.Count > 0)
                {
                    task.Tags = task.TagIds
                        .Select(id => _categoryService.GetCategoryById(id))
                        .Where(c => c != null)
                        .Cast<Category>()
                        .ToList();
                }
            }
            
            Tasks = new ObservableCollection<TodoItem>(projectTasks);
            
            // Notify changes for filtered collections (for Trello view)
            OnPropertyChanged(nameof(TodoTasks));
            OnPropertyChanged(nameof(DoingTasks));
            OnPropertyChanged(nameof(DoneTasks));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tasks: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        if (_categoryService == null) return;
        
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            Categories = new ObservableCollection<Category>(categories);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading categories: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private void OpenAddTaskDialog()
    {
        TaskTitle = string.Empty;
        TaskDescription = null;
        TaskDueDate = DateTimeOffset.Now.AddDays(1);
        TaskStatus = Models.TaskStatus.ToDo;
        SelectedCategory = null;
        SelectedTags.Clear();
        IsAddTaskDialogOpen = true;
    }
    
    [RelayCommand]
    private void CancelAddTask()
    {
        IsAddTaskDialogOpen = false;
        TaskTitle = string.Empty;
        TaskDescription = null;
        TaskDueDate = null;
        TaskStatus = Models.TaskStatus.ToDo;
        SelectedCategory = null;
        SelectedTags.Clear();
    }
    
    [RelayCommand(CanExecute = nameof(CanSaveTask))]
    private async Task AddTaskAsync()
    {
        if (_dataService == null || CurrentProject == null) return;
        
        try
        {
            var newTask = new TodoItem
            {
                Title = TaskTitle.Trim(),
                Description = TaskDescription?.Trim(),
                DueDate = TaskDueDate,
                ProjectId = CurrentProject.Id,
                Status = TaskStatus,
                CategoryId = SelectedCategory?.Id,
                TagIds = SelectedTags.Select(t => t.Id).ToList(),
                IsComplete = TaskStatus == Models.TaskStatus.Done,
                CreatedAt = DateTimeOffset.UtcNow,
                CompletedAt = TaskStatus == Models.TaskStatus.Done ? DateTimeOffset.UtcNow : null
            };
            
            await _dataService.CreateAsync(newTask);
            
            // Close dialog and reload tasks
            IsAddTaskDialogOpen = false;
            TaskTitle = string.Empty;
            TaskDescription = null;
            TaskDueDate = null;
            TaskStatus = Models.TaskStatus.ToDo;
            SelectedCategory = null;
            SelectedTags.Clear();
            
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding task: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private void OpenEditTaskDialog(TodoItem task)
    {
        // Close task details dialog if open
        IsTaskDetailsDialogOpen = false;
        SelectedTaskForDetails = null;
        
        SelectedTask = task;
        TaskTitle = task.Title;
        TaskDescription = task.Description;
        TaskDueDate = task.DueDate;
        TaskStatus = task.Status;
        SelectedCategory = task.Category;
        SelectedTags = new ObservableCollection<Category>(task.Tags ?? new List<Category>());
        IsEditTaskDialogOpen = true;
    }
    
    [RelayCommand]
    private void CancelEditTask()
    {
        IsEditTaskDialogOpen = false;
        SelectedTask = null;
        TaskTitle = string.Empty;
        TaskDescription = null;
        TaskStatus = Models.TaskStatus.ToDo;
        SelectedCategory = null;
        SelectedTags.Clear();
        TaskDueDate = null;
    }
    
    [RelayCommand(CanExecute = nameof(CanSaveTask))]
    private async Task UpdateTaskAsync()
    {
        if (_dataService == null || SelectedTask == null) return;
        
        try
        {
            SelectedTask.Title = TaskTitle.Trim();
            SelectedTask.Description = TaskDescription?.Trim();
            SelectedTask.DueDate = TaskDueDate;
            SelectedTask.Status = TaskStatus;
            SelectedTask.CategoryId = SelectedCategory?.Id;
            SelectedTask.TagIds = SelectedTags.Select(t => t.Id).ToList();
            SelectedTask.IsComplete = TaskStatus == Models.TaskStatus.Done;
            
            if (TaskStatus == Models.TaskStatus.Done && !SelectedTask.CompletedAt.HasValue)
            {
                SelectedTask.CompletedAt = DateTimeOffset.UtcNow;
            }
            else if (TaskStatus != Models.TaskStatus.Done)
            {
                SelectedTask.CompletedAt = null;
            }
            
            await _dataService.UpdateAsync(SelectedTask);
            
            // Close dialog and reload tasks
            IsEditTaskDialogOpen = false;
            SelectedTask = null;
            TaskTitle = string.Empty;
            TaskDescription = null;
            SelectedTags.Clear();
            TaskDueDate = null;
            TaskStatus = Models.TaskStatus.ToDo;
            SelectedCategory = null;
            
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating task: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private async Task DeleteTaskAsync(TodoItem task)
    {
        if (_dataService == null) return;
        
        try
        {
            await _dataService.DeleteAsync(task.Id);
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting task: {ex.Message}");
        }
    }
    
    [RelayCommand]
    private void OpenAddCategoryDialog()
    {
        NewCategoryName = string.Empty;
        NewCategoryColor = "#2196F3";
        IsAddCategoryDialogOpen = true;
    }
    
    [RelayCommand]
    private void CancelAddCategory()
    {
        IsAddCategoryDialogOpen = false;
        NewCategoryName = string.Empty;
        NewCategoryColor = "#2196F3";
    }
    
    [RelayCommand(CanExecute = nameof(CanSaveCategory))]
    private async Task AddCategoryAsync()
    {
        if (_categoryService == null) return;
        
        try
        {
            await _categoryService.CreateCategoryAsync(NewCategoryName.Trim(), NewCategoryColor);
            await LoadCategoriesAsync();
            
            IsAddCategoryDialogOpen = false;
            NewCategoryName = string.Empty;
            NewCategoryColor = "#2196F3";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding category: {ex.Message}");
        }
    }
    
    private bool CanSaveTask() => !string.IsNullOrWhiteSpace(TaskTitle);
    
    private bool CanSaveCategory() => !string.IsNullOrWhiteSpace(NewCategoryName);
    
    partial void OnNewCategoryNameChanged(string value)
    {
        AddCategoryCommand.NotifyCanExecuteChanged();
    }
    
    partial void OnTaskTitleChanged(string value)
    {
        AddTaskCommand.NotifyCanExecuteChanged();
        UpdateTaskCommand.NotifyCanExecuteChanged();
    }
    
    [RelayCommand]
    private void ToggleTodoExpanded()
    {
        IsTodoExpanded = !IsTodoExpanded;
    }
    
    [RelayCommand]
    private void ToggleDoingExpanded()
    {
        IsDoingExpanded = !IsDoingExpanded;
    }
    
    [RelayCommand]
    private void ToggleDoneExpanded()
    {
        IsDoneExpanded = !IsDoneExpanded;
    }
    
    [RelayCommand]
    private void OpenTaskDetails(TodoItem task)
    {
        SelectedTaskForDetails = task;
        IsTaskDetailsDialogOpen = true;
    }
    
    [RelayCommand]
    private void CloseTaskDetails()
    {
        IsTaskDetailsDialogOpen = false;
        SelectedTaskForDetails = null;
    }
    
    [RelayCommand]
    private void AddTag(Category tag)
    {
        if (!SelectedTags.Contains(tag))
        {
            SelectedTags.Add(tag);
        }
    }
    
    [RelayCommand]
    private void RemoveTag(Category tag)
    {
        SelectedTags.Remove(tag);
    }
    
    [RelayCommand]
    private void ToggleTag(Category tag)
    {
        if (SelectedTags.Contains(tag))
        {
            SelectedTags.Remove(tag);
        }
        else
        {
            SelectedTags.Add(tag);
        }
        
        // Notify to refresh the UI
        OnPropertyChanged(nameof(SelectedTags));
    }
    
    [RelayCommand]
    private async Task DeleteTagAsync(Category tag)
    {
        if (_categoryService == null) return;
        
        try
        {
            // Remove from selected tags if present
            SelectedTags.Remove(tag);
            
            // Delete the tag from the service
            await _categoryService.DeleteCategoryAsync(tag.Id);
            
            // Reload categories
            await LoadCategoriesAsync();
            
            // Reload tasks to update their tags
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting tag: {ex.Message}");
        }
    }
    
    public bool IsTagSelected(Category tag)
    {
        return SelectedTags.Any(t => t.Id == tag.Id);
    }
    
    [RelayCommand]
    private void NavigateBack()
    {
        _navigationService?.NavigateToAsync<ProjectListViewModel>();
    }
}
