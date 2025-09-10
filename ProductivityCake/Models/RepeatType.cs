using CommunityToolkit.Mvvm.ComponentModel;
using ProductivityCake.ViewModels;

namespace ProductivityCake.Models;

public partial class RepeatType : ViewModelBase
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private int _interval;
    public override string ToString() => Name;
}