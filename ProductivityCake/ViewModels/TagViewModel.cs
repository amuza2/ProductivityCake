using CommunityToolkit.Mvvm.ComponentModel;
using ProductivityCake.Models;

namespace ProductivityCake.ViewModels;

public partial class TagViewModel : ObservableObject
{
    [ObservableProperty]
    private Category _category;
    
    [ObservableProperty]
    private bool _isSelected;
    
    public TagViewModel(Category category, bool isSelected = false)
    {
        _category = category;
        _isSelected = isSelected;
    }
}
