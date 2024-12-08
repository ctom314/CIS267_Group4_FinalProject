using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SeedSelectionManager : MonoBehaviour
{
    public Image[] seedUIImages; // Assign the Potato, Carrot, Strawberry UI images in order
    private int selectedSeedIndex = 0; // Tracks the currently selected seed (0 = Potato, 1 = Carrot, 2 = Strawberry)

    private PlayerControls controls; // Input Actions reference

    // Start is called before the first frame update
    void Start()
    {
        // Setup initial seed selection
        SelectSeed(0);
    }

    private void Awake()
    {
        // Initialize Player Controls
        controls = new PlayerControls();

        // Hook up input actions for cycling seeds
        controls.Player.CycleSeed.performed += ctx =>
        {
            // Only cycle when not paused
            if (!PauseManager.isPaused)
            {
                // Determine direction (-1 for LB, +1 for RB)
                int direction = ctx.control.name == "leftShoulder" ? -1 : 1;
                CycleSelection(direction);
            }
        };

        // Hook up input actions for direct seed selection
        controls.Player.SelectSeedDirect.performed += ctx =>
        {
            // Check which key was pressed and select the corresponding seed
            switch (ctx.control.name)
            {
                case "1": SelectSeed(0); break; // Key '1'
                case "2": SelectSeed(1); break; // Key '2'
                case "3": SelectSeed(2); break; // Key '3'
            }
        };
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void CycleSelection(int direction)
    {
        selectedSeedIndex += direction;

        // Wrap around the selection
        if (selectedSeedIndex < 0)
            selectedSeedIndex = seedUIImages.Length - 1; // Go to last seed
        else if (selectedSeedIndex >= seedUIImages.Length)
            selectedSeedIndex = 0; // Go to first seed

        UpdateSeedUI();
    }

    private void SelectSeed(int index)
    {
        selectedSeedIndex = index;
        UpdateSeedUI();
    }

    private void UpdateSeedUI()
    {
        for (int i = 0; i < seedUIImages.Length; i++)
        {
            if (i == selectedSeedIndex)
            {
                // Highlight the selected seed
                seedUIImages[i].color = Color.white; // Full brightness
            }
            else
            {
                // Darken unselected seeds
                seedUIImages[i].color = new Color(0.5f, 0.5f, 0.5f, 1f); // Dimmed
            }
        }
    }

    public string GetSelectedSeed()
    {
        // Return the name of the currently selected seed
        switch (selectedSeedIndex)
        {
            case 0: return "Potato";
            case 1: return "Carrot";
            case 2: return "Strawberry";
            default: return null;
        }
    }
}
