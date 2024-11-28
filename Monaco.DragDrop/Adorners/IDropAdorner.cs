using Avalonia.Controls;
using System.Drawing;

namespace Monaco.DragDrop;
public interface IDropAdorner
{
    Control? TargetControl { get; set; }
    void Attach();
    void Detach();

    //void Update(Control? hoveredControl, Point? hoverLocation);

    bool IsDropValid { get; set; }
}
