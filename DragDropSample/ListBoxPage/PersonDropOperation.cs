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
        if (!CanGetPayload(e))
            return false;

        var metadata = GetMetadata(e);

        // Ensure various data are available
        if (SourceCollection is not ObservableCollection<PersonViewModel> personSource // Not necessary for this validation
            || PayloadTarget is not ObservableCollection<PersonViewModel> personDest // Not necessary for this validation
            || GetPayload(e, metadata) is not PersonViewModel person
            || AttachedControl is not { DataContext: ListBoxPageViewModel vm })
            return false;

        // InteractionIds adds some extra necessary information since we didn't aggregate into a TeamViewModel
        return InteractionIds switch
        {
            ["mining"] => Validate(person, person.MiningProficiency, vm.MiningTeam, vm.MiningBudgetRemaining),
            ["crafting"] => Validate(person, person.CraftingProficiency, vm.CraftingTeam, vm.CraftingBudgetRemaining),
            ["combat"] => Validate(person, person.CombatProficiency, vm.CombatTeam, vm.CombatBudgetRemaining),
            _ => false
        };
    }

    private bool Validate(PersonViewModel person, int? proficiency, ObservableCollection<PersonViewModel> team, int budgetRemaining)
    {
        if (proficiency is null) // Worker is not proficient in this area
            return false;

        if (budgetRemaining - person.Salary < 0) // Team budget does not allow
            return false;

        if (person.DislikedPeople.Any(team.Contains)) // Person doesn't like working with someone in the team
            return false;

        if (team.SelectMany(x => x.DislikedPeople).Contains(person)) // Team doesn't like person
            return false;

        return true;
    }
}
