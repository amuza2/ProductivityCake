using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductivityCake.Services;

namespace ProductivityCake.ViewModels;

public partial class TaskCreationViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateTextFormat))]
    private DateTime? _selectedDate;
    
    [ObservableProperty] private bool _isOpenDropDown;

    public TaskCreationViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public TaskCreationViewModel() : this(null!) { }

    public string DateTextFormat =>
        SelectedDate?.ToString("ddd, MMMM dd, yyyy") ?? string.Empty;
    
    [RelayCommand]
    private void OpenCalendar()
    {
        IsOpenDropDown = false;
        Dispatcher.UIThread.Post(() =>
        {
            IsOpenDropDown = true;
        });
    }

    [RelayCommand]
    private void NavigateToTaskList()
    {
        _navigationService.NavigateToAsync<TaskListViewModel>();
    }

}