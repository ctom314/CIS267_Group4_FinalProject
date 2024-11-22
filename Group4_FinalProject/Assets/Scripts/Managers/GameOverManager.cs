using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Check if player is dead
        if (PersistentData.instance.isPlayerGameOver())
        {
            // Game over
            // TODO: Display game over screen

            // TEMP: Freeze Game
            Debug.Log("Game Over");
            Time.timeScale = 0;
        }
    }
}
