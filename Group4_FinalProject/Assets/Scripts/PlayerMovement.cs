using System.Collections;
using System.Collections.Generic;
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
    public GameObject cursor;
    public Sprite cursorSpriteClicked;
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // Animator reference
        animator = GetComponent<Animator>();

        //Cursor.visible = false;
        baseCursorSprite = cursor.GetComponent<SpriteRenderer>().sprite;

        //Initialize the stamina
        currentStamina = maxStamina;

        //Set bar sizes
        UpdateStaminaBars();
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

        if (!pm.isPaused && !PersistentData.instance.isPlayerGameOver())
        {
            if (canMove)
            {
                move();
                updateSprite();
            }

            cursorHandler();
            UpdateStaminaBars();
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

        // Activate sprinting if conditions are met (button is held and the player starts moving)
        if (isSprinting && isMoving && currentStamina > 0)
        {
            moveSpeed *= sprintMultiplier;

            // Drain stamina and immediately update UI
            currentStamina = Mathf.Max(0, currentStamina - Time.deltaTime);
            UpdateStaminaBars(); // Update UI immediately after draining

            // Stop sprinting if stamina depletes
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprinting = false;

                // Start recovery phase
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

                // If fully refilled, exit recovery phase
                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                    recoveringFromDepletion = false;

                    // Trigger flash effect only after full recovery
                    StartCoroutine(FlashStaminaBar());
                }
            }
            else
            {
                // Regular recovery (non-depleted stamina)
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }

            UpdateStaminaBars(); // Update UI immediately during recovery
        }

        // Apply movement
        rb.velocity = moveSpeed * movement;

        // Check if sprinting should be re-enabled after starting movement
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

    private void cursorHandler()
    {
        if (!controller)
        {
            curPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        }
        else if (controller)
        {
            controls.Player.MoveCursor.performed += ctx => curPos = ctx.ReadValue<Vector2>();
            controls.Player.MoveCursor.canceled += ctx => curPos = Vector3.zero;
        }

        if (controller)
        {
            Vector3 moveTo = new Vector3(curPos.x, curPos.y, 0) * 5f * Time.deltaTime;
            cursor.transform.position += moveTo;
        }
        else
        {
            cursor.transform.position = curPos;
        }

        if (Input.GetMouseButtonDown(0) || controls.Player.rightTrigger.triggered)
        {
            cursor.GetComponent<SpriteRenderer>().sprite = cursorSpriteClicked;
        }
        if (Input.GetMouseButtonUp(0) || controls.Player.rightTrigger.WasReleasedThisFrame())
        {
            cursor.GetComponent<SpriteRenderer>().sprite = baseCursorSprite;
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
}

