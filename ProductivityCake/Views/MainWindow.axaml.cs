using Avalonia.Controls;
using Avalonia.Input;

namespace ProductivityCake.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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