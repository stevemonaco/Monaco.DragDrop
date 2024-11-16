using CommunityToolkit.Mvvm.ComponentModel;

namespace DragDropSample.ViewModels;
public partial class PageViewModel : ObservableObject
{
    [ObservableProperty] private string? _title;
}
