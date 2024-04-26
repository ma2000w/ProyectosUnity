using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnOrderController : MonoBehaviour 
{
    #region Constants
    const int turnActivation = 1000; // Threshold for activating a unit's turn
    const int turnCost = 500; // Cost of a unit's turn
    const int moveCost = 300; // Cost of moving during a turn
    const int actionCost = 200; // Cost of taking an action during a turn
    #endregion

    #region Notifications
    public const string RoundBeganNotification = "TurnOrderController.roundBegan"; // Notification for the beginning of a round
    public const string TurnCheckNotification = "TurnOrderController.turnCheck"; // Notification for checking if a unit can take a turn
    public const string TurnBeganNotification = "TurnOrderController.TurnBeganNotification"; // Notification for the beginning of a turn
    public const string TurnCompletedNotification = "TurnOrderController.turnCompleted"; // Notification for the completion of a turn
    public const string RoundEndedNotification = "TurnOrderController.roundEnded"; // Notification for the end of a round
    #endregion

    #region Public Methods
    // Coroutine for managing the rounds in the battle
    public IEnumerator Round ()
    {
        BattleController bc = GetComponent<BattleController>(); // Get the BattleController component
        while (true) // Loop indefinitely for continuous rounds
        {
            this.PostNotification(RoundBeganNotification); // Post notification for the beginning of a round

            List<Unit> units = new List<Unit>( bc.units ); // Create a list of units in the battle
            for (int i = 0; i < units.Count; ++i) // Loop through each unit
            {
                Stats s = units[i].GetComponent<Stats>(); // Get the Stats component of the unit
                s[StatTypes.CTR] += s[StatTypes.SPD]; // Increase the counter of the unit based on its speed
            }

            units.Sort( (a,b) => GetCounter(a).CompareTo(GetCounter(b)) ); // Sort the units based on their counters

            for (int i = units.Count - 1; i >= 0; --i) // Loop through each unit in reverse order
            {
                if (CanTakeTurn(units[i])) // Check if the unit can take a turn
                {
                    bc.turn.Change(units[i]); // Change the current turn to the unit
                    units[i].PostNotification(TurnBeganNotification); // Post notification for the beginning of the unit's turn

                    yield return units[i]; // Yield to allow the unit to take its turn

                    int cost = turnCost; // Initialize the cost of the turn
                    if (bc.turn.hasUnitMoved) // Check if the unit has moved during its turn
                        cost += moveCost; // Add the move cost to the total cost
                    if (bc.turn.hasUnitActed) // Check if the unit has taken an action during its turn
                        cost += actionCost; // Add the action cost to the total cost

                    Stats s = units[i].GetComponent<Stats>(); // Get the Stats component of the unit
                    s.SetValue(StatTypes.CTR, s[StatTypes.CTR] - cost, false); // Deduct the cost from the unit's counter

                    units[i].PostNotification(TurnCompletedNotification); // Post notification for the completion of the unit's turn
                }
            }
            
            this.PostNotification(RoundEndedNotification); // Post notification for the end of a round
        }
    }
    #endregion

    #region Private Methods
    // Check if a unit can take a turn
    bool CanTakeTurn (Unit target)
    {
        BaseException exc = new BaseException( GetCounter(target) >= turnActivation ); // Create a base exception with the condition if the unit can take a turn
        target.PostNotification( TurnCheckNotification, exc ); // Post notification to check if the unit can take a turn
        return exc.toggle; // Return the result of the check
    }

    // Get the counter value of a unit
    int GetCounter (Unit target)
    {
        return target.GetComponent<Stats>()[StatTypes.CTR]; // Return the value of the counter from the unit's Stats component
    }
    #endregion
}
