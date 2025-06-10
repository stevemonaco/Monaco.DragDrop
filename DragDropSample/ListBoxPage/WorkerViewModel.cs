using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace DragDropSample.ViewModels;
public partial class WorkerViewModel : ObservableObject
{
    public int PersonId { get; init; }
    public required string Name { get; init; }
    public int Age { get; init; }
    public int Salary { get; init; }
    public int? MiningProficiency { get; init; }
    public int? CraftingProficiency { get; init; }
    public int? CombatProficiency { get; init; }
    public ObservableCollection<WorkerViewModel> DislikedWorkers { get; set; } = [];
}
