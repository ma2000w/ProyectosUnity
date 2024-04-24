using UnityEngine;

public class SlowStatusEffect : StatusEffect 
{
    Stats myStats;
    private int originalMov;

    void OnEnable ()
    {
        myStats = GetComponentInParent<Stats>() ?? GetComponent<Stats>();
        if (myStats)
        {
            originalMov = myStats[StatTypes.MOV]; // Save the original MOV value
            this.AddObserver(OnMOVWillChange, Stats.WillChangeNotification(StatTypes.MOV), myStats);
            ApplyImmediateMovCap();
        }
      
    }

    void ApplyImmediateMovCap()
    {
        if (myStats[StatTypes.MOV] > 1)
        {
            myStats.SetValue(StatTypes.MOV, 1, false); // Apply without allowing exceptions
        }
    }
    
    void OnDisable ()
    {
        if (myStats != null)
        {
            myStats.SetValue(StatTypes.MOV, originalMov, false); // Restore the original MOV value
            this.RemoveObserver(OnMOVWillChange, Stats.WillChangeNotification(StatTypes.MOV), myStats);
        }
    }
    
    void OnMOVWillChange(object sender, object args)
    {
        ValueChangeException exc = args as ValueChangeException;
        if (exc != null)
        {
            MaxValueModifier capModifier = new MaxValueModifier(1, 1);
            exc.AddModifier(capModifier);
        }
    }
}
