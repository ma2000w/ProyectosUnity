using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfirmAbilityTargetState : BattleState
{
    List<Tile> tiles; // Holds the tiles affected by the ability area.
    AbilityArea aa; // Component that calculates the area affected by the ability.
    int index = 0; // Index for cycling through targets.

    // Called when entering this state.
    public override void Enter ()
    {
        base.Enter();
        aa = turn.ability.GetComponent<AbilityArea>(); // Get the AbilityArea component from the current ability.
        tiles = aa.GetTilesInArea(board, pos); // Compute which tiles are in the ability's area.
        board.SelectTiles(tiles); // Highlight these tiles on the board.
        FindTargets(); // Identify valid targets within these tiles.
        RefreshPrimaryStatPanel(turn.actor.tile.pos); // Update primary stat panel with actor's info.

        // Show hit success indicator if there are targets and the driver is human.
        if (turn.targets.Count > 0)
        {
            if (driver.Current == Drivers.Human)
                hitSuccessIndicator.Show();
            SetTarget(0); // Set initial target for confirmation.
        }

        // If the driver is a computer, start a coroutine to handle ability selection automatically.
        if (driver.Current == Drivers.Computer)
            StartCoroutine(ComputerDisplayAbilitySelection());
    }

    // Called when exiting this state.
    public override void Exit ()
    {
        base.Exit();
        board.DeSelectTiles(tiles); // Deselect all highlighted tiles.
        statPanelController.HidePrimary(); // Hide primary stat panel.
        statPanelController.HideSecondary(); // Hide secondary stat panel.
        hitSuccessIndicator.Hide(); // Hide the hit success indicator.
    }

    // Handles directional inputs to cycle through potential targets.
    protected override void OnMove (object sender, InfoEventArgs<Point> e)
    {
        if (e.info.y > 0 || e.info.x > 0)
            SetTarget(index + 1); // Cycle forward through targets.
        else
            SetTarget(index - 1); // Cycle backward through targets.
    }

    // Handles confirmation or cancellation of the ability target.
    protected override void OnFire (object sender, InfoEventArgs<int> e)
    {
        if (e.info == 0)
        {
            // Confirm and proceed to perform the ability if there are targets.
            if (turn.targets.Count > 0)
            {
                owner.ChangeState<PerformAbilityState>();
            }
        }
        else
            owner.ChangeState<AbilityTargetState>(); // Revert to target selection state.
    }

    // Identify valid targets from the affected tiles.
    void FindTargets ()
    {
        turn.targets = new List<Tile>();
        for (int i = 0; i < tiles.Count; ++i)
            if (turn.ability.IsTarget(tiles[i]))
                turn.targets.Add(tiles[i]);
    }

    // Sets the current target for review or adjustment.
    void SetTarget (int target)
    {
        index = target % turn.targets.Count; // Wrap around the target index.
        if (index < 0)
            index += turn.targets.Count; // Ensure the index is positive.

        if (turn.targets.Count > 0)
        {
            RefreshSecondaryStatPanel(turn.targets[index].pos); // Update secondary stats for the current target.
            UpdateHitSuccessIndicator(); // Update the hit success indicator based on the current target.
        }
    }

    // Update the hit and effect predictions for the selected target.
    void UpdateHitSuccessIndicator ()
    {
        int chance = 0, amount = 0;
        Tile target = turn.targets[index];

        // Check each child of the ability for target effects and calculate the impact.
        Transform obj = turn.ability.transform;
        for (int i = 0; i < obj.childCount; ++i)
        {
            AbilityEffectTarget targeter = obj.GetChild(i).GetComponent<AbilityEffectTarget>();
            if (targeter && targeter.IsTarget(target))
            {
                HitRate hitRate = targeter.GetComponent<HitRate>();
                chance = hitRate.Calculate(target);

                BaseAbilityEffect effect = targeter.GetComponent<BaseAbilityEffect>();
                amount = effect.Predict(target);
                break; // Exit after finding the applicable targeter.
            }
        }

        hitSuccessIndicator.SetStats(chance, amount); // Display hit and effect predictions.
    }
	IEnumerator ComputerDisplayAbilitySelection ()
	{
		owner.battleMessageController.Display(turn.ability.name);
		yield return new WaitForSeconds (2f);
		owner.ChangeState<PerformAbilityState>();
	}
}