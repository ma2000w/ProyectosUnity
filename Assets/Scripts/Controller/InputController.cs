using UnityEngine;
using System;
using System.Collections;

// Repeater class for handling repeated input events
class Repeater
{
	const float threshold = 0.5f; // Threshold for initial input recognition
	const float rate = 0.25f; // Rate at which input is repeated after initial recognition
	float _next; // Time of the next input event
	bool _hold; // Flag indicating whether input is being held down
	string _axis; // Name of the input axis being monitored

	// Constructor to initialize the Repeater with the specified axis name
	public Repeater(string axisName)
	{
		_axis = axisName;
	}

	// Update method to check for input events and return the corresponding value
	public int Update()
	{
		int retValue = 0; // Value to be returned (0 if no input event)
		int value = Mathf.RoundToInt(Input.GetAxisRaw(_axis)); // Get the raw input value from the specified axis

		// Check if input value is non-zero (input is active)
		if (value != 0)
		{
			// Check if enough time has passed since the last input event
			if (Time.time > _next)
			{
				retValue = value; // Set the return value to the input value
				_next = Time.time + (_hold ? rate : threshold); // Set the time for the next input event
				_hold = true; // Set the hold flag to true (indicating input is being held down)
			}
		}
		else
		{
			_hold = false; // Reset the hold flag to false
			_next = 0; // Reset the time for the next input event
		}

		return retValue; // Return the input value (0 if no input event)
	}
}

// InputController class for handling player input
public class InputController : MonoBehaviour 
{
	// Event for move input (directional movement)
	public static event EventHandler<InfoEventArgs<Point>> moveEvent;
	// Event for fire input (firing actions)
	public static event EventHandler<InfoEventArgs<int>> fireEvent;

	Repeater _hor = new Repeater("Horizontal"); // Repeater for horizontal input
	Repeater _ver = new Repeater("Vertical"); // Repeater for vertical input
	string[] _buttons = new string[] { "Fire1", "Fire2", "Fire3" }; // Names of fire buttons

	// Update method called once per frame
	void Update () 
	{
		int x = _hor.Update(); // Get updated horizontal input
		int y = _ver.Update(); // Get updated vertical input

		// Check if there is any movement input
		if (x != 0 || y != 0)
		{
			// Raise the moveEvent with the updated input values
			if (moveEvent != null)
				moveEvent(this, new InfoEventArgs<Point>(new Point(x, y)));
		}

		// Check each fire button for release
		for (int i = 0; i < 3; ++i)
		{
			// Check if the current fire button is released
			if (Input.GetButtonUp(_buttons[i]))
			{
				// Raise the fireEvent with the index of the released fire button
				if (fireEvent != null)
					fireEvent(this, new InfoEventArgs<int>(i));
			}
		}
	}
}
