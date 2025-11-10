using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using ProductivityCake.ViewModels;

namespace ProductivityCake.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Subscribe to DataContext changes to monitor AlwaysOnTop property
        DataContextChanged += OnDataContextChanged;
    }
    
    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is MainWindowViewModel mainViewModel)
        {
            mainViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
        }
    }
    
    private void OnMainViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.CurrentViewModel) && 
            sender is MainWindowViewModel mainViewModel &&
            mainViewModel.CurrentViewModel is TimerViewModel timerViewModel)
        {
            // Subscribe to TimerViewModel property changes
            timerViewModel.PropertyChanged += OnTimerViewModelPropertyChanged;
            
            // Set initial state
            Topmost = timerViewModel.AlwaysOnTop;
        }
    }
    
    private void OnTimerViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimerViewModel.AlwaysOnTop) && sender is TimerViewModel timerViewModel)
        {
            Topmost = timerViewModel.AlwaysOnTop;
        }
    }
    
    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            BeginMoveDrag(e);
        }
    }
    
    private void OnMinimizeClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    
    private void OnCloseClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}