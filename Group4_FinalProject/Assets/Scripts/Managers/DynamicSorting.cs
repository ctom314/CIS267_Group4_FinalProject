using UnityEngine;

public class DynamicSorting : MonoBehaviour
{
    private SpriteRenderer playerRenderer;

    // References to other objects with SpriteRenderers
    public SpriteRenderer barnRenderer;
    public SpriteRenderer shopRenderer;

    private void Start()
    {
        // Get the player's SpriteRenderer
        playerRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Determine sorting order relative to the barn
        if (transform.position.y > barnRenderer.transform.position.y)
        {
            // Player is behind the barn
            playerRenderer.sortingOrder = barnRenderer.sortingOrder - 1;
        }
        else if (transform.position.y <= barnRenderer.transform.position.y)
        {
            // Player is in front of the barn
            playerRenderer.sortingOrder = barnRenderer.sortingOrder + 1;
        }

        // Determine sorting order relative to the shop
        if (transform.position.y > shopRenderer.transform.position.y)
        {
            // Player is behind the shop
            playerRenderer.sortingOrder = Mathf.Max(playerRenderer.sortingOrder, shopRenderer.sortingOrder - 1);
        }
        else if (transform.position.y <= shopRenderer.transform.position.y)
        {
            // Player is in front of the shop
            playerRenderer.sortingOrder = Mathf.Max(playerRenderer.sortingOrder, shopRenderer.sortingOrder + 1);
        }
    }
}
