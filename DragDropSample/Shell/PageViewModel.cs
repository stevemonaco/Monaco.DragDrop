using CommunityToolkit.Mvvm.ComponentModel;

namespace DragDropSample.ViewModels;
public partial class PageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? Title { get; set; }
}
