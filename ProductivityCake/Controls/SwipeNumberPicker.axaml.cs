using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace ProductivityCake.Controls;

public partial class SwipeNumberPicker : UserControl
{
    // Enum for visual styles
    public enum VisualStyle
    {
        TwoLines,
        SelectionArea
    }

    private Point _startPoint;
    private bool _isSwiping;
    private double _currentOffset;
    private double _startOffset;
    private const double ItemHeight = 30;
    private List<int> _availableValues = new();

    // Dependency Properties
    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<SwipeNumberPicker, int>(nameof(Value), defaultValue: 3);

    public static readonly StyledProperty<int> MinimumProperty =
        AvaloniaProperty.Register<SwipeNumberPicker, int>(nameof(Minimum), defaultValue: 2);

    public static readonly StyledProperty<int> MaximumProperty =
        AvaloniaProperty.Register<SwipeNumberPicker, int>(nameof(Maximum), defaultValue: 50);

    public static readonly StyledProperty<int> StepProperty =
        AvaloniaProperty.Register<SwipeNumberPicker, int>(nameof(Step), defaultValue: 1);

    public static readonly StyledProperty<VisualStyle> PickerStyleProperty =
        AvaloniaProperty.Register<SwipeNumberPicker, VisualStyle>(nameof(PickerStyle), defaultValue: VisualStyle.SelectionArea);

    public static readonly StyledProperty<IBrush?> CustomBackgroundProperty =
        AvaloniaProperty.Register<SwipeNumberPicker, IBrush?>(nameof(CustomBackground));

    public static readonly StyledProperty<CornerRadius> CustomCornerRadiusProperty =
        AvaloniaProperty.Register<SwipeNumberPicker, CornerRadius>(nameof(CustomCornerRadius), new CornerRadius(8));

    public static readonly StyledProperty<Thickness> CustomPaddingProperty =
        AvaloniaProperty.Register<SwipeNumberPicker, Thickness>(nameof(CustomPadding), new Thickness(16, 8));

    public static readonly StyledProperty<bool> EnableSmoothAnimationProperty =
        AvaloniaProperty.Register<SwipeNumberPicker, bool>(nameof(EnableSmoothAnimation), defaultValue: true);
    
