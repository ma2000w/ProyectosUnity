using UnityEngine;
using System.Collections;

public class EndFacingState : BattleState 
{
    Directions startDir; // Variable to store the initial direction of the actor for potential reversion.

    // Called when the state is entered.
    public override void Enter ()
    {
        base.Enter(); // Calls the Enter method of the base class.
        startDir = turn.actor.dir; // Saves the current direction of the actor.
        SelectTile(turn.actor.tile.pos); // Highlights the tile the actor is currently on.
        owner.facingIndicator.gameObject.SetActive(true); // Activates the facing indicator UI.
        owner.facingIndicator.SetDirection(turn.actor.dir); // Sets the direction of the facing indicator to match the actor's.

        // If the current driver is computer-controlled, initiate automated facing direction selection.
        if (driver.Current == Drivers.Computer)
            StartCoroutine(ComputerControl());
    }

    // Called when the state is exited.
    public override void Exit ()
    {
        owner.facingIndicator.gameObject.SetActive(false); // Deactivates the facing indicator UI.
        base.Exit(); // Calls the Exit method of the base class.
    }
    
    // Handles directional input for manually setting the direction.
    protected override void OnMove (object sender, InfoEventArgs<Point> e)
    {
        turn.actor.dir = e.info.GetDirection(); // Updates the direction based on the input.
        turn.actor.Match(); // Updates the actor's position or animation to match the new direction.
        owner.facingIndicator.SetDirection(turn.actor.dir); // Updates the facing indicator to the new direction.
    }
    
    // Handles the selection input to confirm or revert the direction change.
    protected override void OnFire (object sender, InfoEventArgs<int> e)
    {
        switch (e.info)
        {
        case 0:
            // Confirm the new facing direction and change to the state for selecting another unit.
            owner.ChangeState<SelectUnitState>();
            break;
        case 1:
            // Revert to the original direction and go back to the command selection state.
            turn.actor.dir = startDir;
            turn.actor.Match();
            owner.ChangeState<CommandSelectionState>();
            break;
        }
    }

    // Coroutine for handling computer-controlled facing direction.
    IEnumerator ComputerControl ()
    {
        yield return new WaitForSeconds(0.5f); // Waits briefly before setting the direction.
        turn.actor.dir = owner.cpu.DetermineEndFacingDirection(); // Sets the direction based on computer logic.
        turn.actor.Match(); // Updates the actor's position or animation to match the new direction.
        owner.facingIndicator.SetDirection(turn.actor.dir); // Updates the facing indicator.
        yield return new WaitForSeconds(0.5f); // Waits briefly before moving to the next state.
        owner.ChangeState<SelectUnitState>(); // Changes to the state for selecting another unit.
    }
}
