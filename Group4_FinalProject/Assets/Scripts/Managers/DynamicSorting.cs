using UnityEngine;

public class DynamicSorting : MonoBehaviour
{
    private SpriteRenderer playerRenderer;

    // References to other objects with SpriteRenderers
    public SpriteRenderer barnRenderer;
    public SpriteRenderer shopRenderer;
    public SpriteRenderer siloRenderer;

    private void Start()
    {
        // Get the player's SpriteRenderer
        playerRenderer = GetComponent<SpriteRenderer>();

        if (playerRenderer == null)
        {
            Debug.LogError("Player SpriteRenderer is missing!");
        }
    }

    private void Update()
    {
        if (playerRenderer == null) return;

        // Determine which object is closest in depth
        SpriteRenderer closestRenderer = null;
        float closestYDifference = float.MaxValue;

        if (barnRenderer != null)
        {
            float yDifference = Mathf.Abs(transform.position.y - barnRenderer.transform.position.y);
            if (yDifference < closestYDifference)
            {
                closestYDifference = yDifference;
                closestRenderer = barnRenderer;
            }
        }

        if (shopRenderer != null)
        {
            float yDifference = Mathf.Abs(transform.position.y - shopRenderer.transform.position.y);
            if (yDifference < closestYDifference)
            {
                closestYDifference = yDifference;
                closestRenderer = shopRenderer;
            }
        }

        if (siloRenderer != null)
        {
            float yDifference = Mathf.Abs(transform.position.y - siloRenderer.transform.position.y);
            if (yDifference < closestYDifference)
            {
                closestYDifference = yDifference;
                closestRenderer = siloRenderer;
            }
        }

        // Apply sorting order based on the closest object
        if (closestRenderer != null)
        {
            if (transform.position.y > closestRenderer.transform.position.y)
            {
                playerRenderer.sortingOrder = closestRenderer.sortingOrder - 1; // Behind
            }
            else
            {
                playerRenderer.sortingOrder = closestRenderer.sortingOrder + 1; // In front
            }
        }
    }
}
