using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ProductivityCake.Controls;

public partial class CategoryComboBox : UserControl
{
    // Named controls - accessed like in CustomButton
    // private Button? InnerButton => this.FindControl<Button>("InnerButton");
    // private TextBlock? DisplayText => this.FindControl<TextBlock>("DisplayText");
    // private StackPanel? CategoriesPanel => this.FindControl<StackPanel>("CategoriesPanel");
    // private TextBox? NewCategoryTextBox => this.FindControl<TextBox>("NewCategoryTextBox");
    // private Button? AddButton => this.FindControl<Button>("AddButton");
    // private Button? CancelButton => this.FindControl<Button>("CancelButton");
    private Flyout? CategoryFlyout => InnerButton?.Flyout as Flyout;

    // Dependency Properties
    public static readonly StyledProperty<ObservableCollection<Category>?> CategoriesProperty =
        AvaloniaProperty.Register<CategoryComboBox, ObservableCollection<Category>?>(
            nameof(Categories), 
            new ObservableCollection<Category>());

    public static readonly StyledProperty<Category?> SelectedCategoryProperty =
        AvaloniaProperty.Register<CategoryComboBox, Category?>(nameof(SelectedCategory));

    public static readonly StyledProperty<string> PlaceholderTextProperty =
        AvaloniaProperty.Register<CategoryComboBox, string>(nameof(PlaceholderText), "Select Category");

    // Events
    public static readonly RoutedEvent<CategorySelectionChangedEventArgs> CategorySelectionChangedEvent =
        RoutedEvent.Register<CategoryComboBox, CategorySelectionChangedEventArgs>(
            nameof(CategorySelectionChanged), 
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent<CategoryAddedEventArgs> CategoryAddedEvent =
        RoutedEvent.Register<CategoryComboBox, CategoryAddedEventArgs>(
            nameof(CategoryAdded), 
            RoutingStrategies.Bubble);

    // CLR Properties
    public ObservableCollection<Category>? Categories
    {
        get => GetValue(CategoriesProperty);
        set => SetValue(CategoriesProperty, value);
    }

    public Category? SelectedCategory
    {
        get => GetValue(SelectedCategoryProperty);
        set => SetValue(SelectedCategoryProperty, value);
    }

    public string PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    // Events
    public event EventHandler<CategorySelectionChangedEventArgs> CategorySelectionChanged
    {
        add => AddHandler(CategorySelectionChangedEvent, value);
        remove => RemoveHandler(CategorySelectionChangedEvent, value);
    }

    public event EventHandler<CategoryAddedEventArgs> CategoryAdded
    {
        add => AddHandler(CategoryAddedEvent, value);
        remove => RemoveHandler(CategoryAddedEvent, value);
    }

    public CategoryComboBox()
    {
        InitializeComponent();
        InitializeEvents();
        UpdateControl(); // Initial update like in CustomButton
    }

    private void InitializeEvents()
    {
        // Wire up events after controls are loaded
        Loaded += (s, e) =>
        {
            if (AddButton != null)
                AddButton.Click += OnAddClicked;
            
            if (CancelButton != null)
                CancelButton.Click += OnCancelClicked;
                
            if (NewCategoryTextBox != null)
                NewCategoryTextBox.KeyDown += OnTextBoxKeyDown;
        };
    }

    // The key fix - handle ALL property changes like CustomButton
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // Update when any of our custom properties change
        if (change.Property == CategoriesProperty ||
            change.Property == SelectedCategoryProperty ||
            change.Property == PlaceholderTextProperty)
        {
            UpdateControl();
        }
    }

    private void UpdateControl()
    {
        UpdateDisplayText();
        UpdateCategoriesList();
    }

    private void UpdateDisplayText()
    {
        if (DisplayText == null) return;
        
        DisplayText.Text = SelectedCategory?.Name ?? PlaceholderText;
    }

    private void UpdateCategoriesList()
    {
        if (CategoriesPanel == null || Categories == null) return;

        // Clear existing category buttons
        CategoriesPanel.Children.Clear();

        // Add button for each category
        foreach (var category in Categories)
        {
            var categoryButton = new Button
            {
                Content = category.Name,
                Classes = { "category-item" },
                Tag = category // Store category in Tag for easy access
            };

            categoryButton.Click += OnCategorySelected;
            CategoriesPanel.Children.Add(categoryButton);
        }
    }

    // Event handlers
    private void OnCategorySelected(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Category category)
        {
            var oldCategory = SelectedCategory;
            SelectedCategory = category;
            CategoryFlyout?.Hide();

            // Raise selection changed event
            var args = new CategorySelectionChangedEventArgs(
                CategorySelectionChangedEvent, 
                oldCategory, 
                category);
            RaiseEvent(args);
        }
    }

    private void OnAddClicked(object? sender, RoutedEventArgs e)
    {
        AddNewCategory();
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e)
    {
        ClearAndCloseFlyout();
    }

    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AddNewCategory();
        }
        else if (e.Key == Key.Escape)
        {
            ClearAndCloseFlyout();
        }
    }

    // Helper methods
    private void AddNewCategory()
    {
        var categoryName = NewCategoryTextBox?.Text?.Trim();
        
        if (string.IsNullOrEmpty(categoryName))
            return;

        // Check if category already exists
        if (Categories?.Any(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)) == true)
        {
            // Category already exists, just select it
            var existingCategory = Categories.First(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            SelectedCategory = existingCategory;
        }
        else
        {
            // Create new category
            var newCategory = new Category { Name = categoryName };
            
            // Add to categories collection
            Categories?.Add(newCategory);
            
            // Set as selected
            SelectedCategory = newCategory;
            
            // Raise CategoryAdded event
            var args = new CategoryAddedEventArgs(CategoryAddedEvent, newCategory);
            RaiseEvent(args);
        }
        
        // Clear text and close flyout
        ClearAndCloseFlyout();
    }

    private void ClearAndCloseFlyout()
    {
        if (NewCategoryTextBox != null)
        {
            NewCategoryTextBox.Text = string.Empty;
        }
        CategoryFlyout?.Hide();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

// Category model class
public class Category
{
    public string Name { get; set; } = string.Empty;
    
    public override bool Equals(object? obj)
    {
        return obj is Category other && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString() => Name;
}

// Event argument classes
public class CategorySelectionChangedEventArgs : RoutedEventArgs
{
    public Category? OldCategory { get; }
    public Category? NewCategory { get; }

    public CategorySelectionChangedEventArgs(RoutedEvent routedEvent, Category? oldCategory, Category? newCategory) 
        : base(routedEvent)
    {
        OldCategory = oldCategory;
        NewCategory = newCategory;
    }
}

public class CategoryAddedEventArgs : RoutedEventArgs
{
    public Category Category { get; }

    public CategoryAddedEventArgs(RoutedEvent routedEvent, Category category) : base(routedEvent)
    {
        Category = category;
    }
}