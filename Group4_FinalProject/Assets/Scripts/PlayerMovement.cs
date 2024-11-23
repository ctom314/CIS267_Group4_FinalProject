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

    //This will track the palyer's current stamina, if their sprinting, and the delay
    private float currentStamina;
    private bool isSprinting = false;
    private bool recoveringFromDepletion = false;

    public GameObject greenBar;
    public GameObject redBar;

    public PauseManager pm;

    // Player sprites
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Cursor.visible = false;
        baseCursorSprite = cursor.GetComponent<SpriteRenderer>().sprite;

        //Initialize the stamina
        currentStamina = maxStamina;

        //Set bar sizes
        UpdateStaminaBars();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pm.isPaused && !PersistentData.instance.isPlayerGameOver())
        {
            move();
            updateSprite();
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
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    // ================================================================================

    private void move()
    {
        //Default move speed
        float moveSpeed = speed;

        //Check if the player is moving
        bool isMoving = movement != Vector2.zero;

        //Activate sprinting if conditions are met (button is held and the player starts moving)
        if (isSprinting && isMoving && currentStamina > 0)
        {
            moveSpeed *= sprintMultiplier;
            currentStamina -= Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isSprinting = false;

                //Start recovery phase
                recoveringFromDepletion = true;
                StartCoroutine(RecoverFromDepletion());
            }
        }
        else if (!isSprinting)
        {
            //Recover stamina when not sprinting
            if (recoveringFromDepletion)
            {
                //Recovery phase: refill stamina
                currentStamina += staminaRecoveryRate * Time.deltaTime;

                //If fully refilled, exit recovery phase
                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                    recoveringFromDepletion = false;
                }
            }
            else
            {
                //Regular recovery (non-depleted stamina)
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }
        }

        //Apply movement
        rb.velocity = moveSpeed * movement;

        //Check if sprinting should be re-enabled after starting movement
        if (isMoving && !isSprinting && controls.Player.Sprint.IsPressed() && currentStamina > 0 && !recoveringFromDepletion)
        {
            isSprinting = true;
        }
    }

    //Change sprite based on which direction the player is moving
    private void updateSprite()
    {
        //Threshold for controller stick sensitivity
        float threshold = 0.25f;

        if (movement.y > threshold)
        {
            //Moving up
            sr.sprite = upSprite;
        }
        else if (movement.y < -threshold)
        {
            //Moving down
            sr.sprite = downSprite;
        }
        else if (movement.x > threshold)
        {
            //Moving right
            sr.sprite = rightSprite;
        }
        else if (movement.x < -threshold)
        {
            //Moving left
            sr.sprite = leftSprite;
        }
    }

    private void cursorHandler()
    {

        if(!controller)
        {
            curPos = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10));
        }
        else if(controller)
        {
            controls.Player.MoveCursor.performed += ctx => curPos = ctx.ReadValue<Vector2>();
            controls.Player.MoveCursor.canceled += ctx => curPos = Vector3.zero;
        }
        
        if(controller)
        {
            Vector3 moveTo = new Vector3(curPos.x, curPos.y, 0) * 5f * Time.deltaTime;
            cursor.transform.position += moveTo;
        }
        else
        {
            cursor.transform.position = curPos;
        }

        if(Input.GetMouseButtonDown(0) || controls.Player.rightTrigger.triggered)
        {
            cursor.GetComponent<SpriteRenderer>().sprite = cursorSpriteClicked;
        }
        if(Input.GetMouseButtonUp(0) || controls.Player.rightTrigger.WasReleasedThisFrame())
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
        //Wait for depletion recovery delay
        yield return new WaitForSeconds(depletionRecoveryDelay);

        //Recovery phase: allow stamina to refill
        recoveringFromDepletion = true;

        //Wait until stamina fully recovers
        while (currentStamina < maxStamina)
        {
            yield return null;
        }

        //Transition from red to green (stamina fully recovered)
        recoveringFromDepletion = false;

        //Trigger the flash effect
        StartCoroutine(FlashStaminaBar());
    }


    private void UpdateStaminaBars()
    {
        //Calculate the fill amount based on current stamina
        float fillAmount = Mathf.Clamp01(currentStamina / maxStamina);

        //Get the Image component
        Image greenBarImage = greenBar.GetComponent<Image>();

        //Set the fill amount
        greenBarImage.fillAmount = fillAmount;

        //Change the color based on depletion status
        if (recoveringFromDepletion)
        {
            greenBarImage.color = depletionColor;
        }
        else
        {
            greenBarImage.color = normalColor;
        }
    }
    private IEnumerator FlashStaminaBar()
    {
        //Get the Image component of the flash overlay
        Image flashImage = staminaFlashOverlay.GetComponent<Image>();

        //Set up the fade duration
        float halfDuration = flashDuration / 2f;

        //Fade in
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, t / halfDuration);
            Color color = flashImage.color;
            color.a = alpha;
            flashImage.color = color;
            yield return null;
        }

        //Ensure fully visible at the end of the fade-in
        Color fullyVisibleColor = flashImage.color;
        fullyVisibleColor.a = 1f;
        flashImage.color = fullyVisibleColor;

        //Fade out
        for (float t = 0; t < halfDuration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / halfDuration);
            Color color = flashImage.color;
            color.a = alpha;
            flashImage.color = color;
            yield return null;
        }

        //Ensure fully transparent at the end of the fade-out
        Color fullyTransparentColor = flashImage.color;
        fullyTransparentColor.a = 0f;
        flashImage.color = fullyTransparentColor;
    }
}
