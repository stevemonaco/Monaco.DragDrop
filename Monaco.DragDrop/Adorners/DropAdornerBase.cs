using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.VisualTree;
using AvaDragDrop = Avalonia.Input.DragDrop;

namespace Monaco.DragDrop;
public abstract class DropAdornerBase : Border, IDropAdorner
{
    public static readonly StyledProperty<Control?> TargetControlProperty =
        AvaloniaProperty.Register<DropAdornerBase, Control?>(nameof(TargetControl), defaultBindingMode: BindingMode.OneWayToSource);

    /// <summary>
    /// Control which the adorner should be positioned above
    /// </summary>
    public Control? TargetControl
    {
        get => GetValue(TargetControlProperty);
        set => SetValue(TargetControlProperty, value);
    }

    private bool _isDropValid;
    public static readonly DirectProperty<DropAdornerBase, bool> IsDropValidProperty =
        AvaloniaProperty.RegisterDirect<DropAdornerBase, bool>(nameof(IsDropValid), o => o.IsDropValid);

    /// <summary>
    /// Control which the adorner should be positioned above
    /// </summary>
    public bool IsDropValid
    {
        get => _isDropValid;
        set { SetAndRaise(IsDropValidProperty, ref _isDropValid, value); }
    }

    private string? _errorMessage;
    public static readonly DirectProperty<DropAdornerBase, string?> ErrorMessageProperty =
        AvaloniaProperty.RegisterDirect<DropAdornerBase, string?>(nameof(ErrorMessage), o => o.ErrorMessage);

    /// <summary>
    /// Error message indicating that a validation has failed
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set { SetAndRaise(ErrorMessageProperty, ref _errorMessage, value); }
    }

    static DropAdornerBase()
    {
        IsHitTestVisibleProperty.OverrideDefaultValue<DropAdornerBase>(false);
    }

    public DropAdornerBase()
    {
        AvaDragDrop.SetAllowDrop(this, false);
        UpdatePseudoclasses(IsDropValid);
    }

    public virtual void Attach()
    {
        if (TargetControl is null)
            return;

        var layer = AdornerLayer.GetAdornerLayer(TargetControl);
        AdornerLayer.SetAdorner(TargetControl, this);
    }

    public virtual void Detach()
    {
        if (TargetControl is null)
            return;

        var layer = AdornerLayer.GetAdornerLayer(TargetControl);
        AdornerLayer.SetAdorner(TargetControl, null);
    }

    /// <summary>
    /// Gets the rect overlaying the TargetControl in AdornerLayer coordinates
    /// </summary>
    /// <returns></returns>
    public Rect GetAdornerRect()
    {
        var root = (Window)TargetControl!.GetVisualRoot()!;
        var bounds = TargetControl!.Bounds;
        var point = TargetControl.TranslatePoint(bounds.TopLeft, root)!.Value;

        return new Rect(point.X - bounds.X, point.Y - bounds.Y, bounds.Width, bounds.Height);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsDropValidProperty && change.NewValue is bool isDropValid)
        {
            UpdatePseudoclasses(isDropValid);
        }
    }

    private void UpdatePseudoclasses(bool isDropValid)
    {
        PseudoClasses.Set(":error", !isDropValid);
    }
}
