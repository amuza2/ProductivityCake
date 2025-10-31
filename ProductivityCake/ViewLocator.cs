using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ProductivityCake.ViewModels;
using ProductivityCake.Views;

namespace ProductivityCake;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;
        
        if (param is string || param.GetType().IsPrimitive)
            return new TextBlock { Text = param.ToString() };

        // Direct mapping for Native AOT compatibility
        return param switch
        {
            ProjectListViewModel => new ProjectListView(),
            ProjectDetailsViewModel => new ProjectDetailsView(),
            TimerViewModel => new TimerView(),
            _ => new TextBlock { Text = "Not Found: " + param.GetType().Name }
        };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}