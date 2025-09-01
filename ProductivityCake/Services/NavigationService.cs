using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ProductivityCake.ViewModels;

namespace ProductivityCake.Services;

public class NavigationService : INavigationService
{
    private MainWindowViewModel? _mainWindowViewModel;
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task NavigateToAsync<T>() where T : ViewModelBase
    {
        if(_mainWindowViewModel == null)
            return Task.CompletedTask;
        
        var vm = _serviceProvider.GetRequiredService<T>();
        _mainWindowViewModel.CurrentViewModel = vm;
        return Task.CompletedTask;
    }

    public void SetMainWindowViewModel(MainWindowViewModel mainWindowViewModel)
    {
        _mainWindowViewModel = mainWindowViewModel;
    }
}