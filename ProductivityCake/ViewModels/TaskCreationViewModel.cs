using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProductivityCake.Enums;
using ProductivityCake.Models;
using ProductivityCake.Services;

namespace ProductivityCake.ViewModels;

public partial class TaskCreationViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IJsonDataService _dataService;
    
    [ObservableProperty] private string _title;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DateTextFormat))]
    private DateTimeOffset? _selectedDate;
    
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(TimeTextFormat))] private TimeSpan? _selectedTime;
    
    public string DateTextFormat =>
        SelectedDate?.ToString("ddd, MMMM dd, yyyy") ?? string.Empty;

    public string TimeTextFormat => SelectedTime.HasValue ? DateTime.Today.Add(SelectedTime.Value).ToString("hh:mm tt") : string.Empty;

    // merge date and time
    public DateTimeOffset? DueDate => SelectedDate.HasValue && SelectedTime.HasValue
        ? SelectedDate.Value.Date + SelectedTime.Value
        : null;

    [ObservableProperty] private ObservableCollection<RepeatType> _repeatTypes = new()
    {
        new RepeatType() {Name = "No Repeat"},
        new RepeatType() {Name = "Once a Day"},
        new RepeatType() {Name = "Once a Day (Mon-Fri)"},
        new RepeatType() {Name = "Once a Week"},
        new RepeatType() {Name = "Once a Month"},
        new RepeatType() {Name = "Once a Year"},
        new RepeatType() {Name = "Other..."},
    };
    [ObservableProperty] private RepeatType _selectedRepeat;
    [ObservableProperty] private bool _showCustomPicker;
    [ObservableProperty] private int _customRepeatDefaultValue = 2;

    [ObservableProperty] private ObservableCollection<Category> _categories = new()
    {
        new Category() {Name = "Default"},
        new Category() {Name = "Personal"},
        new Category() {Name = "Shopping"},
        new Category() {Name = "Wishlist"},
        new Category() {Name = "Work"},
    };
    [ObservableProperty] private Category _selectedCategory;
    
    [ObservableProperty] private string _errorMessage;
    
    [ObservableProperty] private string? _newCategoryName;
    [ObservableProperty] private bool _isCategoryFlyoutOpen;
    
    public TaskCreationViewModel(INavigationService navigationService, IJsonDataService dataService)
    {
        _navigationService = navigationService;
        _dataService = dataService;
        SelectedCategory = Categories.FirstOrDefault()!;
        SelectedRepeat = RepeatTypes.FirstOrDefault()!;
    }

    public TaskCreationViewModel() : this(null!, null!) { }
    
    partial void OnSelectedRepeatChanged(RepeatType value)
    {
        // Show the custom picker only when "Other..." is selected
        ShowCustomPicker = value.Name == "Other...";
        
        // If switching away from "Other...", reset the custom interval
        if (!ShowCustomPicker)
        {
            CustomRepeatDefaultValue = 2;
        }
    }

    [RelayCommand]
    private void CancelCustomRepeat()
    {
        ShowCustomPicker = false;
        SelectedRepeat = RepeatTypes.FirstOrDefault()!;
    }
    
    [RelayCommand]
    private void OpenDatePicker(DatePicker? datePicker)
    {
        if (datePicker != null)
        {
            // Find the internal flyout button using the template part name
            var flyoutButton = datePicker.GetTemplateChildren()
                .OfType<Button>()
                .FirstOrDefault(b => b.Name == "PART_FlyoutButton");
            
            if (flyoutButton != null)
            {
                // Simulate a click on the internal button
                flyoutButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                
                Dispatcher.UIThread.Post(() =>
                {
                    var popup = datePicker.GetTemplateChildren()
                        .OfType<Popup>()
                        .FirstOrDefault(p => p.Name == "PART_Popup");
                
                    if (popup != null)
                    {
                        popup.Placement = PlacementMode.Pointer;
                        popup.HorizontalOffset = -280;
                        popup.VerticalOffset = -50;
                    }
                }, DispatcherPriority.Background);
            }
        }
    }

    [RelayCommand]
    private void ClearTitle()
    {
        Title = string.Empty;
    }

    [RelayCommand]
    private void ClearDate()
    {
        SelectedDate = null;
    }

    [RelayCommand]
    private void ClearTime()
    {
        SelectedTime =  null;
    }

    [RelayCommand]
    private void OpenTimePicker(TimePicker? timePicker)
    {
        if (timePicker != null)
        {
            // Find the internal flyout button using the template part name
            var flyoutButton = timePicker.GetTemplateChildren()
                .OfType<Button>()
                .FirstOrDefault(b => b.Name == "PART_FlyoutButton");
            
            if (flyoutButton != null)
            {
                // Simulate a click on the internal button
                flyoutButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                
                Dispatcher.UIThread.Post(() =>
                {
                    var popup = timePicker.GetTemplateChildren()
                        .OfType<Popup>()
                        .FirstOrDefault(p => p.Name == "PART_Popup");
                
                    if (popup != null)
                    {
                        popup.Placement = PlacementMode.Pointer;
                        popup.HorizontalOffset = -250;
                        popup.VerticalOffset = -50;
                    }
                }, DispatcherPriority.Background);
            }
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Add at least a title";
                return;
            }

            if (DueDate == null)
            {
                ErrorMessage = "Add due date";
                return;
            }
            
            ErrorMessage = string.Empty;
            
            var newTask = new TodoItem()
            {
                Title = Title,
                DueDate = DueDate.Value,
                RepeatType = SelectedRepeat,
                Category = SelectedCategory.Name,
            };

            Console.WriteLine("it works here!");
            
            await _dataService.CreateAsync(newTask);
            await _navigationService.NavigateToAsync<TaskListViewModel>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [RelayCommand]
    private void OpenCategoryFlyout(Button button)
    {
        if(button?.Flyout != null)
            button.Flyout.ShowAt(button);
    }

    [RelayCommand]
    private void AddCategory(Button button)
    {
        if (!string.IsNullOrWhiteSpace(NewCategoryName))
        {
            var newCategory = new Category() { Name = NewCategoryName };
            Categories.Add(newCategory);
            SelectedCategory = newCategory;
            NewCategoryName = string.Empty;
            
            if(button?.Flyout != null)
                button.Flyout.Hide();
        }
    }

    [RelayCommand]
    private void CancelCategory(Button button)
    {
        NewCategoryName = string.Empty;
        
        if(button?.Flyout != null)
            button.Flyout.Hide();
    }

    [RelayCommand]
    private void NavigateToTaskList()
    {
        _navigationService.NavigateToAsync<TaskListViewModel>();
    }

    partial void OnSelectedDateChanged(DateTimeOffset? value)
    {
        OnPropertyChanged(nameof(DueDate));
    }

    partial void OnSelectedTimeChanged(TimeSpan? value)
    {
        OnPropertyChanged(nameof(DueDate));
    }
}