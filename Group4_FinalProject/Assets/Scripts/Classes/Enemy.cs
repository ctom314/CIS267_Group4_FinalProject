using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    // Enemy Stats
    public int health = 10;
    public int maxHealth = 10;
    public int damage = 1;

    private PauseManager pm;
    private HealthManager hm;

    private AIPath aIPath;
    private AIDestinationSetter destSetter;
    private Animator animator; // Reference to Animator

    // TEMP
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        pm = GameObject.Find("GameManager").GetComponent<PauseManager>();
        hm = pm.GetComponent<HealthManager>();

        player = GameObject.FindGameObjectWithTag("Player");

        aIPath = GetComponent<AIPath>();
        destSetter = GetComponent<AIDestinationSetter>();

        // Set the enemy to track the player
        destSetter.target = player.transform;

        // Get the Animator component
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!PauseManager.isPaused)
        {
            // Get the enemy's movement direction
            Vector2 movement = aIPath.desiredVelocity;

            // Update the Animator parameters
            UpdateAnimationParameters(movement);
        }
    }

    private void UpdateAnimationParameters(Vector2 movement)
    {
        if (movement.magnitude > 0.1f) // Check if the enemy is moving
        {
            // Normalize the movement vector for consistent direction
            Vector2 normalizedMovement = movement.normalized;

            // Update Animator parameters
            animator.SetFloat("MoveX", normalizedMovement.x);
            animator.SetFloat("MoveY", normalizedMovement.y);
            animator.SetBool("isMoving", true); // Set isMoving to true
        }
        else
        {
            // Enemy is not moving
            animator.SetBool("isMoving", false); // Set isMoving to false
        }

        // Debugging logs
        Debug.Log($"Updating Animator: MoveX = {movement.x}, MoveY = {movement.y}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If enemy collides with player, decrease player HP
        if (collision.gameObject.CompareTag("Player"))
        {
            // Allow damage when not in immunity frames
            if (hm.getDealDamage())
            {
                PersistentData.instance.decrementPlayerHP();

                // Update health display
                hm.updateLivesDisplay();

                // Start immunity frames
                StartCoroutine(hm.immunityFrames());
            }
        }
    }

    public void TakeDamage()
    {
        Destroy(gameObject); // Destroy the enemy object
    }
}
