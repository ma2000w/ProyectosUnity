using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbilityTargetState : BattleState 
{
    List<Tile> tiles; // List to store the tiles affected by the current ability.
    AbilityRange ar; // Component that determines the range of the ability.

    // Called when entering this state.
    public override void Enter()
    {
        base.Enter(); // Calls the Enter method of the base class (BattleState).
        ar = turn.ability.GetComponent<AbilityRange>(); // Gets the AbilityRange component from the current ability.
        SelectTiles(); // Determines and highlights tiles affected by the ability.
        statPanelController.ShowPrimary(turn.actor.gameObject); // Shows primary stats for the actor using the ability.

        if (ar.directionOriented)
        {
            RefreshSecondaryStatPanel(pos); // Refreshes secondary stats based on position if ability is direction-oriented.
        }

        if (driver.Current == Drivers.Computer)
        {
            StartCoroutine(ComputerHighlightTarget()); // Starts coroutine for AI to highlight target.
        }
    }

    // Called when exiting this state.
    public override void Exit()
    {
        base.Exit(); // Calls the Exit method of the base class (BattleState).
        board.DeSelectTiles(tiles); // Deselects all highlighted tiles.
        statPanelController.HidePrimary(); // Hides the primary stat panel.
        statPanelController.HideSecondary(); // Hides the secondary stat panel.
    }

    // Called when there is a move command.
    protected override void OnMove(object sender, InfoEventArgs<Point> e)
    {
        if (ar.directionOriented)
        {
            ChangeDirection(e.info); // Changes direction of the actor if the ability is direction-oriented.
        }
        else
        {
            SelectTile(e.info + pos); // Selects new tile based on move command.
            RefreshSecondaryStatPanel(pos); // Refreshes the secondary stat panel.
        }
    }

    // Called when there is a fire command.
    protected override void OnFire(object sender, InfoEventArgs<int> e)
    {
        if (e.info == 0)
        {
            if (ar.directionOriented || tiles.Contains(board.GetTile(pos)))
                owner.ChangeState<ConfirmAbilityTargetState>(); // Proceeds to confirmation state if conditions are met.
        }
        else
        {
            owner.ChangeState<CategorySelectionState>(); // Changes state to category selection if the fire command is not for confirmation.
        }
    }

    // Changes the direction based on input.
    void ChangeDirection(Point p)
    {
        Directions dir = p.GetDirection(); // Calculates the new direction based on input point.
        if (turn.actor.dir != dir) // If direction has changed:
        {
            board.DeSelectTiles(tiles); // Deselects all highlighted tiles.
            turn.actor.dir = dir; // Updates actor's direction.
            turn.actor.Match(); // Updates actor's position or orientation.
            SelectTiles(); // Re-selects tiles based on new direction.
        }
    }

    // Selects tiles based on the range of the ability.
    void SelectTiles()
    {
        tiles = ar.GetTilesInRange(board); // Gets tiles in range from the AbilityRange component.
        board.SelectTiles(tiles); // Highlights these tiles on the board.
    }

    // Coroutine for computer-controlled actor to highlight a target.
    IEnumerator ComputerHighlightTarget()
    {
        if (ar.directionOriented)
        {
            ChangeDirection(turn.plan.attackDirection.GetNormal()); // Changes direction to normal attack direction.
            yield return new WaitForSeconds(0.25f); // Waits for a quarter second.
        }
        else
        {
            Point cursorPos = pos; // Starts at current position.
            // Moves cursor to the planned fire location:
            while (cursorPos != turn.plan.fireLocation)
            {
                if (cursorPos.x < turn.plan.fireLocation.x) cursorPos.x++;
                if (cursorPos.x > turn.plan.fireLocation.x) cursorPos.x--;
                if (cursorPos.y < turn.plan.fireLocation.y) cursorPos.y++;
                if (cursorPos.y > turn.plan.fireLocation.y) cursorPos.y--;
                SelectTile(cursorPos); // Selects tile at cursor position.
                yield return new WaitForSeconds(0.25f); // Waits for a quarter second.
            }
        }
        yield return new WaitForSeconds(0.5f); // Waits for half a second.
        owner.ChangeState<ConfirmAbilityTargetState>(); // Changes state to confirm ability target.
    }
}
