using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommandSelectionState : BaseAbilityMenuState 
{
    // Called when entering the state. Sets up the UI and potentially starts a computer-controlled turn.
    public override void Enter ()
    {
        base.Enter();
        statPanelController.ShowPrimary(turn.actor.gameObject); // Displays primary statistics for the active unit.
        if (driver.Current == Drivers.Computer)
            StartCoroutine(ComputerTurn()); // If the current driver is a computer, start the AI turn process.
    }

    // Called when exiting the state. Cleans up the UI.
    public override void Exit ()
    {
        base.Exit();
        statPanelController.HidePrimary(); // Hides the primary statistics panel.
    }

    // Initializes or refreshes the command menu.
    protected override void LoadMenu ()
    {
        if (menuOptions == null)
        {
            menuTitle = "Commands"; // Sets the title for the menu.
            menuOptions = new List<string>(3); // Initializes the menu options list.
            menuOptions.Add("Move");
            menuOptions.Add("Action");
            menuOptions.Add("Wait");
        }

        abilityMenuPanelController.Show(menuTitle, menuOptions); // Displays the command menu.
        abilityMenuPanelController.SetLocked(0, turn.hasUnitMoved); // Locks the "Move" option if the unit has moved.
        abilityMenuPanelController.SetLocked(1, turn.hasUnitActed); // Locks the "Action" option if the unit has acted.
    }

    // Responds to a confirmation input based on the selected menu option.
    protected override void Confirm ()
    {
        switch (abilityMenuPanelController.selection)
        {
        case 0: // Move
            owner.ChangeState<MoveTargetState>(); // Changes to the state for moving the unit.
            break;
        case 1: // Action
            owner.ChangeState<CategorySelectionState>(); // Changes to the state for selecting an action category.
            break;
        case 2: // Wait
            owner.ChangeState<EndFacingState>(); // Changes to the state for ending the turn and setting facing direction.
            break;
        }
    }

    // Handles cancellation actions, such as undoing a move.
    protected override void Cancel ()
    {
        if (turn.hasUnitMoved && !turn.lockMove)
        {
            turn.UndoMove(); // Undoes the last move if possible.
            abilityMenuPanelController.SetLocked(0, false); // Unlocks the move option.
            SelectTile(turn.actor.tile.pos); // Re-selects the tile of the unit.
        }
        else
        {
            owner.ChangeState<ExploreState>(); // Changes to a state suitable for exploring the game world.
        }
    }

    // Coroutine for handling AI turns.
    IEnumerator ComputerTurn ()
    {
        if (turn.plan == null)
        {
            turn.plan = owner.cpu.Evaluate(); // Evaluates the best move/ability for the AI.
            turn.ability = turn.plan.ability; // Sets the chosen ability as part of the plan.
        }

        yield return new WaitForSeconds(1f); // Waits for a second before executing the AI's plan.

        // Determines the next state based on the AI's plan.
        if (!turn.hasUnitMoved && turn.plan.moveLocation != turn.actor.tile.pos)
            owner.ChangeState<MoveTargetState>(); // If the unit hasn't moved and has a new position to move to.
        else if (!turn.hasUnitActed && turn.plan.ability != null)
            owner.ChangeState<AbilityTargetState>(); // If the unit hasn't acted and has an ability to perform.
        else
            owner.ChangeState<EndFacingState>(); // If no other actions, proceed to end the turn.
    }
}
