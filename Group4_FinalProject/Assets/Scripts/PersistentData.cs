using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        isGameOver = false;

        // Reset Time/Season Stats
        day = 1;
        seasonId = 1;
        daysThisSeason = 1;

        // Add more here when needed
    }

    // Vars we will probably need to store:
    // - Number of crops planted and what they are.
    // - Season
    // - Days
    // If you can think of anything else, add it here.

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
}
