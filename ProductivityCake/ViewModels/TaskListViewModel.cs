using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductivityCake.Enums;
using ProductivityCake.Models;
using ProductivityCake.Services;

namespace ProductivityCake.ViewModels;

public partial class TaskListViewModel : ViewModelBase
{
    private readonly IJsonDataService _jsonService;
    private readonly INavigationService _navigationService;
    
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private DateTimeOffset? _completedAt;
    [ObservableProperty] private TodoPriority _priority = TodoPriority.Low;
    
    [ObservableProperty] private ObservableCollection<TodoItem> _todoItems;
    [ObservableProperty] private TodoItem? _selectedItem;
    public Array ListFilter => Enum.GetValues(typeof(FilterSelection));
    [ObservableProperty] private FilterSelection _selectedFilter;

    [ObservableProperty] private bool _inSearchMode;
    [ObservableProperty] private string _searchText;
    
    public TaskListViewModel(IJsonDataService jsonService, INavigationService navigationService)
    {
        _jsonService = jsonService;
        _navigationService = navigationService;
        TodoItems = new ObservableCollection<TodoItem>();
        _ = LoadTasksAsync();
    }

    private async Task LoadTasksAsync()
    {
        var tasks = await _jsonService.GetAllAsync();
        foreach (var task in tasks)
        {
            TodoItems.Add(task);
        }
    }

    public TaskListViewModel():this(null!,null!)
    { }

    [RelayCommand]
    private void ToggleSearchMode()
    {
        InSearchMode = !InSearchMode;
    }
    
    [RelayCommand]
    private async Task NavigateToTaskCreationView()
    {
        await _navigationService.NavigateToAsync<TaskCreationViewModel>();
    }
}