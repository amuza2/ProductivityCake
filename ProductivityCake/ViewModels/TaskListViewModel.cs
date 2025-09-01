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
    [ObservableProperty] private ObservableCollection<ListSelection> _listSelections;
    [ObservableProperty] private ListSelection _selectedList;

    [ObservableProperty] private bool _inSearchMode;
    [ObservableProperty] private string _searchText;
    
    public TaskListViewModel(IJsonDataService jsonService, INavigationService navigationService)
    {
        _jsonService = jsonService;
        _navigationService = navigationService;
        TodoItems = new  ObservableCollection<TodoItem>()
        {
            new TodoItem{Id = 1,Title = "Task1", Description = "Task 1 Description", DueDate = DateTimeOffset.Now.AddDays(1),  Priority = TodoPriority.Medium},
            new TodoItem{Id = 2,Title = "Task2", Description = "Task 2 Description", DueDate = DateTimeOffset.Now,  Priority = TodoPriority.Medium}
        };
        ListSelections = new ObservableCollection<ListSelection>() { ListSelection.All , ListSelection.Completed, ListSelection.NotCompleted};
        SelectedList = ListSelection.All;
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