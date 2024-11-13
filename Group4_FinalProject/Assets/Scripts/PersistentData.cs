using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    // This Script will be attached to a PersistantData Object to store data between scenes.
    // It uses the Singleton pattern to ensure only one instance of this class exists.
    // On launch, it will move itself to a "DontDestroyOnLoad" scene.
    // Using the "PersistantData.instance" property, other scripts can access the data stored in this class.

    public static PersistentData instance { get; private set; }

    // Vars to store between scenes
    public int day = 1;

    // Vars we will probably need to store:
    // - Number of crops planted and what they are.
    // - Season
    // - Days
    // If you can think of anything else, add it here.

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

    // Methods to set and get data.

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


    // Data to reset when the game is restarted/game over
    public void ResetData()
    {
        day = 1;

        // Add more here when needed
    }
}
