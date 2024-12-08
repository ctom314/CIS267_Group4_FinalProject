using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public List<GameObject> spawners;
    public List<GameObject> enemies;

    // Time between spawns
    public float spawnTime = 10f;

    private PauseManager pm;
    private TimeManager tm;

    // Start is called before the first frame update
    void Start()
    {
        pm = GameObject.Find("GameManager").GetComponent<PauseManager>();
        tm = pm.GetComponent<TimeManager>();

        StartCoroutine(spawnTimer());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject chooseEnemy()
    {
        // Choose random enemy from list
        return enemies[Random.Range(0, enemies.Count)];
    }

    private void spawnEnemy()
    {
        // Pick a random spawner from the list
        GameObject spawner = spawners[Random.Range(0, spawners.Count)];

        // Get bounds of spawner using its collider
        BoxCollider2D spawnerCol = spawner.GetComponent<BoxCollider2D>();
        Bounds bounds = spawnerCol.bounds;

        // Spawn enemy at random position inside spawner bounds
        float randX = Random.Range(bounds.min.x, bounds.max.x);
        float randY = Random.Range(bounds.min.y, bounds.max.y);
        Vector2 spawnPos = new Vector2(randX, randY);

        // Spawn a random enemy
        GameObject enemy = chooseEnemy();
        Instantiate(enemy, spawnPos, Quaternion.identity);
    }

    private IEnumerator spawnTimer()
    {

        // Only spawn enemies at night, when it's not paused
        while (!PauseManager.isPaused)
        {
            yield return new WaitForSeconds(spawnTime);

            if (!tm.isDay)
            {
                // Spawn enemy
                spawnEnemy();
            }
        }
    }
}
