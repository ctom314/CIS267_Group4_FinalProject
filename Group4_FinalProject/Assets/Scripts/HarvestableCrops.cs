using UnityEngine;

public class HarvestableCrop : MonoBehaviour
{
    public string cropType; // "Potato", "Carrot", "Strawberry"
    public int minYield = 1; // Minimum amount harvested
    public int maxYield = 5; // Maximum amount harvested

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Player collided with {cropType}. Harvesting...");

            // Generate a random harvest yield
            int harvestAmount = Random.Range(minYield, maxYield + 1);

            // Add the harvested crop to the silo
            PersistentData.instance.AddCropToSilo(cropType, harvestAmount);

            // Notify the player
            NotificationManager notificationManager = FindObjectOfType<NotificationManager>();
            if (notificationManager != null)
            {
                notificationManager.ShowNotification($"+ {harvestAmount} {cropType}(s)");
            }

            // Optionally update the UI
            GuiManager guiManager = FindObjectOfType<GuiManager>();
            if (guiManager != null)
            {
                guiManager.UpdateHarvestTexts();
            }

            // Destroy the crop after harvesting
            Destroy(gameObject);
        }
    }
}
