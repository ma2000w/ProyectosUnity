using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveTargetState : BattleState
{
    List<Tile> tiles; // List to hold the tiles within movement range.
    
    // Called when the state is entered.
    public override void Enter ()
    {
        base.Enter(); // Ensures that the base class's Enter method is also executed.
        Movement mover = turn.actor.GetComponent<Movement>(); // Retrieves the Movement component of the active unit.
        tiles = mover.GetTilesInRange(board); // Gets the tiles that the unit can move to based on its movement capabilities.
        board.SelectTiles(tiles); // Highlights these tiles on the board.
        RefreshPrimaryStatPanel(pos); // Updates the primary statistics panel to reflect the current state.
        if (driver.Current == Drivers.Computer)
            StartCoroutine(ComputerHighlightMoveTarget()); // If the current player is computer-controlled, execute the movement automatically.
    }
    
    // Called when the state is exited.
    public override void Exit ()
    {
        base.Exit(); // Ensures that the base class's Exit method is also executed.
        board.DeSelectTiles(tiles); // Deselects all previously selected tiles.
        tiles = null; // Clears the list of tiles.
        statPanelController.HidePrimary(); // Hides the primary statistics panel.
    }
    
    // Handles movement input from the player.
    protected override void OnMove (object sender, InfoEventArgs<Point> e)
    {
        SelectTile(e.info + pos); // Adjusts the selected tile based on the input direction.
        RefreshPrimaryStatPanel(pos); // Refreshes the primary statistics panel to reflect the new tile selection.
    }
    
    // Handles "fire" input which typically involves confirming or canceling the movement.
    protected override void OnFire (object sender, InfoEventArgs<int> e)
    {
        if (e.info == 0)
        {
            if (tiles.Contains(owner.currentTile)) // Checks if the currently selected tile is within the valid movement range.
                owner.ChangeState<MoveSequenceState>(); // Proceeds to execute the movement sequence.
        }
        else
        {
            owner.ChangeState<CommandSelectionState>(); // Returns to command selection state if canceled.
        }
    }

    // Coroutine to automatically handle computer-controlled movement.
    IEnumerator ComputerHighlightMoveTarget ()
    {
        Point cursorPos = pos; // Starts from the current position.
        // Gradually moves the cursor to the planned move location.
        while (cursorPos != turn.plan.moveLocation)
        {
            if (cursorPos.x < turn.plan.moveLocation.x) cursorPos.x++;
            if (cursorPos.x > turn.plan.moveLocation.x) cursorPos.x--;
            if (cursorPos.y < turn.plan.moveLocation.y) cursorPos.y++;
            if (cursorPos.y > turn.plan.moveLocation.y) cursorPos.y--;
            SelectTile(cursorPos); // Selects the new tile at each step.
            yield return new WaitForSeconds(0.25f); // Pauses briefly between moves to visually represent the movement.
        }
        yield return new WaitForSeconds(0.5f); // Adds a final pause for emphasis.
        owner.ChangeState<MoveSequenceState>(); // Changes to the move sequence state to execute the move.
    }
}
