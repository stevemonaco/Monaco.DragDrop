using Avalonia.Controls;

namespace Monaco.DragDrop;
public interface IDropAdorner
{
    Control? TargetControl { get; set; }
    void Attach();
    void Detach();
}
