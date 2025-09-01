using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductivityCake.Services;

namespace ProductivityCake.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    [ObservableProperty] ViewModelBase? _currentViewModel;  
    
    
    public MainWindowViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        ((NavigationService)navigationService).SetMainWindowViewModel(this);
        _navigationService.NavigateToAsync<TaskListViewModel>();
    }

    public MainWindowViewModel() : this(null!)
    { }
    
    [RelayCommand]
    private void NavigateToTasksPage()
    {
        _navigationService.NavigateToAsync<TaskListViewModel>();
    }
    
    [RelayCommand]
    private void NavigateToTimerPage()
    {
        _navigationService.NavigateToAsync<TimerViewModel>();
    }
}