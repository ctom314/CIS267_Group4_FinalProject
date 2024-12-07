using UnityEngine;

public class PlantedCrop : MonoBehaviour
{
    public Crop cropData; // Reference to the Crop data model
    private float growthTimer = 0f; // Tracks real-time growth progress
    private bool isGrowing = true; // Whether the crop is actively growing
    private TimeManager timeManager; // Reference to TimeManager

    private SpriteRenderer spriteRenderer; // SpriteRenderer for visual updates
    public Sprite growingSprite; // Sprite during the growing phase
    public Sprite fullyGrownSprite; // Sprite when fully grown

    private void Start()
    {
        // Reference TimeManager
        timeManager = FindObjectOfType<TimeManager>();

        if (timeManager == null)
        {
            Debug.LogError("TimeManager not found!");
            return;
        }

        // Initialize SpriteRenderer and set the growing sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = growingSprite;
        }
    }

    private void Update()
    {
        if (isGrowing && timeManager != null && timeManager.isDay)
        {
            // Increment growth timer
            growthTimer += Time.deltaTime;

            // Convert crop growth days to seconds
            float growTimeInSeconds = cropData.GetNumDaysToGrow() * 7 * 60;

            // Check if growth is complete
            if (growthTimer >= growTimeInSeconds)
            {
                GrowthComplete();
            }
        }
    }

    private void GrowthComplete()
    {
        isGrowing = false; // Stop growing

        // Update sprite to fully grown
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = fullyGrownSprite;
        }

        Debug.Log($"{cropData.GetName()} has fully grown!");
    }

    public void PauseGrowth()
    {
        isGrowing = false;
    }

    public void ResumeGrowth()
    {
        if (timeManager != null && timeManager.isDay)
        {
            isGrowing = true;
        }
    }
}
