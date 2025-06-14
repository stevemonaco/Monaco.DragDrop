using CommunityToolkit.Mvvm.ComponentModel;

namespace DragDropSample.ViewModels.Nodes;
public abstract partial class NodeViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; }

    public NodeViewModel(string name)
    {
        Name = name;
    }
}
