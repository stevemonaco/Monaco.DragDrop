using CommunityToolkit.Mvvm.ComponentModel;

namespace DragDropSample.ViewModels.Nodes;
public abstract partial class NodeViewModel : ObservableObject
{
    [ObservableProperty] private string _name;

    public NodeViewModel(string name)
    {
        Name = name;
    }
}
