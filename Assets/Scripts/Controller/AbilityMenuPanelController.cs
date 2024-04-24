using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AbilityMenuPanelController : MonoBehaviour 
{
    #region Constants
    const string ShowKey = "Show";
    const string HideKey = "Hide";
    const string EntryPoolKey = "AbilityMenuPanel.Entry";
    const int MenuCount = 4;
    #endregion

    #region Fields / Properties
    [SerializeField] GameObject entryPrefab; // Prefab for menu entries.
    [SerializeField] Text titleLabel; // UI text label for displaying the title of the menu.
    [SerializeField] Panel panel; // The panel component that will contain the menu entries.
    [SerializeField] GameObject canvas; // Canvas GameObject that acts as a root for UI components.
    List<AbilityMenuEntry> menuEntries = new List<AbilityMenuEntry>(MenuCount); // List of menu entries.
    public int selection { get; private set; } // Index of the currently selected menu entry.
    #endregion

    #region MonoBehaviour
    void Awake ()
    {
        // Initializes the GameObject pool for menu entries.
        GameObjectPoolController.AddEntry(EntryPoolKey, entryPrefab, MenuCount, int.MaxValue);
    }

    void Start ()
    {
        // Sets initial panel position to "Hide" and deactivates the canvas.
        panel.SetPosition(HideKey, false);
        canvas.SetActive(false);
    }
    #endregion

    #region Public
    public void Show (string title, List<string> options)
    {
        // Activates the canvas and sets up the menu entries.
        canvas.SetActive(true);
        Clear(); // Clears any existing entries.
        titleLabel.text = title; // Sets the menu title.
        for (int i = 0; i < options.Count; ++i)
        {
            AbilityMenuEntry entry = Dequeue(); // Retrieves an entry from the pool.
            entry.Title = options[i]; // Sets the title for the entry.
            menuEntries.Add(entry); // Adds the entry to the list of active entries.
        }
        SetSelection(0); // Selects the first entry by default.
        TogglePos(ShowKey); // Animates the panel to the "Show" position.
    }

    public void Hide ()
    {
        // Initiates the hide animation and deactivates the canvas when completed.
        Tweener t = TogglePos(HideKey);
        t.completedEvent += delegate(object sender, System.EventArgs e)
        {
            if (panel.CurrentPosition == panel[HideKey])
            {
                Clear();
                canvas.SetActive(false);
            }
        };
    }

    public void SetLocked (int index, bool value)
    {
        // Locks or unlocks a menu entry and adjusts the selection if necessary.
        if (index < 0 || index >= menuEntries.Count)
            return;

        menuEntries[index].IsLocked = value;
        if (value && selection == index)
            Next(); // Moves to the next available entry if the current selection gets locked.
    }

    public void Next ()
    {
        // Cycles forward through the menu entries to find an unlocked item.
        for (int i = selection + 1; i < selection + menuEntries.Count; ++i)
        {
            int index = i % menuEntries.Count;
            if (SetSelection(index))
                break;
        }
    }

    public void Previous ()
    {
        // Cycles backward through the menu entries to find an unlocked item.
        for (int i = selection - 1 + menuEntries.Count; i > selection; --i)
        {
            int index = i % menuEntries.Count;
            if (SetSelection(index))
                break;
        }
    }
    #endregion

    #region Private
    AbilityMenuEntry Dequeue ()
    {
        // Retrieves a pooled entry or creates a new one if necessary.
        Poolable p = GameObjectPoolController.Dequeue(EntryPoolKey);
        AbilityMenuEntry entry = p.GetComponent<AbilityMenuEntry>();
        entry.transform.SetParent(panel.transform, false);
        entry.transform.localScale = Vector3.one;
        entry.gameObject.SetActive(true);
        entry.Reset();
        return entry;
    }

    void Enqueue (AbilityMenuEntry entry)
    {
        // Returns an entry to the pool.
        Poolable p = entry.GetComponent<Poolable>();
        GameObjectPoolController.Enqueue(p);
    }
	void Clear ()
	{
		for (int i = menuEntries.Count - 1; i >= 0; --i)
			Enqueue(menuEntries[i]);
		menuEntries.Clear();
	}

	bool SetSelection (int value)
	{
		if (menuEntries[value].IsLocked)
			return false;
		
		// Deselect the previously selected entry
		if (selection >= 0 && selection < menuEntries.Count)
			menuEntries[selection].IsSelected = false;
		
		selection = value;
		
		// Select the new entry
		if (selection >= 0 && selection < menuEntries.Count)
			menuEntries[selection].IsSelected = true;
		
		return true;
	}

	Tweener TogglePos (string pos)
	{
		Tweener t = panel.SetPosition(pos, true);
		t.duration = 0.5f;
		t.equation = EasingEquations.EaseOutQuad;
		return t;
	}
	#endregion
}
