using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Alias for a list of GameObjects representing a party
using Party = System.Collections.Generic.List<UnityEngine.GameObject>;

public static class ExperienceManager
{
    // Constants defining the minimum and maximum level bonuses for awarding experience
    const float minLevelBonus = 1.5f;
    const float maxLevelBonus = 0.5f;

    // Method for awarding experience to a party of GameObjects
    public static void AwardExperience(int amount, Party party)
    {
        // Create a list to store the Rank components of the party members
        List<Rank> ranks = new List<Rank>(party.Count);

        // Iterate through the party members to gather their Rank components
        for (int i = 0; i < party.Count; ++i)
        {
            Rank r = party[i].GetComponent<Rank>(); // Get the Rank component of the party member
            if (r != null)
                ranks.Add(r); // Add the Rank component to the list
        }

        // Step 1: Determine the range of actor levels within the party
        int min = int.MaxValue; // Initialize the minimum level to the maximum possible value
        int max = int.MinValue; // Initialize the maximum level to the minimum possible value
        for (int i = ranks.Count - 1; i >= 0; --i)
        {
            min = Mathf.Min(ranks[i].LVL, min); // Update the minimum level
            max = Mathf.Max(ranks[i].LVL, max); // Update the maximum level
        }

        // Step 2: Weight the amount to award per actor based on their level
        float[] weights = new float[ranks.Count]; // Create an array to store the weight for each actor
        float summedWeights = 0; // Initialize the sum of weights to 0
        for (int i = ranks.Count - 1; i >= 0; --i)
        {
            // Calculate the percentage of the actor's level within the range of levels
            float percent = (float)(ranks[i].LVL - min) / (float)(max - min);
            // Interpolate the weight based on the level percentage
            weights[i] = Mathf.Lerp(minLevelBonus, maxLevelBonus, percent);
            // Add the weight to the sum of weights
            summedWeights += weights[i];
        }

        // Step 3: Distribute the weighted award among the party members
        for (int i = ranks.Count - 1; i >= 0; --i)
        {
            // Calculate the sub-amount of experience to award to the actor based on their weight
            int subAmount = Mathf.FloorToInt((weights[i] / summedWeights) * amount);
            // Add the sub-amount of experience to the actor's experience points
            ranks[i].EXP += subAmount;
        }
    }
}
