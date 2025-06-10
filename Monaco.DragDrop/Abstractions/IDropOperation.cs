using Avalonia.Controls;

namespace Monaco.DragDrop.Abstractions;
public interface IDropOperation
{
    IList<string> InteractionIds { get; set; }
    Control? AttachedControl { get; }
    object? PayloadTarget { get; set; }

    void Attach(Control control);
    void Detach(Control control);
}
