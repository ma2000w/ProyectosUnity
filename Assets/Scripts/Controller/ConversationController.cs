using UnityEngine;
using System;
using System.Collections;

public class ConversationController : MonoBehaviour 
{
	#region Events
	// Event triggered when the conversation is completed
	public static event EventHandler completeEvent;
	#endregion

	#region Constants
	// Constants for panel positions
	const string ShowTop = "Show Top";
	const string ShowBottom = "Show Bottom";
	const string HideTop = "Hide Top";
	const string HideBottom = "Hide Bottom";
	#endregion

	#region Fields
	[SerializeField] ConversationPanel leftPanel; // Reference to the left conversation panel
	[SerializeField] ConversationPanel rightPanel; // Reference to the right conversation panel

	Canvas canvas; // Reference to the canvas
	IEnumerator conversation; // Enumerator for handling conversation sequence
	Tweener transition; // Tweener for handling panel transitions
	#endregion

	#region MonoBehaviour
	void Start ()
	{
		// Get the canvas component
		canvas = GetComponentInChildren<Canvas>();

		// Set initial panel positions
		if (leftPanel.panel.CurrentPosition == null)
			leftPanel.panel.SetPosition(HideBottom, false);
		if (rightPanel.panel.CurrentPosition == null)
			rightPanel.panel.SetPosition(HideBottom, false);

		// Deactivate the canvas
		canvas.gameObject.SetActive(false);
	}
	#endregion

	#region Public
	// Start showing the conversation with the provided data
	public void Show (ConversationData data)
	{
		// Activate the canvas
		canvas.gameObject.SetActive(true);
		// Start the conversation sequence
		conversation = Sequence(data);
		conversation.MoveNext();
	}

	// Proceed to the next message in the conversation
	public void Next ()
	{
		// Check if conversation is ongoing and transition is not in progress
		if (conversation == null || transition != null)
			return;
		
		// Move to the next message
		conversation.MoveNext();
	}
	#endregion

	#region Private
	// Coroutine for handling the conversation sequence
	IEnumerator Sequence (ConversationData data)
	{
		// Iterate through each message in the conversation data
		for (int i = 0; i < data.list.Count; ++i)
		{
			// Get the speaker data for the current message
			SpeakerData sd = data.list[i];

			// Determine which panel to use based on the anchor of the speaker data
			ConversationPanel currentPanel = (sd.anchor == TextAnchor.UpperLeft || sd.anchor == TextAnchor.MiddleLeft || sd.anchor == TextAnchor.LowerLeft) ? leftPanel : rightPanel;

			// Display the message on the appropriate panel
			IEnumerator presenter = currentPanel.Display(sd);
			presenter.MoveNext();

			// Determine the show and hide positions based on the speaker's anchor
			string show, hide;
			if (sd.anchor == TextAnchor.UpperLeft || sd.anchor == TextAnchor.UpperCenter || sd.anchor == TextAnchor.UpperRight)
			{
				show = ShowTop;
				hide = HideTop;
			}
			else
			{
				show = ShowBottom;
				hide = HideBottom;
			}

			// Set the panel position to hide before showing the message
			currentPanel.panel.SetPosition(hide, false);
			// Move the panel to show the message
			MovePanel(currentPanel, show);

			yield return null;
			// Wait for the message presenter to finish
			while (presenter.MoveNext())
				yield return null;

			// Move the panel to hide the message after the presenter finishes
			MovePanel(currentPanel, hide);

			// Handle transition completion to proceed to the next message
			transition.completedEvent += delegate(object sender, EventArgs e) {
				conversation.MoveNext();
			};

			yield return null;
		}

		// Deactivate the canvas when the conversation ends
		canvas.gameObject.SetActive(false);
		// Trigger the complete event if there are subscribers
		if (completeEvent != null)
			completeEvent(this, EventArgs.Empty);
	}

	// Move the panel to the specified position with animation
	void MovePanel (ConversationPanel obj, string pos)
	{
		transition = obj.panel.SetPosition(pos, true); // Set the panel position
		transition.duration = 0.5f; // Set the duration of the transition animation
		transition.equation = EasingEquations.EaseOutQuad; // Set the easing equation for smooth animation
	}
	#endregion
}
