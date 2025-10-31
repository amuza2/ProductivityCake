using Microsoft.Extensions.DependencyInjection;
using ProductivityCake.Models;
using ProductivityCake.Services;
using ProductivityCake.ViewModels;

namespace ProductivityCake.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        // Register json file for tasks
        collection.AddSingleton<IJsonDataService>(p =>
            new JsonDataService("todo.json"));
        
        // Register Services
        collection.AddSingleton<INavigationService, NavigationService>();
        collection.AddSingleton<ProjectService>();
        collection.AddSingleton<CategoryService>();
        
        // Register ViewModels
        collection.AddTransient<MainWindowViewModel>();
        collection.AddSingleton<TimerViewModel>(); // Singleton to persist timer state across navigation
        collection.AddTransient<ProjectListViewModel>();
        collection.AddTransient<ProjectDetailsViewModel>();
    }
}