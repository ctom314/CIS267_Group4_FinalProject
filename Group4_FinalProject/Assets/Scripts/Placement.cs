//===============================================================================
// Author: Isaac Shields
//
// Desc. This script handles placing objects
//
// How to use: To call from a separate script, set "placementPrefab" to
//             whatever object you want to place. After that set the bool
//             "startPlacement" to true.
//===============================================================================



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placement : MonoBehaviour
{

    public GameObject PlacementPrefab;
    public bool moving = false;
    public LayerMask baseLayer;
    private GameObject tempPrefab;
    private Color baseColor;
    private Collider2D prefabCollider;
    public bool startPlacement;
    private PlayerControls controls;
    private void Start() 
    {
        controls = new PlayerControls();
        controls.Enable();
    }

    private void Update()
    {
        //spawn in the prefab, save the base color, set the color to a transparent green
        if(startPlacement && !moving)
        {
            // Get the cursor position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            tempPrefab = Instantiate(PlacementPrefab, mousePos, Quaternion.identity);
            baseColor = tempPrefab.GetComponent<SpriteRenderer>().color;
            prefabCollider = tempPrefab.GetComponent<Collider2D>();
            moving = true;
        }

        //follow the mouse
        if(moving)
        {
            // Get the cursor position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            tempPrefab.transform.position = mousePos;
        }

        //place the prefab and set the color back to it's normal color
        if(moving && Input.GetMouseButtonDown(0) && validPlacement() || controls.Player.rightTrigger.triggered && moving && validPlacement())
        {
            moving = false;
            tempPrefab.GetComponent<SpriteRenderer>().color = baseColor;
            tempPrefab.layer = 3;
            startPlacement = false;
        }

        //cancel the placement
        if(moving && Input.GetKeyDown(KeyCode.Escape) || controls.Player.Pause.triggered)
        {
            moving = false;
            Destroy(tempPrefab);
            startPlacement = false;
        }

        //change color from green to red if it's in an invalid position
        if(moving && !validPlacement())
        {
            //set color to red
            tempPrefab.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0.55f);
        }
        else if(moving && validPlacement())
        {
            //set color to green
            tempPrefab.GetComponent<SpriteRenderer>().color = new Color(0, 143, 0, 0.5f);
        }
    }


    private bool validPlacement()
    {
        //check if the item is colliding with another item
        bool vp = false;
        if(prefabCollider is BoxCollider2D bxc)
        {
            Collider2D tester = Physics2D.OverlapBox(tempPrefab.transform.position, new Vector2(bxc.transform.lossyScale.x, bxc.transform.lossyScale.y), 0, baseLayer);
            if(tester == null)
            {
                vp = true;
            }
        }
        else if(prefabCollider is CircleCollider2D cc)
        {
            Collider2D tester = Physics2D.OverlapCircle(tempPrefab.transform.position, cc.transform.lossyScale.x/2, baseLayer);
            if(tester == null)
            {
                vp = true;
            }
        }

        return vp;
    }
}
