using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class BattleMessageController : MonoBehaviour 
{
	// Reference to the UI text component for displaying the battle message.
	[SerializeField] Text label;
	// Reference to the canvas GameObject containing the text component.
	[SerializeField] GameObject canvas;
	// Reference to the canvas group component for controlling transparency.
	[SerializeField] CanvasGroup group;
	// EasingControl component for handling alpha animation.
	EasingControl ec;

	// Called when the script instance is being loaded.
	void Awake ()
	{
		// Add an EasingControl component for managing the animation.
		ec = gameObject.AddComponent<EasingControl>();
		ec.duration = 0.5f; // Set the duration of the animation.
		ec.equation = EasingEquations.EaseInOutQuad; // Set the easing equation for smooth animation.
		ec.endBehaviour = EasingControl.EndBehaviour.Constant; // Set the behavior when the animation ends.
		ec.updateEvent += OnUpdateEvent; // Subscribe to the update event of the EasingControl.
	}

	// Displays the battle message.
	public void Display (string message)
	{
		group.alpha = 0; // Set the initial alpha value to 0 to make the canvas invisible.
		canvas.SetActive(true); // Activate the canvas GameObject.
		label.text = message; // Set the text of the label to the provided message.
		StartCoroutine(Sequence()); // Start the message display sequence.
	}

	// Event handler for updating the alpha value of the canvas group.
	void OnUpdateEvent (object sender, EventArgs e)
	{
		group.alpha = ec.currentValue; // Update the alpha value based on the current value of the EasingControl.
	}

	// Controls the sequence of displaying and hiding the message.
	IEnumerator Sequence ()
	{
		ec.Play(); // Start the animation.

		// Wait until the animation finishes.
		while (ec.IsPlaying)
			yield return null;

		yield return new WaitForSeconds(1); // Wait for 1 second.

		ec.Reverse(); // Reverse the animation to hide the message.

		// Wait until the animation finishes.
		while (ec.IsPlaying)
			yield return null;

		canvas.SetActive(false); // Deactivate the canvas 
