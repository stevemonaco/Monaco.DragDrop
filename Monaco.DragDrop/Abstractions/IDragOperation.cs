using Avalonia.Controls;
using Avalonia.Input;

namespace Monaco.DragDrop.Abstractions;
public interface IDragOperation
{
    IList<string> InteractionIds { get; set; }
    Control? AttachedControl { get; }
    object? Payload { get; set; }

    void Attach(Control control);
    void Detach(Control control);

    void DropCompleted(DragDropEffects effect, DragInfo dragInfo, DropInfo dropInfo);
}
