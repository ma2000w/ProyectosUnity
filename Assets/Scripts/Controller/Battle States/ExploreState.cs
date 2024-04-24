using UnityEngine;
using System.Collections;

public class ExploreState : BattleState 
{
    // Called when entering the exploration state.
    public override void Enter ()
    {
        base.Enter(); // Calls the Enter method of the base class.
        RefreshPrimaryStatPanel(pos); // Updates the primary stat panel with details relevant to the selected position.
    }

    // Called when exiting the exploration state.
    public override void Exit ()
    {
        base.Exit(); // Calls the Exit method of the base class.
        statPanelController.HidePrimary(); // Hides the primary statistics panel.
    }

    // Handles movement inputs during exploration.
    protected override void OnMove (object sender, InfoEventArgs<Point> e)
    {
        SelectTile(e.info + pos); // Adjusts the selected position based on the direction moved.
        RefreshPrimaryStatPanel(pos); // Refreshes the primary stat panel to show updated information for the new position.
    }
    
    // Handles 'fire' or selection inputs during exploration.
    protected override void OnFire (object sender, InfoEventArgs<int> e)
    {
        if (e.info == 0)
            owner.ChangeState<CommandSelectionState>(); // Transitions to the command selection state when an action is confirmed.
    }
}
