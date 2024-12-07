using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlantingManager : MonoBehaviour
{
    public GameObject growingCropPrefab; // Prefab for growing crop
    public GameObject fullyGrownPotatoPrefab; // Fully grown potato prefab
    public GameObject fullyGrownCarrotPrefab; // Fully grown carrot prefab
    public GameObject fullyGrownStrawberryPrefab; // Fully grown strawberry prefab
    private PlayerControls controls; // Reference to PlayerControls
    private Camera mainCamera; // Reference to the main camera for mouse position
    private SeedSelectionManager seedSelection;

    private bool canPlant = true; // Prevent double planting

    private void Awake()
    {
        controls = new PlayerControls();
        mainCamera = Camera.main; // Get the main camera
        seedSelection = FindObjectOfType<SeedSelectionManager>();
    }

    private void OnEnable()
    {
        controls.Player.LeftClick.performed += OnLeftClick;
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.LeftClick.performed -= OnLeftClick;
        controls.Player.Disable();
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        if (!canPlant) return; // Prevent double planting
        canPlant = false; // Block further planting until reset

        // Get mouse position in world space
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0f; // Ensure Z position is zero for 2D

        // Attempt to plant the crop
        PlantCrop(mousePosition);

        // Re-enable planting after a short delay
        Invoke(nameof(ResetPlanting), 0.1f);
    }

    private void ResetPlanting()
    {
        canPlant = true;
    }

    private void PlantCrop(Vector3 position)
    {
        // Check if planting is possible
        Collider2D plantingSpot = Physics2D.OverlapPoint(position);

        if (plantingSpot != null && plantingSpot.CompareTag("PlantingSpot"))
        {
            // Ensure no overlapping crops
            Collider2D[] collidersAtSpot = Physics2D.OverlapPointAll(position);

            foreach (Collider2D collider in collidersAtSpot)
            {
                Debug.Log($"Collider Tag: {collider.tag}");
                if (collider.CompareTag("Crops"))
                {
                    Debug.Log("A crop is already planted here!");
                    return;
                }
            }

            // Get the selected seed
            string selectedSeed = seedSelection.GetSelectedSeed();

            // Check if the player has enough seeds
            if (PersistentData.instance.UseSeed(selectedSeed))
            {
                // Instantiate the growing crop prefab
                GameObject growingCrop = Instantiate(growingCropPrefab, position, Quaternion.identity);

                // Start the growth process
                StartCoroutine(GrowCrop(growingCrop, selectedSeed));
                Debug.Log($"Planted a {selectedSeed} crop at {position}!");
            }
            else
            {
                Debug.Log($"Not enough {selectedSeed} seeds to plant!");
            }
        }
        else
        {
            Debug.Log("Invalid planting location!");
        }
    }

    private IEnumerator GrowCrop(GameObject growingCrop, string seedType)
    {
        // Determine growth duration based on crop type
        float growTimeInSeconds = GetGrowthDays(seedType) * 7 * 60; // Convert days to seconds
        yield return new WaitForSeconds(growTimeInSeconds);

        // Determine the fully grown prefab based on the seed type
        GameObject fullyGrownPrefab = null;
        switch (seedType)
        {
            case "Potato":
                fullyGrownPrefab = fullyGrownPotatoPrefab;
                break;
            case "Carrot":
                fullyGrownPrefab = fullyGrownCarrotPrefab;
                break;
            case "Strawberry":
                fullyGrownPrefab = fullyGrownStrawberryPrefab;
                break;
        }

        if (fullyGrownPrefab != null)
        {
            // Replace the growing crop with the fully grown prefab
            Vector3 position = growingCrop.transform.position;
            Destroy(growingCrop); // Remove the growing crop
            Instantiate(fullyGrownPrefab, position, Quaternion.identity);
            Debug.Log($"Crop grown: {seedType}");
        }
    }

    private int GetGrowthDays(string cropName)
    {
        switch (cropName)
        {
            case "Potato": return 1; // Half a day
            case "Carrot": return 2; // Day and a quarter
            case "Strawberry": return 3; // Day and a half
            default: return 1; // Default to 1 day
        }
    }

    public bool TryPlantCrop(string selectedSeed, Vector3 position)
    {
        float minimumDistance = 5f; // Adjust this value for desired spacing

        // Check if planting is possible
        Collider2D plantingSpot = Physics2D.OverlapPoint(position);

        if (plantingSpot != null && plantingSpot.CompareTag("PlantingSpot"))
        {
            // Ensure no overlapping crops and check for nearby crops
            Collider2D[] collidersAtSpot = Physics2D.OverlapCircleAll(position, minimumDistance);

            foreach (Collider2D collider in collidersAtSpot)
            {
                Debug.Log($"Detected collider: {collider.name} with tag {collider.tag}");
                if (collider.CompareTag("Crops"))
                {
                    Debug.Log("Too close to another crop!");
                    return false; // Prevent planting too close to another crop
                }
            }

            // Check if the player has enough seeds
            if (PersistentData.instance.UseSeed(selectedSeed))
            {
                // Instantiate the growing crop prefab
                GameObject crop = Instantiate(growingCropPrefab, position, Quaternion.identity);
                crop.tag = "Crops";
                Debug.Log($"Tag assigned to crop: {crop.tag}");
                return true;
            }
            else
            {
                Debug.Log($"Not enough {selectedSeed} seeds to plant!");
            }
        }
        else
        {
            Debug.Log("Invalid planting location!");
        }

        return false; // Planting failed
    }
}

