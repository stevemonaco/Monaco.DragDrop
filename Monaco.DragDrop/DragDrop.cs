using System.Windows.Input;
using Avalonia.Data;
using Avalonia;
using Avalonia.Controls;
using Monaco.DragDrop.Abstractions;

namespace Monaco.DragDrop;
public partial class DragDrop
{
    static DragDrop()
    {
        DragOperationProperty.Changed.AddClassHandler<Control>(HandleDragOperationChanged);
        DropOperationProperty.Changed.AddClassHandler<Control>(HandleDropOperationChanged);
    }

    /// <summary>
    /// Identifies the <seealso cref="CommandProperty"/> avalonia attached property.
    /// </summary>
    /// <value>Provide an <see cref="ICommand"/> derived object or binding.</value>
    public static readonly AttachedProperty<IDragOperation?> DragOperationProperty = AvaloniaProperty.RegisterAttached<DragDrop, Control, IDragOperation?>(
        "DragOperation", null, false, BindingMode.OneTime);

    /// <summary>
    /// Accessor for Attached property <see cref="CommandProperty"/>.
    /// </summary>
    public static void SetDragOperation(AvaloniaObject element, IDragOperation value)
    {
        element.SetValue(DragOperationProperty, value);
    }

    /// <summary>
    /// Accessor for Attached property <see cref="CommandProperty"/>.
    /// </summary>
    public static IDragOperation? GetDragOperation(AvaloniaObject element)
    {
        return element.GetValue(DragOperationProperty);
    }

    private static void HandleDragOperationChanged(Control control, AvaloniaPropertyChangedEventArgs change)
    {
        if (change.OldValue is IDragOperation oldDrag)
        {
            oldDrag.Detach(control);
        }

        if (change.NewValue is IDragOperation newDrag)
        {
            newDrag.Attach(control);
        }
    }

    /// <summary>
    /// Identifies the <seealso cref="CommandProperty"/> avalonia attached property.
    /// </summary>
    /// <value>Provide an <see cref="ICommand"/> derived object or binding.</value>
    public static readonly AttachedProperty<IDropOperation?> DropOperationProperty = AvaloniaProperty.RegisterAttached<DragDrop, Control, IDropOperation?>(
        "DropOperation", null, false, BindingMode.OneTime);

    /// <summary>
    /// Accessor for Attached property <see cref="CommandProperty"/>.
    /// </summary>
    public static void SetDropOperation(AvaloniaObject element, IDropOperation value)
    {
        element.SetValue(DropOperationProperty, value);
    }

    /// <summary>
    /// Accessor for Attached property <see cref="CommandProperty"/>.
    /// </summary>
    public static IDropOperation? GetDropOperation(AvaloniaObject element)
    {
        return element.GetValue(DropOperationProperty);
    }

    private static void HandleDropOperationChanged(Control control, AvaloniaPropertyChangedEventArgs change)
    {
        if (change.OldValue is IDropOperation oldDrop)
        {
            oldDrop.Detach(control);
        }

        if (change.NewValue is IDropOperation newDrop)
        {
            newDrop.Attach(control);
        }
    }
}
