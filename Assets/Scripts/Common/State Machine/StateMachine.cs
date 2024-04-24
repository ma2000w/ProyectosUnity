using UnityEngine;
using System.Collections;

// StateMachine class inherits from MonoBehaviour, allowing it to be attached to GameObjects.
public class StateMachine : MonoBehaviour 
{
    // Public property to get or set the current state of the state machine.
    public virtual State CurrentState
    {
        get { return _currentState; }
        set { Transition(value); } // Setting a new state triggers the Transition method.
    }
    
    // Private field to hold the current active state.
    protected State _currentState;
    
    // Flag to prevent state transitions from happening while already transitioning.
    protected bool _inTransition;

    // Generic method to get or add a state component to the GameObject this script is attached to.
    public virtual T GetState<T>() where T : State
    {
        T target = GetComponent<T>(); // Try to get the state component already attached to the GameObject.
        if (target == null)
            target = gameObject.AddComponent<T>(); // If not present, add it dynamically.
        return target;
    }
    
    // Generic method to change the state machine's state to the specified type.
    public virtual void ChangeState<T>() where T : State
    {
        CurrentState = GetState<T>(); // Sets the CurrentState to the specified state type.
    }

    // Handles transitioning between states.
    protected virtual void Transition(State value)
    {
        if (_currentState == value || _inTransition)
            return; // Return if trying to transition to the same state or already transitioning.

        _inTransition = true; // Mark as transitioning.

        if (_currentState != null)
            _currentState.Exit(); // Call exit method on the current state, if any.

        _currentState = value; // Update the current state.

        if (_currentState != null)
            _currentState.Enter(); // Call enter method on the new state.

        _inTransition = false; // End transitioning.
    }
}
