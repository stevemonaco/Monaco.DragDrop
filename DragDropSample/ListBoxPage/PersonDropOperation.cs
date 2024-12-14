using Avalonia.Input;
using DragDropSample.ViewModels;
using Monaco.DragDrop;
using System.Collections.ObjectModel;
using System.Linq;

namespace DragDropSample.CustomDragDrop;

/// <summary>
/// Adds specialized validation to check if the person is a fit for the team
/// </summary>
public class PersonDropOperation : CollectionDropOperation
{
    protected override bool CanDrop(DragEventArgs e)
    {
        if (!CanGetPayload(e) || !TryGetMetadata<CollectionDragMetadata>(e, out var metadata))
            return false;

        // Ensure various data are available
        if (metadata.PayloadCollection is not ObservableCollection<PersonViewModel> personSource // Not necessary for this validation
            || PayloadTarget is not ObservableCollection<PersonViewModel> personDest // Not necessary for this validation
            || !TryGetPayload<PersonViewModel>(e, out var person)
            || AttachedControl is not { DataContext: ListBoxPageViewModel vm })
            return false;

        // InteractionIds adds some extra necessary information since we didn't create a specialized TeamViewModel type
        return InteractionIds switch
        {
            ["app:mining"] => Validate(person, person.MiningProficiency, vm.MiningTeam, vm.MiningBudgetRemaining),
            ["app:crafting"] => Validate(person, person.CraftingProficiency, vm.CraftingTeam, vm.CraftingBudgetRemaining),
            ["app:combat"] => Validate(person, person.CombatProficiency, vm.CombatTeam, vm.CombatBudgetRemaining),
            _ => false
        };
    }

    private bool Validate(PersonViewModel person, int? proficiency, ObservableCollection<PersonViewModel> team, int budgetRemaining)
    {
        if (proficiency is null)
        {
            DropAdorner!.ErrorMessage = "Worker is not proficient in this area";
            return false;
        }

        if (budgetRemaining - person.Salary < 0)
        {
            DropAdorner!.ErrorMessage = "Team budget does not allow";
            return false;
        }

        if (person.DislikedPeople.Any(team.Contains))
        {
            DropAdorner!.ErrorMessage = $"{person.Name} doesn't like someone in the team";
            return false;
        }

        if (team.SelectMany(x => x.DislikedPeople).Contains(person))
        {
            DropAdorner!.ErrorMessage = $"Someone in the team doesn't like {person.Name}";
            return false;
        }

        DropAdorner!.ErrorMessage = null;
        return true;
    }
}
