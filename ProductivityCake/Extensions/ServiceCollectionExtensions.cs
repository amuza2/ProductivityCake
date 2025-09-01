using Microsoft.Extensions.DependencyInjection;
using ProductivityCake.Services;
using ProductivityCake.ViewModels;

namespace ProductivityCake.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        // Register json file
        collection.AddSingleton<IJsonDataService>(p =>
            new JsonDataService("todo.json"));
        
        // Register Service
        collection.AddSingleton<INavigationService, NavigationService>();
        
        // Register ViewModels
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<TaskListViewModel>();
        collection.AddTransient<TimerViewModel>();
        collection.AddTransient<TaskCreationViewModel>();
    }
}