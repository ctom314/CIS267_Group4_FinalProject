using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placement : MonoBehaviour
{
    public GameObject PlacementPrefab;
    public LayerMask baseLayer;

    private GameObject tempPrefab;
    private bool moving = false;
    private Collider2D prefabCollider;
    private Color baseColor;

    void Update() 
    {
        //spawn in the prefab, save the base color, set the color to a transparent green
        if(Input.GetKeyDown(KeyCode.Q) && !moving)
        {
            Vector3 trueMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            tempPrefab = Instantiate(PlacementPrefab, trueMousePos, Quaternion.identity);
            baseColor = tempPrefab.GetComponent<SpriteRenderer>().color;
            prefabCollider = tempPrefab.GetComponent<Collider2D>();
            moving = true;
        }

        //follow the mouse
        if(moving)
        {
            Vector3 trueMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            tempPrefab.transform.position = trueMousePos;
        }

        //place the prefab and set the color back to it's normal color
        if(moving && Input.GetMouseButtonDown(0) && validPlacement())
        {
            moving = false;
            tempPrefab.GetComponent<SpriteRenderer>().color = baseColor;
            tempPrefab.layer = 3;
        }

        //cancel the placement
        if(moving && Input.GetKeyDown(KeyCode.Escape))
        {
            moving = false;
            Destroy(tempPrefab);
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
