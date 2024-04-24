using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Manages the state for selecting action categories, like different types of abilities.
public class CategorySelectionState : BaseAbilityMenuState 
{
    // Called when entering the state.
    public override void Enter ()
    {
        base.Enter(); // Calls the Enter method of the base class to handle common entrance behavior.
        statPanelController.ShowPrimary(turn.actor.gameObject); // Shows primary statistics for the active actor.
    }
	
    // Called when exiting the state.
    public override void Exit ()
    {
        base.Exit(); // Calls the Exit method of the base class to handle common exit behavior.
        statPanelController.HidePrimary(); // Hides the primary statistics panel.
    }

    // Loads the menu with available action categories.
    protected override void LoadMenu ()
    {
        if (menuOptions == null)
            menuOptions = new List<string>(); // Initializes the list of menu options if it's null.
        else
            menuOptions.Clear(); // Clears existing options if the list is already initialized.

        menuTitle = "Action"; // Sets the title of the menu.
        menuOptions.Add("Attack"); // Adds a default "Attack" option.

        AbilityCatalog catalog = turn.actor.GetComponentInChildren<AbilityCatalog>(); // Retrieves the ability catalog from the actor.
        for (int i = 0; i < catalog.CategoryCount(); ++i)
            menuOptions.Add(catalog.GetCategory(i).name); // Adds each ability category to the menu options.
		
        abilityMenuPanelController.Show(menuTitle, menuOptions); // Displays the menu with the title and options.
    }

    // Handles the confirmation action when a menu option is selected.
    protected override void Confirm ()
    {
        if (abilityMenuPanelController.selection == 0)
            Attack(); // If "Attack" is selected, perform the attack action.
        else
            SetCategory(abilityMenuPanelController.selection - 1); // Sets the category for other abilities and changes state.
    }
	
    // Handles the cancellation action, typically returning to the previous menu.
    protected override void Cancel ()
    {
        owner.ChangeState<CommandSelectionState>(); // Changes the state back to command selection.
    }

    // Triggers the attack action by setting the corresponding ability and changing state.
    void Attack ()
    {
        turn.ability = turn.actor.GetComponentInChildren<Ability>(); // Sets the actor's default attack ability.
        owner.ChangeState<AbilityTargetState>(); // Changes the state to target selection for the ability.
    }

    // Sets the selected category for further action selection and changes the state.
    void SetCategory (int index)
    {
        ActionSelectionState.category = index; // Sets the static category index used by the ActionSelectionState.
        owner.ChangeState<ActionSelectionState>(); // Changes the state to the specific action selection.
    }
}
