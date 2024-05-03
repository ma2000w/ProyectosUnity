using UnityEngine;
using System.Collections;

public class AutoStatusController : MonoBehaviour 
{
    // Method is called when the MonoBehaviour is enabled.
    void OnEnable ()
    {
        // Add an observer to listen for notifications when the HP stat of the associated Stats component changes.
        this.AddObserver(OnHPDidChangeNotification, Stats.DidChangeNotification(StatTypes.HP));
    }
    
    // Method is called when the MonoBehaviour is disabled.
    void OnDisable ()
    {
        // Remove the observer that was added in OnEnable to avoid memory leaks.
        this.RemoveObserver(OnHPDidChangeNotification, Stats.DidChangeNotification(StatTypes.HP));
    }
    
    // Method is the callback function that is invoked when the HP stat of the associated Stats component changes.
    void OnHPDidChangeNotification (object sender, object args)
    {
        // Cast the sender as Stats to access its properties.
        Stats stats = sender as Stats;

        // Check if the HP has reached zero.
        if (stats[StatTypes.HP] == 0)
        {
            // Retrieve the Status component (if any) attached to the same GameObject.
            Status status = stats.GetComponentInChildren<Status>();

            // Add a KnockOutStatusEffect status effect to the Status component.
            // StatComparisonCondition c is initialized to check if the HP is equal to zero.
            StatComparisonCondition c = status.Add<KnockOutStatusEffect, StatComparisonCondition>();
            c.Init(StatTypes.HP, 0, c.EqualTo);
        }
    }
}
