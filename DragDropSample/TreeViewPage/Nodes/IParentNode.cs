using System.Collections.ObjectModel;

namespace DragDropSample.ViewModels.Nodes;
public interface IParentNode
{
    ObservableCollection<NodeViewModel> Children { get; init; }
    bool CanAddNode(NodeViewModel node);
}
