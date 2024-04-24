using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// CutSceneState extends the BattleState, indicating it is part of the battle system's state machine.
public class CutSceneState : BattleState 
{
    ConversationController conversationController; // Controller for managing conversations.
    ConversationData data; // Data object containing the conversation script.

    // Awake is called when the script instance is being loaded.
    protected override void Awake ()
    {
        base.Awake();
        // Retrieves the ConversationController component from children of the owner (BattleController).
        conversationController = owner.GetComponentInChildren<ConversationController>();
    }

    // Enter is called when the state becomes active.
    public override void Enter ()
    {
        base.Enter();
        if (IsBattleOver())
        {
            // Loads the appropriate conversation data based on whether the player won or lost the battle.
            if (DidPlayerWin())
                data = Resources.Load<ConversationData>("Conversations/OutroSceneWin");
            else
                data = Resources.Load<ConversationData>("Conversations/OutroSceneLose");
        }
        else
        {
            // Loads introductory conversation data if the battle is not over.
            data = Resources.Load<ConversationData>("Conversations/IntroScene");
        }
        // Displays the conversation using the conversation controller.
        conversationController.Show(data);
    }

    // Exit is called when the state is exited.
    public override void Exit ()
    {
        base.Exit();
        // Unloads the conversation data from memory to free up resources.
        if (data)
            Resources.UnloadAsset(data);
    }

    // Adds event listeners when the state is entered.
    protected override void AddListeners ()
    {
        base.AddListeners();
        // Subscribes to the completion event of the conversation.
        ConversationController.completeEvent += OnCompleteConversation;
    }

    // Removes event listeners when the state is exited.
    protected override void RemoveListeners ()
    {
        base.RemoveListeners();
        // Unsubscribes from the completion event of the conversation.
        ConversationController.completeEvent -= OnCompleteConversation;
    }

    // Responds to the 'fire' event, typically used to advance the conversation.
    protected override void OnFire (object sender, InfoEventArgs<int> e)
    {
        base.OnFire(sender, e);
        // Advances to the next part of the conversation.
        conversationController.Next();
    }

    // Called when a conversation is completed.
    void OnCompleteConversation (object sender, System.EventArgs e)
    {
        // Checks if the battle is over and changes the state accordingly.
        if (IsBattleOver())
            owner.ChangeState<EndBattleState>(); // Changes to the state indicating the end of the battle.
        else
            owner.ChangeState<SelectUnitState>(); // Returns to the state where a unit is selected for action.
    }
}
