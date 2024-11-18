using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPlacerScript : MonoBehaviour
{
    public Placement placer;
    public GameObject barn;

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            placer.PlacementPrefab = barn;
            placer.startPlacement = true;
        }

        if(Input.GetKeyDown(KeyCode.I) && !placer.startPlacement)
        {
            Debug.Log("Open store menu");
        }
    }
}
