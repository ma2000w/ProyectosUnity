using UnityEngine;
using System.Collections;


public class MaxValueModifier : ValueModifier 
{
    public float max;

    public MaxValueModifier (int sortOrder, float max) : base (sortOrder)
    {
        this.max = max;
    }

    public override float Modify (float fromValue, float toValue)
    {
        return Mathf.Min(toValue, max);  // Use Mathf.Min to enforce the maximum limit
    }
}