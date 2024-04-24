using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Abstract class that extends BattleState, providing a template for menu-based states.
public abstract class BaseAbilityMenuState : BattleState
{
    protected string menuTitle; // Title for the menu UI.
    protected List<string> menuOptions; // List to store the menu options displayed to the user.

    // Called when the state is entered.
    public override void Enter ()
    {
        base.Enter(); // Calls the Enter method of the base class.
        SelectTile(turn.actor.tile.pos); // Selects the tile where the actor is positioned.
        if (driver.Current == Drivers.Human)
            LoadMenu(); // If the current driver is human, load the menu options.
    }

    // Called when the state is exited.
    public override void Exit ()
    {
        base.Exit(); // Calls the Exit method of the base class.
        abilityMenuPanelController.Hide(); // Hides the menu UI panel.
    }

    // Handles "fire" input, which is typically a selection or confirmation action.
    protected override void OnFire (object sender, InfoEventArgs<int> e)
    {
        if (e.info == 0)
            Confirm(); // If the input is '0', confirm the selection.
        else
            Cancel(); // Any other input triggers a cancel action.
    }

    // Handles "move" input, which is used to navigate through menu options.
    protected override void OnMove (object sender, InfoEventArgs<Point> e)
    {
        if (e.info.x > 0 || e.info.y < 0)
            abilityMenuPanelController.Next(); // If input suggests a "next" action, move to the next menu option.
        else
            abilityMenuPanelController.Previous(); // Otherwise, move to the previous menu option.
    }

    // Abstract method to load the menu; implementation must be provided in derived classes.
    protected abstract void LoadMenu();

    // Abstract method for confirming a selection; implementation must be provided in derived classes.
    protected abstract void Confirm();

    // Abstract method for canceling the menu; implementation must be provided in derived classes.
    protected abstract void Cancel();
}
