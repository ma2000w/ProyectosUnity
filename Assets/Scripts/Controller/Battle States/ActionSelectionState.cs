using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Defines a state for selecting an action from a list of abilities.
public class ActionSelectionState : BaseAbilityMenuState 
{
    public static int category; // Static variable to hold the ability category.
    AbilityCatalog catalog; // Reference to the catalog of abilities.

    // Called when this state is entered.
    public override void Enter ()
    {
        base.Enter(); // Calls the Enter method of the base class.
        statPanelController.ShowPrimary(turn.actor.gameObject); // Displays primary stats of the actor taking the turn.
    }
	
    // Called when this state is exited.
    public override void Exit ()
    {
        base.Exit(); // Calls the Exit method of the base class.
        statPanelController.HidePrimary(); // Hides the primary stats panel.
    }

    // Loads the abilities menu based on the selected category.
    protected override void LoadMenu ()
    {
        catalog = turn.actor.GetComponentInChildren<AbilityCatalog>(); // Retrieves the AbilityCatalog component.
        GameObject container = catalog.GetCategory(category); // Fetches the category container based on the selected category.
        menuTitle = container.name; // Sets the menu title to the container's name.

        int count = catalog.AbilityCount(container); // Determines the number of abilities in this category.
        if (menuOptions == null)
            menuOptions = new List<string>(count); // Initializes the menuOptions list if it's null.
        else
            menuOptions.Clear(); // Clears existing options if the list already exists.

        bool[] locks = new bool[count]; // Array to keep track of which abilities are locked.
        for (int i = 0; i < count; ++i)
        {
            Ability ability = catalog.GetAbility(category, i); // Retrieves each ability.
            AbilityMagicCost cost = ability.GetComponent<AbilityMagicCost>(); // Gets the cost component of the ability.
            if (cost)
                menuOptions.Add(string.Format("{0}: {1}", ability.name, cost.amount)); // Formats the ability name with its cost.
            else
                menuOptions.Add(ability.name); // Adds just the ability name if there's no cost component.
            locks[i] = !ability.CanPerform(); // Sets lock status based on whether the ability can be performed.
        }

        abilityMenuPanelController.Show(menuTitle, menuOptions); // Displays the menu with the options.
        for (int i = 0; i < count; ++i)
            abilityMenuPanelController.SetLocked(i, locks[i]); // Locks or unlocks menu items as necessary.
    }

    // Called when an option is confirmed.
    protected override void Confirm ()
    {
        turn.ability = catalog.GetAbility(category, abilityMenuPanelController.selection); // Sets the selected ability as the current turn's ability.
        owner.ChangeState<AbilityTargetState>(); // Changes the state to targeting for the selected ability.
    }

    // Called when the action is cancelled.
    protected override void Cancel ()
    {
        owner.ChangeState<CategorySelectionState>(); // Reverts to the category selection state.
    }
}