    // CLR Properties
    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public int Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public int Step
    {
        get => GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public VisualStyle PickerStyle
    {
        get => GetValue(PickerStyleProperty);
        set => SetValue(PickerStyleProperty, value);
    }

    public IBrush? CustomBackground
    {
        get => GetValue(CustomBackgroundProperty);
        set => SetValue(CustomBackgroundProperty, value);
    }

    public CornerRadius CustomCornerRadius
    {
        get => GetValue(CustomCornerRadiusProperty);
        set => SetValue(CustomCornerRadiusProperty, value);
    }

    public Thickness CustomPadding
    {
        get => GetValue(CustomPaddingProperty);
        set => SetValue(CustomPaddingProperty, value);
    }
    
    public bool EnableSmoothAnimation
    {
        get => GetValue(EnableSmoothAnimationProperty);
        set => SetValue(EnableSmoothAnimationProperty, value);
    }

    public SwipeNumberPicker()
    {
        InitializeComponent();
        InitializeEvents();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        InitializeValues();
        UpdateControl();
    }

    private void InitializeEvents()
    {
        AddHandler(PointerPressedEvent, OnPointerPressed, handledEventsToo: true);
        AddHandler(PointerReleasedEvent, OnPointerReleased, handledEventsToo: true);
        AddHandler(PointerMovedEvent, OnPointerMoved, handledEventsToo: true);
    }

    private void InitializeValues()
    {
        _availableValues.Clear();
        ValueStackPanel.Children.Clear();
    
        // Generate available values
        for (int i = Minimum; i <= Maximum; i += Step)
        {
            _availableValues.Add(i);
        }

        // Add padding at top for better swipe experience
        for (int i = 0; i < 2; i++)
        {
            ValueStackPanel.Children.Add(CreateValueItem(-1));
        }

        // Add actual values
        foreach (var value in _availableValues)
        {
            ValueStackPanel.Children.Add(CreateValueItem(value));
        }

        // Add padding at bottom for better swipe experience
        for (int i = 0; i < 2; i++)
        {
            ValueStackPanel.Children.Add(CreateValueItem(-1));
        }

        // Set initial position to center on current value
        // The center of the 90px container is at 45px, and we want the selected value at that position
        var currentIndex = _availableValues.IndexOf(Value);
        if (currentIndex >= 0)
        {
            // Calculate the offset needed to center the selected value
            var selectedItemPosition = (currentIndex + 2) * ItemHeight + (ItemHeight / 2); // +2 for top padding
            _currentOffset = 45 - selectedItemPosition; // Center is at 45px
            UpdateStackPanelPosition();
        }
    }

    private TextBlock CreateValueItem(int value)
    {
        var isCurrentValue = value == Value;
        var text = value == -1 ? "" : value.ToString();
        
        return new TextBlock
        {
            Text = text,
            FontSize = 20,
            FontWeight = isCurrentValue ? FontWeight.Bold : FontWeight.Normal,
            Foreground = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Height = ItemHeight,
            Opacity = value == -1 ? 0 : (isCurrentValue ? 1.0 : 0.7)
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            // Only update display if value actually changed
            if (IsInitialized && change.NewValue is int newValue && newValue != (change.OldValue as int?))
            {
                UpdateValueDisplay(false); // Don't update value property recursively
            }
        }
        else if (change.Property == MinimumProperty ||
                 change.Property == MaximumProperty ||
                 change.Property == StepProperty)
        {
            if (IsInitialized)
            {
                InitializeValues();
                UpdateControl();
            }
        }
        else if (change.Property == PickerStyleProperty)
        {
            UpdateVisualStyle();
        }
        else if (change.Property == CustomBackgroundProperty ||
                 change.Property == CustomCornerRadiusProperty ||
                 change.Property == CustomPaddingProperty)
        {
            UpdateControl();
        }
    }

    private void UpdateControl()
    {
        UpdateBorderStyling();
        UpdateValueDisplay();
        UpdateVisualStyle();
    }

    private void UpdateBorderStyling()
    {
        if (CustomBackground != null) 
            MainBorder.Background = CustomBackground;
        
        MainBorder.CornerRadius = CustomCornerRadius;
        MainBorder.Padding = CustomPadding;
    }

    private void UpdateValueDisplay(bool updateValueProperty = true)
    {
        // First, reset all items to normal styling
        for (int i = 0; i < ValueStackPanel.Children.Count; i++)
        {
            if (ValueStackPanel.Children[i] is TextBlock textBlock)
            {
                if (int.TryParse(textBlock.Text ?? "", out int value) && value != -1)
                {
                    textBlock.FontWeight = FontWeight.Normal;
                    textBlock.Foreground = Brushes.Gray;
                    textBlock.FontSize = 20;
                    textBlock.Opacity = 0.7;
                }
            }
        }
        
        // Now find and highlight the value that's in the center of the selection area
        var selectionAreaCenter = 45;
        double minDistance = double.MaxValue;
        TextBlock? centerTextBlock = null;
        int centerValue = Value; // Default to current value
        
        for (int i = 0; i < ValueStackPanel.Children.Count; i++)
        {
            if (ValueStackPanel.Children[i] is TextBlock textBlock && 
                int.TryParse(textBlock.Text ?? "", out int value) && value != -1)
            {
                var itemCenter = (i * ItemHeight) + (ItemHeight / 2);
                var itemPositionInContainer = itemCenter + _currentOffset;
                var distance = Math.Abs(itemPositionInContainer - selectionAreaCenter);
            
                if (distance < minDistance)
                {
                    minDistance = distance;
                    centerTextBlock = textBlock;
                    centerValue = value;
                }
            }
        }
        
        // Highlight the center value
        if (centerTextBlock != null)
        {
            centerTextBlock.FontWeight = FontWeight.Bold;
            centerTextBlock.Foreground = Brushes.Gray;
            centerTextBlock.FontSize = 20;
            centerTextBlock.Opacity = 1.0;
        
            // Update the Value property if it's different and we're allowed to
            if (updateValueProperty && centerValue != Value)
            {
                Value = centerValue;
            }
        }
    }

    private void UpdateVisualStyle()
    {
        if (TwoLinesStyle != null && SelectionAreaStyle != null)
        {
            switch (PickerStyle)
            {
                case VisualStyle.TwoLines:
                    TwoLinesStyle.IsVisible = true;
                    SelectionAreaStyle.IsVisible = false;
                    break;
                case VisualStyle.SelectionArea:
                    TwoLinesStyle.IsVisible = false;
                    SelectionAreaStyle.IsVisible = true;
                    break;
            }
        }
    }

    private void UpdateStackPanelPosition()
    {
        ValueStackPanel.Margin = new Thickness(0, _currentOffset, 0, 0);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _startPoint = e.GetPosition(this);
        _startOffset = _currentOffset;
        _isSwiping = true;
        e.Pointer.Capture(this);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isSwiping) return;

        var currentPoint = e.GetPosition(this);
        var deltaY = currentPoint.Y - _startPoint.Y;
    
        // Update offset and move the stack panel proportionally to finger movement
        _currentOffset = _startOffset + deltaY;
        
        UpdateStackPanelPosition();
        UpdateValueDisplay(false); // Don't update value property during drag
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isSwiping)
        {
            _isSwiping = false;
            e.Pointer.Capture(null);
            
            // Snap to the value closest to the center line with smooth animation
            SnapToNearestValueWithAnimation();
        }
    }

    private void SnapToNearestValueWithAnimation()
    {
        // The center of the selection area is at 45px in the 90px container
        var selectionAreaCenter = 45;
    
        // Find the index of the value item closest to the center
        double minDistance = double.MaxValue;
        int closestIndex = -1;
    
        for (int i = 0; i < ValueStackPanel.Children.Count; i++)
        {
            if (ValueStackPanel.Children[i] is TextBlock textBlock && 
                int.TryParse(textBlock.Text ?? "", out int value) && value != -1)
            {
                var itemCenter = (i * ItemHeight) + (ItemHeight / 2);
                var itemPositionInContainer = itemCenter + _currentOffset;
                var distance = Math.Abs(itemPositionInContainer - selectionAreaCenter);
            
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }
        }

        // Adjust for padding (first 2 items are padding)
        var valueIndex = closestIndex - 2;
    
        if (valueIndex >= 0 && valueIndex < _availableValues.Count)
        {
            var newValue = _availableValues[valueIndex];
            if (newValue != Value)
            {
                Value = newValue;
            }
        
            // Calculate target offset to perfectly center the selected value
            var targetItemCenter = (closestIndex * ItemHeight) + (ItemHeight / 2);
            var targetOffset = selectionAreaCenter - targetItemCenter;
        
            // Smoothly animate to the target position
            SmoothSnapToPosition(targetOffset);
        }
    }

    private async void SmoothSnapToPosition(double targetOffset)
    {
        if (!EnableSmoothAnimation)
        {
            _currentOffset = targetOffset;
            UpdateStackPanelPosition();
            UpdateValueDisplay(false);
            return;
        }
        
        const int animationDuration = 150; // ms
        const int steps = 8;
        var stepTime = animationDuration / steps;
        var startOffset = _currentOffset;
        var delta = targetOffset - startOffset;

        for (int i = 1; i <= steps; i++)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(stepTime));
            if (!_isSwiping) // Don't animate if user started swiping again
            {
                _currentOffset = startOffset + (delta * i / steps);
                UpdateStackPanelPosition();
                if (i % 2 == 0) UpdateValueDisplay(false);
            }
        }
        
        if (!_isSwiping)
        {
            _currentOffset = targetOffset;
            UpdateStackPanelPosition();
            UpdateValueDisplay(false);
        }
    }
}