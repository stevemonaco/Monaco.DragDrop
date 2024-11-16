using Avalonia.Controls;

namespace Monaco.DragDrop.Abstractions;
public interface IDragOperation
{
    IList<string> InteractionIds { get; set; }
    Control? AttachedControl { get; }
    object? DragPayload { get; set; }

    void Attach(Control control);
    void Detach(Control control);
}
