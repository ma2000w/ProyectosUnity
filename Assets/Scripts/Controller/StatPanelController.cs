using UnityEngine;
using System.Collections;

public class StatPanelController : MonoBehaviour 
{
    #region Constants
    const string ShowKey = "Show"; // Key for showing the panel
    const string HideKey = "Hide"; // Key for hiding the panel
    #endregion

    #region Fields
    [SerializeField] StatPanel primaryPanel; // Reference to the primary stat panel
    [SerializeField] StatPanel secondaryPanel; // Reference to the secondary stat panel
    
    Tweener primaryTransition; // Tweener for transitioning the primary stat panel
    Tweener secondaryTransition; // Tweener for transitioning the secondary stat panel
    #endregion

    #region MonoBehaviour
    void Start ()
    {
        // Set initial positions of the panels to hide
        if (primaryPanel.panel.CurrentPosition == null)
            primaryPanel.panel.SetPosition(HideKey, false);
        if (secondaryPanel.panel.CurrentPosition == null)
            secondaryPanel.panel.SetPosition(HideKey, false);
    }
    #endregion

    #region Public Methods
    // Show the primary stat panel with the specified game object
    public void ShowPrimary (GameObject obj)
    {
        primaryPanel.Display(obj); // Display the primary panel with the specified game object
        MovePanel(primaryPanel, ShowKey, ref primaryTransition); // Move the primary panel into view
    }

    // Hide the primary stat panel
    public void HidePrimary ()
    {
        MovePanel(primaryPanel, HideKey, ref primaryTransition); // Move the primary panel out of view
    }

    // Show the secondary stat panel with the specified game object
    public void ShowSecondary (GameObject obj)
    {
        secondaryPanel.Display(obj); // Display the secondary panel with the specified game object
        MovePanel(secondaryPanel, ShowKey, ref secondaryTransition); // Move the secondary panel into view
    }

    // Hide the secondary stat panel
    public void HideSecondary ()
    {
        MovePanel(secondaryPanel, HideKey, ref secondaryTransition); // Move the secondary panel out of view
    }
    #endregion

    #region Private Methods
    // Move the specified panel to the specified position with a transition
    void MovePanel (StatPanel obj, string pos, ref Tweener t)
    {
        Panel.Position target = obj.panel[pos]; // Get the target position for the panel
        if (obj.panel.CurrentPosition != target) // Check if the current position is not already the target position
        {
            if (t != null)
                t.Stop(); // Stop any ongoing transition
            t = obj.panel.SetPosition(pos, true); // Set the position of the panel with a transition
            t.duration = 0.5f; // Set the duration of the transition
            t.equation = EasingEquations.EaseOutQuad; // Set the easing equation for the transition
        }
    }
    #endregion
}
