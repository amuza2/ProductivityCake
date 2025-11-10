using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using ProductivityCake.ViewModels;

namespace ProductivityCake.Views;

public partial class TimerView : UserControl
{
    public TimerView()
    {
        InitializeComponent();
        
        // Enable keyboard input
        Focusable = true;
        
        // Attach keyboard event handler
        KeyDown += OnKeyDown;
        
        // Auto-focus when loaded
        Loaded += (s, e) => Focus();
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not TimerViewModel viewModel)
            return;
        
        var handled = true;
        
        switch (e.Key)
        {
            case Key.Space:
                viewModel.ToggleTimerCommand.Execute(null);
                break;
            
            case Key.R:
                viewModel.ResetTimerCommand.Execute(null);
                break;
            
            case Key.S:
                viewModel.SkipSessionCommand.Execute(null);
                break;
            
            case Key.OemPlus:
            case Key.Add:
                viewModel.AddTimeCommand.Execute(null);
                break;
            
            case Key.OemMinus:
            case Key.Subtract:
                viewModel.SubtractTimeCommand.Execute(null);
                break;
            
            case Key.D1:
            case Key.NumPad1:
                viewModel.SwitchToWorkCommand.Execute(null);
                break;
            
            default:
                handled = false;
                break;
        }
        
        e.Handled = handled;
    }
}