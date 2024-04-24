using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InitBattleState : BattleState 
{
	// Called when the state is entered.
    public override void Enter ()
    {
        base.Enter(); // Ensures that any setup in the BattleState base class is also executed.
        StartCoroutine(Init()); // Starts the initialization coroutine.
    }
	
	// Coroutine to handle the asynchronous tasks needed to set up the battle.
    IEnumerator Init ()
    {
        board.Load(levelData); // Loads the board configuration from level data.
        Point p = new Point((int)levelData.tiles[0].x, (int)levelData.tiles[0].z); // Initializes a point to select the initial tile.
        SelectTile(p); // Selects the initial tile based on the loaded level data.
        SpawnTestUnits(); // Spawns units onto the board for testing or gameplay.
        AddVictoryCondition(); // Sets up the victory conditions for the battle.
        owner.round = owner.gameObject.AddComponent<TurnOrderController>().Round(); // Initializes the round system for turn management.
        yield return null; // Waits one frame before transitioning states, ensuring all setup is complete.
        owner.ChangeState<CutSceneState>(); // Transitions to the CutSceneState for initial battle exposition or setup.
    }
	
// Spawns units on the board based on predefined recipes.
    void SpawnTestUnits ()
    {
        string[] recipes = new string[]
        {
            "Aleph", "Tetrioth", "Samekh", "Bug", "Bug", "Enemy Critter", "Flying Critter"
        };
        
        GameObject unitContainer = new GameObject("Units"); // Creates a new GameObject to contain all units.
        unitContainer.transform.SetParent(owner.transform); // Sets the BattleController as the parent of the unit container.
        
        List<Tile> locations = new List<Tile>(board.tiles.Values); // Creates a list of all possible tile locations.
        for (int i = 0; i < recipes.Length; ++i)
        {
            int level = UnityEngine.Random.Range(9, 12); // Randomly assigns a level to the unit.
            GameObject instance = UnitFactory.Create(recipes[i], level); // Creates a unit based on a recipe and level.
            instance.transform.SetParent(unitContainer.transform); // Sets the unit container as the parent of the unit instance.
            
            int random = UnityEngine.Random.Range(0, locations.Count); // Selects a random location for the unit.
            Tile randomTile = locations[random]; // Gets the random tile.
            locations.RemoveAt(random); // Removes the tile from available locations to prevent overlap.
            
            Unit unit = instance.GetComponent<Unit>();
            unit.Place(randomTile); // Places the unit on the selected tile.
            unit.dir = (Directions)UnityEngine.Random.Range(0, 4); // Randomly assigns a facing direction to the unit.
            unit.Match(); // Updates the unit's position and orientation on the board.
            
            units.Add(unit); // Adds the unit to the list of all units in the battle.
        }
        
        SelectTile(units[0].tile.pos); // Selects the tile of the first unit.
    }
    
    // Adds a victory condition component to the battle.
    void AddVictoryCondition ()
    {
        DefeatTargetVictoryCondition vc = owner.gameObject.AddComponent<DefeatTargetVictoryCondition>(); // Adds a defeat target victory condition.
        Unit enemy = units[units.Count - 1]; // Assumes the last spawned unit is the key enemy.
        vc.target = enemy; // Sets the target of the victory condition to the key enemy.
        Health health = enemy.GetComponent<Health>();
        health.MinHP = 10; // Sets the minimum health for the target, potentially defining a threshold for defeat.
    }
}