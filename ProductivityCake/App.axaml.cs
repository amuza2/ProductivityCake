using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ProductivityCake.Extensions;
using ProductivityCake.ViewModels;
using ProductivityCake.Views;

namespace ProductivityCake;

public partial class App : Application
{
    private IServiceProvider? _services;
    private MainWindow? _mainWindow;
    private NativeMenuItem? _toggleMenuItem;
    private const string TrayIconPath = "icons8-cake-96.png";
    private const string ExitIconPath = "icons8-exit-24.png";
    private const string GitHubIconPath = "icons8-github-24.png";
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            BindingPlugins.DataValidators.RemoveAt(0);
            
            var collection = new ServiceCollection();
            collection.AddCommonServices();
            
            _services = collection.BuildServiceProvider();
            var dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            Directory.CreateDirectory(dataDirectory);
            
            var vm = _services.GetRequiredService<MainWindowViewModel>();
            
            DisableAvaloniaDataAnnotationValidation();
            _mainWindow = new MainWindow
            {
                DataContext = vm,
            };
            
            desktop.MainWindow = _mainWindow;
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            _mainWindow.Closing += OnMainWindowClosing;
            
            CreateTrayIcon();
            
            if (_toggleMenuItem != null)
                _toggleMenuItem.Header = "Hide";
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
    
    // Tray Icon Implementation
    
    private void OnMainWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = true;
        _mainWindow?.Hide();
        UpdateToggleMenuText();
    }
    
    private void OnToggleClicked(object? sender, EventArgs e)
    {
        ToggleWindow();
        UpdateToggleMenuText();
    }
    
    private void UpdateToggleMenuText()
    {
        if (_toggleMenuItem != null)
        {
            _toggleMenuItem.Header = GetToggleMenuText();
        }
    }
    
    private void OnExitClicked(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
    
    private void ToggleWindow()
    {
        if (_mainWindow == null) return;
        
        if (_mainWindow.IsVisible)
        {
            _mainWindow.Hide();
        }
        else
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Activate();
        }
    }
    
    private void OnTrayIconClicked(object? sender, EventArgs e)
    {
        ToggleWindow();
        UpdateToggleMenuText();
    }
    
    private void OnGitHubClicked(object? sender, EventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/amuza2/ProductivityCake",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening GitHub repository: {ex.Message}");
        }
    }
    
    private void CreateTrayIcon()
    {
        try
        {
            var statusMenuItem = new NativeMenuItem
            {
                Header = "ProductivityCake",
                IsEnabled = false
            };
            
            _toggleMenuItem = new NativeMenuItem
            {
                Header = GetToggleMenuText()
            };
            _toggleMenuItem.Click += OnToggleClicked;
            
            var githubMenuItem = new NativeMenuItem
            {
                Header = "Go to Repository",
                Icon = LoadBitmap($"/Assets/{GitHubIconPath}")
            };
            githubMenuItem.Click += OnGitHubClicked;
            
            var exitMenuItem = new NativeMenuItem
            {
                Header = "Exit",
                Icon = LoadBitmap($"/Assets/{ExitIconPath}")
            };
            exitMenuItem.Click += OnExitClicked;
            
            // Create the native menu
            var menu = new NativeMenu();
            menu.Add(statusMenuItem);
            menu.Add(_toggleMenuItem);
            menu.Add(githubMenuItem);
            menu.Add(new NativeMenuItemSeparator());
            menu.Add(exitMenuItem);
            
            // Create main tray icon
            var trayIcon = new TrayIcon
            {
                Icon = LoadWindowIcon($"/Assets/{TrayIconPath}"),
                ToolTipText = "ProductivityCake - Your Productivity Companion",
                Menu = menu
            };
            
            trayIcon.Clicked += OnTrayIconClicked;
            
            // Set the tray icon on the application
            var trayIcons = new TrayIcons { trayIcon };
            TrayIcon.SetIcons(this, trayIcons);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating tray icon: {ex.Message}");
        }
    }
    
    private string GetToggleMenuText()
    {
        return (_mainWindow?.IsVisible == true) ? "Hide" : "Show";
    }
    
    private WindowIcon? LoadWindowIcon(string path)
    {
        try
        {
            var uri = new Uri($"avares://ProductivityCake{path}");
            using var stream = Avalonia.Platform.AssetLoader.Open(uri);
            return new WindowIcon(stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading window icon: {path} - {ex.Message}");
            return null;
        }
    }
    
    private Avalonia.Media.Imaging.Bitmap? LoadBitmap(string path)
    {
        try
        {
            var uri = new Uri($"avares://ProductivityCake{path}");
            using var stream = Avalonia.Platform.AssetLoader.Open(uri);
            return new Avalonia.Media.Imaging.Bitmap(stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading bitmap: {path} - {ex.Message}");
            return null;
        }
    }
}