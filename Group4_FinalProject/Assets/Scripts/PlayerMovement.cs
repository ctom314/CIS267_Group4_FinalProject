using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Cursor.visible = false;
        baseCursorSprite = cursor.GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pm.isPaused)
        {
            move();
            updateSprite();
            cursorHandler();
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
        // Prevent faster diagonal movement
        //movement.Normalize();

        // Move the player
        rb.velocity = speed * movement;
    }

    // Change sprite based on which direction the player is moving
    private void updateSprite()
    {
        // Threshold for controller stick sensitivity
        float threshold = 0.25f;

        if (movement.y > threshold)
        {
            // Moving up
            sr.sprite = upSprite;
        }
        else if (movement.y < -threshold)
        {
            // Moving down
            sr.sprite = downSprite;
        }
        else if (movement.x > threshold)
        {
            // Moving right
            sr.sprite = rightSprite;
        }
        else if (movement.x < -threshold)
        {
            // Moving left
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
}
