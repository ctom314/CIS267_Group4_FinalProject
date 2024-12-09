using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PitchforkController : MonoBehaviour
{
    public GameObject pitchfork; // Assign the pitchfork sprite here
    public float jabDuration = 3f; // How long the pitchfork is visible
    public float cooldownDuration = 2f; // Cooldown time between jabs

    // Adjustable offsets for each direction
    [Header("Offsets for Pitchfork Position")]
    public Vector2 offsetUp = new Vector2(0, 0.5f);
    public Vector2 offsetDown = new Vector2(0, -0.5f);
    public Vector2 offsetLeft = new Vector2(-0.5f, 0);
    public Vector2 offsetRight = new Vector2(0.5f, 0);

    private bool canJab = true; // Tracks whether the player can jab
    private Vector2 facingDirection; // Direction the player is facing
    private PlayerMovement playerMovement; // Reference to the PlayerMovement script

    private PlayerControls controls; // Input actions

    private void Awake()
    {
        // Initialize input actions
        controls = new PlayerControls();

        // Bind the Weapon action to the TryJab method
        controls.Player.Weapon.performed += ctx => TryJab();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();

        if (pitchfork != null)
        {
            pitchfork.SetActive(false); // Ensure the pitchfork is initially hidden
        }
    }

    private void Update()
    {
        // Get the player's facing direction from PlayerMovement
        facingDirection = new Vector2(
            playerMovement.GetFacingDirection().x,
            playerMovement.GetFacingDirection().y
        ).normalized;

        // Handle right-click input for jab (in case you want to keep mouse support)
        if (canJab && Input.GetMouseButtonDown(1))
        {
            TryJab();
        }
    }

    private void TryJab()
    {
        if (canJab)
        {
            Debug.Log("Weapon Action Triggered!");
            StartCoroutine(PerformJab());
        }
    }

    private IEnumerator PerformJab()
    {
        canJab = false; // Disable further jabs during the cooldown
        pitchfork.SetActive(true);

        // Position and rotate the pitchfork
        UpdatePitchforkTransform();

        // Check for enemies in range
        Collider2D[] enemiesHit = Physics2D.OverlapBoxAll(
            pitchfork.transform.position,
            new Vector2(1, 1), // Adjust size for detection
            pitchfork.transform.eulerAngles.z,
            LayerMask.GetMask("Enemy")
        );

        foreach (Collider2D enemy in enemiesHit)
        {
            Destroy(enemy.gameObject);
        }

        // Wait for the jab duration
        yield return new WaitForSeconds(jabDuration);

        pitchfork.SetActive(false); // Hide the pitchfork

        // Wait for cooldown
        yield return new WaitForSeconds(cooldownDuration - jabDuration);

        canJab = true; // Allow jabs again
    }

    private void UpdatePitchforkTransform()
    {
        // Get player's movement input and last input directions
        Vector2 currentMovement = playerMovement.GetMovementInput(); // Retrieve raw input
        Animator animator = playerMovement.GetComponent<Animator>();
        float lastInputX = animator.GetFloat("LastInputX");
        float lastInputY = animator.GetFloat("LastInputY");

        Vector3 offset = Vector3.zero;
        float rotation = 0f;

        // Determine direction based on movement or fallback to last facing direction
        if (currentMovement != Vector2.zero) // If the player is moving
        {
            // Use current movement direction
            if (currentMovement.x > 0.1f) // Moving right
            {
                offset = offsetRight;
                rotation = 0f;
            }
            else if (currentMovement.x < -0.1f) // Moving left
            {
                offset = offsetLeft;
                rotation = 180f;
            }
            else if (currentMovement.y > 0.1f) // Moving up
            {
                offset = offsetUp;
                rotation = 90f;
            }
            else if (currentMovement.y < -0.1f) // Moving down
            {
                offset = offsetDown;
                rotation = -90f;
            }
        }
        else // Use last facing direction
        {
            if (lastInputX > 0.1f) // Facing right
            {
                offset = offsetRight;
                rotation = 0f;
            }
            else if (lastInputX < -0.1f) // Facing left
            {
                offset = offsetLeft;
                rotation = 180f;
            }
            else if (lastInputY > 0.1f) // Facing up
            {
                offset = offsetUp;
                rotation = 90f;
            }
            else if (lastInputY < -0.1f) // Facing down
            {
                offset = offsetDown;
                rotation = -90f;
            }
        }

        // Apply position and rotation
        pitchfork.transform.localPosition = offset;
        pitchfork.transform.localRotation = Quaternion.Euler(0, 0, rotation - 45); // Adjust for pitchfork sprite's default rotation
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the pitchfork hitbox in the editor
        if (pitchfork != null && pitchfork.activeSelf)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                pitchfork.transform.position,
                new Vector3(1, 1, 0) // Adjust size for visualization
            );
        }
    }
}
