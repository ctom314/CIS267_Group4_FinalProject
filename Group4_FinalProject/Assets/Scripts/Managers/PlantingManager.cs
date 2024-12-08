using UnityEngine;

public class PlantingManager : MonoBehaviour
{
    public GameObject potatoPrefab;
    public GameObject carrotPrefab;
    public GameObject strawberryPrefab;

    private const float minimumDistance = 0.5f; // Minimum distance between crops

    // Handles the planting logic
    public void HandleAction(Vector3 position)
    {
        Debug.Log($"HandleAction called at position: {position}");

        // Try to plant
        if (TryPlant(position))
        {
            Debug.Log("Planting successful!");
        }
        else
        {
            Debug.Log("Failed to plant!");
        }
    }

    // Attempt to plant a crop at the given position
    public bool TryPlant(Vector3 position)
    {
        Collider2D plantingSpot = Physics2D.OverlapPoint(position);
        if (plantingSpot == null || !plantingSpot.CompareTag("PlantingSpot"))
        {
            Debug.Log("Invalid planting location!");
            return false; // Planting failed
        }

        // Check for nearby crops
        Collider2D[] nearbyCrops = Physics2D.OverlapCircleAll(position, minimumDistance);
        foreach (Collider2D collider in nearbyCrops)
        {
            if (collider.CompareTag("Crops") || collider.CompareTag("Harvestable"))
            {
                Debug.Log("A crop or harvestable item is already planted here!");
                return false; // Planting failed
            }
        }

        // Get the selected seed
        string selectedSeed = FindObjectOfType<SeedSelectionManager>().GetSelectedSeed();
        if (!PersistentData.instance.UseSeed(selectedSeed))
        {
            Debug.Log($"Not enough {selectedSeed} seeds!");
            return false; // Planting failed
        }

        // Determine the crop prefab
        GameObject cropPrefab = null;
        switch (selectedSeed)
        {
            case "Potato": cropPrefab = potatoPrefab; break;
            case "Carrot": cropPrefab = carrotPrefab; break;
            case "Strawberry": cropPrefab = strawberryPrefab; break;
        }

        if (cropPrefab == null)
        {
            Debug.Log("Invalid seed selected!");
            return false; // Planting failed
        }

        // Plant the crop
        GameObject crop = Instantiate(cropPrefab, position, Quaternion.identity);
        crop.tag = "Crops"; // Default tag for crops
        Debug.Log($"Planted {selectedSeed} at {position}!");
        return true; // Planting succeeded
    }
}
