using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtonHandler : MonoBehaviour
{
    private PauseManager pm;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PauseManager>();
    }

    public void resumeGame()
    {
        pm.togglePause();
    }

    public void returnToMainMenu()
    {
        // Load main menu scene
        // TODO: Make main menu scene
    }
}
