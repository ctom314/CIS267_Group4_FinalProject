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

    // Sprites
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private PauseManager pm;
    private HealthManager hm;

    private AIPath aIPath;
    private AIDestinationSetter destSetter;
    private SpriteRenderer sr;

    

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

        // Get child sprite renderer
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseManager.isPaused)
        {
            // Get movement
            Vector2 movement = aIPath.desiredVelocity;

            // Update sprite
            updateSprite(movement);
        }
    }

    // Change sprite based on which direction the enemy is moving
    private void updateSprite(Vector2 movement)
    {
        float threshold = 0.5f;

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

    
}
