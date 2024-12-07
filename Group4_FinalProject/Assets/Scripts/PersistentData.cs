using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    // This Script will be attached to a PersistantData Object to store data between scenes.
    // It uses the Singleton pattern to ensure only one instance of this class exists.
    // On launch, it will move itself to a "DontDestroyOnLoad" scene.
    // Using the "PersistantData.instance" property, other scripts can access the data stored in this class.

    public static PersistentData instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of this class exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSeedInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Data to reset when the game is restarted/game over
    public void ResetData()
    {
        // Reset Player Stats
        cropData.Clear();
        playerData.SetMaxHealth(3);
        playerData.SetHealth(playerData.GetMaxHealth());
        playerMoney = PLAYER_STARTING_MONEY;
        isGameOver = false;

        // Reset Time/Season Stats
        day = 1;
        seasonId = 1;
        daysThisSeason = 1;

        // Add more here when needed
    }

    // ====================================================================================
    //                                      PLAYER DATA
    // ====================================================================================

    // Store player data
    private PlayerStats playerData = new PlayerStats(PlayerStats.DEFAULT_HEALTH, PlayerStats.DEFAULT_HEALTH);
    private bool isGameOver = false;

    // Store crop info
    private Dictionary<string, Crop> cropData = new Dictionary<string, Crop>();

    // Manage Player Data
    // Get HP
    public int getPlayerHP()
    {
        return playerData.GetHealth();
    }

    // Get Max HP
    public int getPlayerMaxHP()
    {
        return playerData.GetMaxHealth();
    }

    // Increment HP
    public void incrementPlayerHP()
    {
        if (playerData.GetHealth() < playerData.GetMaxHealth())
        {
            playerData.SetHealth(playerData.GetHealth() + 1);
        }
    }

    // Check for Game Over
    public bool isPlayerGameOver()
    {
        return playerData.GetHealth() <= 0;
    }

    // Decrement HP
    public void decrementPlayerHP()
    {
        playerData.SetHealth(playerData.GetHealth() - 1);

        // Check if player is dead
        if (isPlayerGameOver())
        {
            isGameOver = true;
        }
    }


    // Manage Crop Data
    // Add
    public void addCropData(string cropName, Crop crop)
    {
        cropData.Add(cropName, crop);
    }

    // Get
    public Crop getCropData(string cropName)
    {
        // Get crop data if it exists, else null
        Crop crop = cropData.ContainsKey(cropName) ? cropData[cropName] : null;
        return crop;
    }

    // Remove
    public void removeCropData(string cropName)
    {
        cropData.Remove(cropName);
    }

    // ====================================================================================
    //                                      TIME DATA
    // ====================================================================================

    // Vars to store between scenes
    public int day = 1;

    // Day Get/Set
    public int getDay()
    {
        return day;
    }

    public void setDay(int day)
    {
        this.day = day;
    }

    public void incrementDay()
    {
        day++;
    }

    // ====================================================================================
    //                                      SEASON DATA
    // ====================================================================================

    // Current season as an ID
    // 1 = Spring, 2 = Summer, 3 = Autumn, 4 = Winter
    public int seasonId = 1;

    // Number of days that have passed this season
    public int daysThisSeason = 1;

    // The names of each season
    public Dictionary<int, string> seasonNames = new Dictionary<int, string>
    {
        { 1, "Spring" },
        { 2, "Summer" },
        { 3, "Autumn" },
        { 4, "Winter" }
    };

    // The lengths of each season
    // 1 = Spring, 2 = Summer, 3 = Autumn, 4 = Winter
    public Dictionary<int, int> seasonLengths = new Dictionary<int, int>
    {
        { 1, 7 },
        { 2, 7 },
        { 3, 7 },
        { 4, 7 }
    };

    public int getSeasonId()
    {
        return seasonId;
    }

    // Update season if enough time has passed
    public void incrementSeason()
    {
        daysThisSeason = 1;

        if (seasonId == 4)
        {
            seasonId = 1;
        }
        else
        {
            seasonId++;
        }

        loadSeasonMap(seasonId);
    }

    // Load season map
    public void loadSeasonMap(int id)
    {
        string sceneName = seasonNames[id] + "Map";
        SceneManager.LoadScene(sceneName);
    }

    // Get length of a season
    public int getSeasonLength(int seasonId)
    {
        return seasonLengths[seasonId];
    }

    // Get name of current season
    public string getSeasonName()
    {
        // Get name of season using value
        return seasonNames[seasonId];
    }

    public int getSeasonDay()
    {
        return daysThisSeason;
    }

    public void incrementSeasonDay()
    {
        daysThisSeason++;
    }

    // ====================================================================================
    //                                      MONEY DATA
    // ====================================================================================

    // Starting money amount
    public static readonly int PLAYER_STARTING_MONEY = 1000; // Starting money amount (set default as needed)

    private int playerMoney; 

    // Get the player's current money
    public int GetMoney()
    {
        return playerMoney;
    }

    // Set the player's money
    public void SetMoney(int amount)
    {
        playerMoney = Mathf.Max(0, amount); // Ensure money does not go below 0
    }

    // Add money to the player's total
    public void AddMoney(int amount)
    {
        playerMoney += amount;
    }

    // Subtract money from the player's total
    public void SubtractMoney(int amount)
    {
        playerMoney = Mathf.Max(0, playerMoney - amount); // Ensure money does not go below 0
    }

    // ====================================================================================
    //                                      SEASON TRANSITION
    // ====================================================================================

    // Transition to the next season
    public void TransitionToNextSeason()
    {
        // Reset health
        playerData.SetHealth(playerData.GetMaxHealth());

        // Check if the season has ended
        if (daysThisSeason >= seasonLengths[seasonId])
        {
            // Increment the season
            incrementSeason();
        }

        // Fade to black and load the next scene
        StartCoroutine(LoadNextSeasonScene());
    }

    private IEnumerator LoadNextSeasonScene()
    {
        Debug.Log("Starting fade-out...");
        yield return StartCoroutine(FadeManager.instance.FadeOut(2f));

        Debug.Log("Loading next scene...");
        switch (seasonId)
        {
            case 1:
                SceneManager.LoadScene("SpringMap");
                break;
            case 2:
                SceneManager.LoadScene("SummerMap");
                break;
            case 3:
                SceneManager.LoadScene("FallMap");
                break;
            case 4:
                SceneManager.LoadScene("WinterMap");
                break;
            default:
                Debug.LogError("Invalid season ID!");
                break;
        }

        yield return new WaitForSeconds(1f);

        Debug.Log("Fading back in...");
        yield return StartCoroutine(FadeManager.instance.FadeIn(2f));
    }

    // ====================================================================================
    //                                      SEED DATA
    // ====================================================================================

    private Dictionary<string, int> seedInventory = new Dictionary<string, int>();

    // Initialize seed inventory with 0 seeds for each type
    private void InitializeSeedInventory()
    {
        seedInventory["Potato"] = 0;
        seedInventory["Carrot"] = 0;
        seedInventory["Strawberry"] = 0;
        // Add more crops as needed
    }

    // Add seeds to the player's inventory
    public void AddSeeds(string seedName, int quantity)
    {
        if (seedInventory.ContainsKey(seedName))
        {
            seedInventory[seedName] += quantity;
            Debug.Log($"Added {quantity} {seedName} seeds. Total: {seedInventory[seedName]}");
        }
        else
        {
            Debug.LogError($"Seed type '{seedName}' does not exist in inventory.");
        }
    }

    // Get the number of seeds of a specific type
    public int GetSeedCount(string seedName)
    {
        if (seedInventory.ContainsKey(seedName))
        {
            return seedInventory[seedName];
        }
        else
        {
            Debug.LogError($"Seed type '{seedName}' does not exist in inventory.");
            return 0;
        }
    }

    // Use seeds from the inventory
    // Returns true if successful, false if not enough seeds
    public bool UseSeed(string seedName)
    {
        if (seedInventory.ContainsKey(seedName))
        {
            if (seedInventory[seedName] > 0)
            {
                seedInventory[seedName]--;
                Debug.Log($"Used 1 {seedName} seed. Remaining: {seedInventory[seedName]}");
                return true;
            }
            else
            {
                Debug.LogWarning($"No {seedName} seeds available to use.");
                return false;
            }
        }
        else
        {
            Debug.LogError($"Seed type '{seedName}' does not exist in inventory.");
            return false;
        }
    }

    private Dictionary<string, int> siloContents = new Dictionary<string, int>
{
    { "Potato", 0 },
    { "Carrot", 0 },
    { "Strawberry", 0 }
};

    public void AddCropToSilo(string cropType, int amount)
    {
        if (siloContents.ContainsKey(cropType))
        {
            siloContents[cropType] += amount;
        }
        else
        {
            Debug.LogWarning($"Invalid crop type: {cropType}");
        }
    }

    public int GetCropCount(string cropType)
    {
        return siloContents.ContainsKey(cropType) ? siloContents[cropType] : 0;
    }

}