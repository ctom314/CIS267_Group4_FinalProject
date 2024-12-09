using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;

    //These will be for our sprint mechanic
    public float sprintMultiplier = 2.0f;
    public float maxStamina = 5.0f;
    public float staminaRecoveryRate = 1.0f;
    public float depletionRecoveryDelay = 2.0f;

    //This will track the player's current stamina, if their sprinting, and the delay
    public float currentStamina;
    private bool isSprinting = false;
    private bool recoveringFromDepletion = false;

    public GameObject currentSprintBar; // Active sprint bar (Greyscale Sprint)
    public PauseManager pm;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 movement;
    public Vector3 curPos;

    // Controller movement
    private PlayerControls controls;
    private Sprite baseCursorSprite;
    public bool controller;

    //For the colors of the Stamina Bar
    public Color depletionColor = new Color(1f, 0.3f, 0.3f);
    public Color normalColor = Color.green;

    //Flash overlay for stamina bar
    public GameObject staminaFlashOverlay;
    public float flashDuration = 0.2f;

    // Default to Level 1
    public int currentStaminaLevel = 1;
    // Allow player to move
    public static bool canMove = true;

    // Animator for handling animations
    private Animator animator;

    //Reference to PlantingManager
    private PlantingManager plantingManager;

    // Declaration of SeedSelectionManager
    private SeedSelectionManager seedSelection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // Animator reference
        animator = GetComponent<Animator>();


        // Initialize the stamina
        currentStamina = maxStamina;

        // Set bar sizes
        UpdateStaminaBars();

        plantingManager = GameObject.Find("GameManager").GetComponent<PlantingManager>();
        Debug.Log("PlantingManager found: " + plantingManager);

        // Initialize SeedSelectionManager
        seedSelection = FindObjectOfType<SeedSelectionManager>();
        if (seedSelection == null)
        {
            Debug.LogError("SeedSelectionManager not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Stop movement if any menu is open
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!PauseManager.isPaused && !PersistentData.instance.isPlayerGameOver())
        {
            if (canMove)
            {
                move();
                updateSprite();
            }

            //cursorHandler();
            UpdateStaminaBars();

            // Handle control scheme changes
            handleControlScheme();
        }
    }

    // ================================================================================ 
    //                                CONTROLLER INPUT 
    // ================================================================================ 
    private void Awake()
    {
        // Get Player Controls
        controls = new PlayerControls();

        // Move the player based on input
        controls.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();

        // Reset movement to zero when input is released
        controls.Player.Move.canceled += ctx => movement = Vector2.zero;

        //Sprint input for controller and keyboard
        controls.Player.Sprint.performed += ctx => StartSprinting();
        controls.Player.Sprint.canceled += ctx => StopSprinting();

        // Planting crops
        controls.Player.Plant.performed += ctx => PlantCrop();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        Debug.Log("Player Controls Enabled");
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        Debug.Log("Player Controls Disabled");
    }

    private void handleControlScheme()
    {
        // Update control scheme based on device type
        InputDevice lastDevice = InputSystem.devices.FirstOrDefault(device => device.lastUpdateTime == InputSystem.devices.Max(d => d.lastUpdateTime));
        controller = lastDevice is Gamepad;
    }

    // ================================================================================ 

    private void move()
    {
        // Stop movement if any menu is open
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Default move speed
        float moveSpeed = speed;

        // Check if the player is moving
        bool isMoving = movement != Vector2.zero;

        // Update animator direction when moving
        if (isMoving)
        {
            animator.SetFloat("LastInputX", movement.x);
            animator.SetFloat("LastInputY", movement.y);
        }

        // Sprint handling
        if (isSprinting && isMoving && currentStamina > 0)
        {
            moveSpeed *= sprintMultiplier;

            // Drain stamina and immediately update UI
            currentStamina = Mathf.Max(0, currentStamina - Time.deltaTime);
            UpdateStaminaBars();

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprinting = false;
                recoveringFromDepletion = true;
                StartCoroutine(RecoverFromDepletion());
            }
        }
        else if (!isSprinting)
        {
            // Recover stamina when not sprinting
            if (recoveringFromDepletion)
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;

                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                    recoveringFromDepletion = false;
                    StartCoroutine(FlashStaminaBar());
                }
            }
            else
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }

            UpdateStaminaBars();
        }

        // Apply movement
        rb.velocity = moveSpeed * movement;

        // Re-enable sprinting if conditions are met
        if (isMoving && !isSprinting && controls.Player.Sprint.IsPressed() && currentStamina > 0 && !recoveringFromDepletion)
        {
            isSprinting = true;
        }
    }

    // Updated animation handling using Blend Tree
    private void updateSprite()
    {
        // Update animator parameters based on movement
        animator.SetBool("isWalking", movement != Vector2.zero); // Walking state
        animator.SetFloat("InputX", movement.x); // Horizontal direction
        animator.SetFloat("InputY", movement.y); // Vertical direction

        // Save the last direction when stopping
        if (movement != Vector2.zero)
        {
            animator.SetFloat("LastInputX", movement.x);
            animator.SetFloat("LastInputY", movement.y);
        }

        // Adjust Animator's speed based on sprinting state
        if (isSprinting)
        {
            animator.speed = 1.5f; // Sprinting animation speed (adjust as needed)
        }
        else
        {
            animator.speed = 1.0f; // Normal animation speed
        }
    }

    private void StartSprinting()
    {
        //Only allow sprinting if stamina is greater than 0, not recovering from depletion, and moving
        if (currentStamina > 0 && !recoveringFromDepletion && movement != Vector2.zero)
        {
            isSprinting = true;
        }
    }

    private void StopSprinting()
    {
        isSprinting = false;
    }

    private IEnumerator RecoverFromDepletion()
    {
        // Wait for depletion recovery delay
        yield return new WaitForSeconds(depletionRecoveryDelay);

        recoveringFromDepletion = true;

        // Use a fixed refill interval to control the refill speed
        float refillInterval = 0.02f; // Adjust this for finer increments
        float refillAmountPerInterval = staminaRecoveryRate * refillInterval;

        // Gradually refill stamina at a steady rate
        while (currentStamina < maxStamina)
        {
            currentStamina += refillAmountPerInterval;

            // Ensure stamina doesn't exceed maxStamina
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }

            // Update UI after every refill step
            UpdateStaminaBars();

            yield return new WaitForSeconds(refillInterval);
        }

        recoveringFromDepletion = false;

        // Trigger the flash effect immediately after full recovery
        StartCoroutine(FlashStaminaBar());
    }

    private void UpdateStaminaBars()
    {
        // Calculate fill amount based on exact current stamina
        float fillAmount = Mathf.Clamp01(currentStamina / maxStamina);

        // Update the current sprint bar's fill amount
        Image sprintBarImage = currentSprintBar.GetComponent<Image>();
        sprintBarImage.fillAmount = fillAmount;

        // Change color based on depletion status
        sprintBarImage.color = recoveringFromDepletion ? depletionColor : normalColor;
    }

    private IEnumerator FlashStaminaBar()
    {
        // Get the Image component of the flash overlay
        Image flashImage = staminaFlashOverlay.GetComponent<Image>();

        // Ensure the overlay starts fully transparent
        Color initialColor = flashImage.color;
        initialColor.a = 0f;
        flashImage.color = initialColor;
        flashImage.gameObject.SetActive(true); // Ensure the flash overlay is visible

        float halfDuration = flashDuration / 2f;

        // Fade in
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / halfDuration);
            Color color = flashImage.color;
            color.a = alpha;
            flashImage.color = color;
            yield return null;
        }

        // Fade out
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / halfDuration);
            Color color = flashImage.color;
            color.a = alpha;
            flashImage.color = color;
            yield return null;
        }

        // Ensure the overlay is fully transparent and disable it
        Color finalColor = flashImage.color;
        finalColor.a = 0f;
        flashImage.color = finalColor;
        flashImage.gameObject.SetActive(false);

        Debug.Log("Flash effect completed, overlay hidden.");
    }

    // Method to update the active sprint bar
    public void UpdateCurrentSprintBar(GameObject newSprintBar)
    {
        currentSprintBar = newSprintBar;
        Debug.Log("Updated to new sprint bar!");
    }

    void PlantCrop()
    {
        if (plantingManager == null)
        {
            Debug.LogError("PlantingManager is not assigned!");
            return;
        }

        // If mouse was used get mouse position, otherwise get player position
        Vector3 position;

        if (!controller)
        {
            // Get the mouse position in world space
            position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            position.z = 0f; // Ensure Z position is zero for 2D
        }
        else
        {
            // Get the player's current position
            position = transform.position;
        }

        // Call the central HandleAction method in PlantingManager
        plantingManager.HandleAction(position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collision detected with {collision.name}, Tag: {collision.tag}");

        // Ignore crops that are not fully grown
        if (collision.CompareTag("Crops"))
        {
            Debug.Log($"Collision ignored. {collision.name} is still growing.");
            return;
        }

        // Check if the player collides with a harvestable crop
        if (collision.CompareTag("Harvestable"))
        {
            PlantedCrop plantedCrop = collision.GetComponent<PlantedCrop>();
            if (plantedCrop != null && !plantedCrop.isGrowing)
            {
                HarvestableCrop harvestableCrop = collision.GetComponent<HarvestableCrop>();
                if (harvestableCrop != null)
                {
                    Debug.Log($"Harvesting crop: {harvestableCrop.cropType}");

                    // Add the harvested crop to the silo
                    PersistentData.instance.AddCropToSilo(harvestableCrop.cropType, 1);

                    // Update the UI
                    GuiManager guiManager = FindObjectOfType<GuiManager>();
                    if (guiManager != null)
                    {
                        guiManager.UpdateHarvestTexts();
                    }

                    // Destroy the crop after harvesting
                    Destroy(collision.gameObject);
                }
                else
                {
                    Debug.LogWarning("No HarvestableCrop script found on collision object.");
                }
            }
            else
            {
                Debug.LogWarning("Crop is not ready for harvesting or is still growing.");
            }
        }
        else
        {
            Debug.LogWarning("Collided object is not tagged as Harvestable.");
        }
    }

    public Vector2 GetFacingDirection()
    {
        // Retrieve the last direction the player was facing
        return new Vector2(
            animator.GetFloat("LastInputX"),
            animator.GetFloat("LastInputY")
        ).normalized;
    }

    public Vector2 GetMovementInput()
    {
        return movement; // Current input vector (updated from controls)
    }

}