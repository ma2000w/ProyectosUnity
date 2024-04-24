using UnityEngine;
using System.IO;
using System.Collections;

public static class UnitFactory
{
    #region Public Methods

    // Create a unit based on its name and level
    public static GameObject Create (string name, int level)
    {
        // Load the unit recipe from resources
        UnitRecipe recipe = Resources.Load<UnitRecipe>("Unit Recipes/" + name);

        // Check if the recipe is found
        if (recipe == null)
        {
            Debug.LogError("No Unit Recipe for name: " + name);
            return null;
        }

        // Create the unit using the loaded recipe and specified level
        return Create(recipe, level);
    }

    // Create a unit based on a unit recipe and level
    public static GameObject Create (UnitRecipe recipe, int level)
    {
        // Instantiate the prefab for the unit
        GameObject obj = InstantiatePrefab("Units/" + recipe.model);

        // Set the name of the unit
        obj.name = recipe.name;

        // Add necessary components to the unit
        obj.AddComponent<Unit>();
        AddStats(obj);
        AddLocomotion(obj, recipe.locomotion);
        obj.AddComponent<Status>();
        obj.AddComponent<Equipment>();
        AddJob(obj, recipe.job);
        AddRank(obj, level);
        obj.AddComponent<Health>();
        obj.AddComponent<Mana>();
        AddAttack(obj, recipe.attack);
        AddAbilityCatalog(obj, recipe.abilityCatalog);
        AddAlliance(obj, recipe.alliance);
        AddAttackPattern(obj, recipe.strategy);

        return obj;
    }

    #endregion

    #region Private Methods

    // Instantiate a prefab from resources
    static GameObject InstantiatePrefab (string name)
    {
        GameObject prefab = Resources.Load<GameObject>(name);
        if (prefab == null)
        {
            Debug.LogError("No Prefab for name: " + name);
            return new GameObject(name);
        }
        GameObject instance = GameObject.Instantiate(prefab);
        instance.name = instance.name.Replace("(Clone)", "");
        return instance;
    }

    // Add default stats to the unit
    static void AddStats (GameObject obj)
    {
        Stats s = obj.AddComponent<Stats>();
        s.SetValue(StatTypes.LVL, 1, false);
    }

    // Add a job to the unit
    static void AddJob (GameObject obj, string name)
    {
        GameObject instance = InstantiatePrefab("Jobs/" + name);
        instance.transform.SetParent(obj.transform);
        Job job = instance.GetComponent<Job>();
        job.Employ();
        job.LoadDefaultStats();
    }

    // Add locomotion type to the unit
    static void AddLocomotion (GameObject obj, Locomotions type)
    {
        switch (type)
        {
            case Locomotions.Walk:
                obj.AddComponent<WalkMovement>();
                break;
            case Locomotions.Fly:
                obj.AddComponent<FlyMovement>();
                break;
            case Locomotions.Teleport:
                obj.AddComponent<TeleportMovement>();
                break;
        }
    }

    // Add alliance to the unit
    static void AddAlliance (GameObject obj, Alliances type)
    {
        Alliance alliance = obj.AddComponent<Alliance>();
        alliance.type = type;
    }

    // Add rank to the unit
    static void AddRank (GameObject obj, int level)
    {
        Rank rank = obj.AddComponent<Rank>();
        rank.Init(level);
    }

    // Add attack ability to the unit
    static void AddAttack (GameObject obj, string name)
    {
        GameObject instance = InstantiatePrefab("Abilities/" + name);
        instance.transform.SetParent(obj.transform);
    }

    // Add ability catalog to the unit
    static void AddAbilityCatalog (GameObject obj, string name)
    {
        GameObject main = new GameObject("Ability Catalog");
        main.transform.SetParent(obj.transform);
        main.AddComponent<AbilityCatalog>();

        AbilityCatalogRecipe recipe = Resources.Load<AbilityCatalogRecipe>("Ability Catalog Recipes/" + name);
        if (recipe == null)
        {
            Debug.LogError("No Ability Catalog Recipe Found: " + name);
            return;
        }

        for (int i = 0; i < recipe.categories.Length; ++i)
        {
            GameObject category = new GameObject( recipe.categories[i].name );
            category.transform.SetParent(main.transform);

            for (int j = 0; j < recipe.categories[i].entries.Length; ++j)
            {
                string abilityName = string.Format("Abilities/{0}/{1}", recipe.categories[i].name, recipe.categories[i].entries[j]);
                GameObject ability = InstantiatePrefab(abilityName);
                ability.transform.SetParent(category.transform);
            }
        }
    }

    // Add attack pattern to the unit
    static void AddAttackPattern (GameObject obj, string name)
    {
        Driver driver = obj.AddComponent<Driver>();
        if (string.IsNullOrEmpty(name))
        {
            driver.normal = Drivers.Human;
        }
        else
