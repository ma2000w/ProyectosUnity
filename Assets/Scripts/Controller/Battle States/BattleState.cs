using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Abstract base class for managing battle-related states.
public abstract class BattleState : State 
{
    // References to battle controller components.
    protected BattleController owner;
    protected Driver driver;
    // Properties to access various components through the battle controller.
    public CameraRig cameraRig { get { return owner.cameraRig; }}
    public Board board { get { return owner.board; }}
    public LevelData levelData { get { return owner.levelData; }}
    public Transform tileSelectionIndicator { get { return owner.tileSelectionIndicator; }}
    public Point pos { get { return owner.pos; } set { owner.pos = value; }}
    public Tile currentTile { get { return owner.currentTile; }}
    public AbilityMenuPanelController abilityMenuPanelController { get { return owner.abilityMenuPanelController; }}
    public StatPanelController statPanelController { get { return owner.statPanelController; }}
    public HitSuccessIndicator hitSuccessIndicator { get { return owner.hitSuccessIndicator; }}
    public Turn turn { get { return owner.turn; }}
    public List<Unit> units { get { return owner.units; }}

    // Initialize the battle state by linking to the BattleController component.
    protected virtual void Awake ()
    {
        owner = GetComponent<BattleController>();
    }

    // Subscribe to input events when the battle state becomes active.
    protected override void AddListeners ()
    {
        if (driver == null || driver.Current == Drivers.Human)
        {
            InputController.moveEvent += OnMove;
            InputController.fireEvent += OnFire;
        }
    }

    // Unsubscribe from input events when the state is no longer active.
    protected override void RemoveListeners ()
    {
        InputController.moveEvent -= OnMove;
        InputController.fireEvent -= OnFire;
    }

    // Enter the state, setting up any necessary state components.
    public override void Enter ()
    {
        driver = (turn.actor != null) ? turn.actor.GetComponent<Driver>() : null;
        base.Enter ();
    }

    // Abstract methods for handling movement and fire events.
    protected virtual void OnMove (object sender, InfoEventArgs<Point> e)
    {
    }
    
    protected virtual void OnFire (object sender, InfoEventArgs<int> e)
    {
    }

    // Selects a tile on the board, updating the position and indicator.
    protected virtual void SelectTile (Point p)
    {
        if (pos == p || !board.tiles.ContainsKey(p))
            return;

        pos = p;
        tileSelectionIndicator.localPosition = board.tiles[p].center;
    }

    // Retrieves a Unit at a specified point.
    protected virtual Unit GetUnit (Point p)
    {
        Tile t = board.GetTile(p);
        GameObject content = t != null ? t.content : null;
        return content != null ? content.GetComponent<Unit>() : null;
    }

    // Updates the primary stat panel for a unit.
    protected virtual void RefreshPrimaryStatPanel (Point p)
    {
        Unit target = GetUnit(p);
        if (target != null)
            statPanelController.ShowPrimary(target.gameObject);
        else
            statPanelController.HidePrimary();
    }

    // Updates the secondary stat panel for a unit.
    protected virtual void RefreshSecondaryStatPanel (Point p)
    {
        Unit target = GetUnit(p);
        if (target != null)
            statPanelController.ShowSecondary(target.gameObject);
        else
            statPanelController.HideSecondary();
    }

    // Checks if the player has won the battle.
    protected virtual bool DidPlayerWin ()
    {
        return owner.GetComponent<BaseVictoryCondition>().Victor == Alliances.Hero;
    }
    
    // Determines if the battle is over.
    protected virtual bool IsBattleOver ()
    {
        return owner.GetComponent<BaseVictoryCondition>().Victor != Alliances.None;
    }
}
