using UnityEngine;

public class HarvestableCrop : MonoBehaviour
{
    public string cropType; // "Potato", "Carrot", or "Strawberry"

    private void OnMouseDown()
    {
        // Check if the crop is harvestable
        if (!string.IsNullOrEmpty(cropType))
        {
            // Add the harvested crop to the silo (PersistentData)
            PersistentData.instance.AddCropToSilo(cropType, 1);
            Debug.Log($"Harvested 1 {cropType}!");

            // Destroy the crop after harvesting
            Destroy(gameObject);
        }
    }
}
