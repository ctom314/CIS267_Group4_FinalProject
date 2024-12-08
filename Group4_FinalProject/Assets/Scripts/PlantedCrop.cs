using UnityEngine;

public class PlantedCrop : MonoBehaviour
{
    public string cropName;
    public Sprite growingSprite;
    public Sprite readySprite;

    private float growthTime;
    private float growthTimer = 0f;
    public bool isGrowing { get; private set; } = true; // Make this public but only modifiable internally
    private SpriteRenderer spriteRenderer;
    private TimeManager timeManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        timeManager = FindObjectOfType<TimeManager>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = growingSprite;
        }

        // Set the growth time based on the crop name
        switch (cropName)
        {
            case "Potato":
                growthTime = (timeManager.dayLengthMinutes * 60) / 2; // Half a day
                break;
            case "Carrot":
                growthTime = timeManager.dayLengthMinutes * 1.25f * 60; // 1.25 days
                break;
            case "Strawberry":
                growthTime = timeManager.dayLengthMinutes * 1.5f * 60; // 1.5 days
                break;
            default:
                growthTime = timeManager.dayLengthMinutes * 60; // Default 1 day
                break;
        }

        Debug.Log($"Crop {cropName} growth time set to {growthTime} seconds.");

        // Disable collider during growth
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private void Update()
    {
        if (isGrowing && timeManager != null && timeManager.isDay)
        {
            growthTimer += Time.deltaTime;

            if (growthTimer >= growthTime)
            {
                CompleteGrowth();
            }
        }
    }

    private void CompleteGrowth()
    {
        isGrowing = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = readySprite;
        }

        gameObject.tag = "Harvestable";

        // Enable collider only when fully grown
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        Debug.Log($"Growth complete. {cropName} is now harvestable. Collider enabled.");
    }

    public void PauseGrowth()
    {
        isGrowing = false;
        Debug.Log($"{cropName} growth paused.");
    }

    public void ResumeGrowth()
    {
        isGrowing = true;
        Debug.Log($"{cropName} growth resumed.");
    }
}