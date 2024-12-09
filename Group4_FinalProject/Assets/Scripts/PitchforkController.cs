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

    public static bool canJab = true; // Tracks whether the player can jab
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

        float elapsedTime = 0f;

        while (elapsedTime < jabDuration)
        {
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

            // Next frame
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        pitchfork.SetActive(false); // Hide the pitchfork

        // Wait for cooldown
        yield return new WaitForSeconds(cooldownDuration - jabDuration);

        canJab = true; // Allow jabs again
    }

    private int playerFacingDirection(Sprite s)
    {
        // 0: Up
        // 1: Down
        // 2: Left
        // 3: Right
        // -1: Unknown

        if (s.name.Contains("Up"))
        {
            return 0;
        }
        else if (s.name.Contains("Down"))
        {
            return 1;
        }
        else if (s.name.Contains("Left"))
        {
            return 2;
        }
        else if (s.name.Contains("Right"))
        {
            return 3;
        }
        else
        {
            return -1;
        }
    }

    private void UpdatePitchforkTransform()
    {
        // Get player's sprite and which direction its facing
        SpriteRenderer playerSprite = playerMovement.GetComponent<SpriteRenderer>();
        int playerDir = playerFacingDirection(playerSprite.sprite);

        Vector3 offset = Vector3.zero;
        float rotation = 0f;

        // Get rotation and offset based on player's facing direction
        if (playerDir != -1)
        {
            if (playerDir == 0)
            {
                // Up
                offset = offsetUp;
                rotation = 90f;
            }
            else if (playerDir == 1)
            {
                // Down
                offset = offsetDown;
                rotation = -90f;
            }
            else if (playerDir == 2)
            {
                // Left
                offset = offsetLeft;
                rotation = 180f;
            }
            else if (playerDir == 3)
            {
                // Right
                offset = offsetRight;
                rotation = 0f;
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
