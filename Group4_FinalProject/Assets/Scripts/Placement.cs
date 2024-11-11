using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placement : MonoBehaviour
{
    public GameObject PlacementPrefab;
    private GameObject tempPrefab;
    public bool moving = false;

    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Q) && !moving)
        {
            Color baseColor = PlacementPrefab.GetComponent<SpriteRenderer>().color;
            PlacementPrefab.GetComponent<SpriteRenderer>().color = new Color(23, 176, 23, 0.61f);
            Vector3 trueMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            tempPrefab = Instantiate(PlacementPrefab, trueMousePos, Quaternion.identity);
            moving = true;
        }

        if(moving)
        {
            Vector3 trueMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            tempPrefab.transform.position = trueMousePos;
        }



        if(moving && Input.GetMouseButtonDown(0))
        {
            Debug.Log("PLace!!");
            moving = false;

        }
    }
}
