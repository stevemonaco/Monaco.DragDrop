using CommunityToolkit.Mvvm.ComponentModel;

namespace DragDropSample.ViewModels;
public sealed partial class PersonViewModel : ObservableObject
{
    [ObservableProperty] public partial int PersonId { get; set; }

    [ObservableProperty] public partial string Name { get; set; }

    [ObservableProperty] public partial int Age { get; set; }

    [ObservableProperty] public partial string Address { get; set; }
}
