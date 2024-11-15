using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject squareEnemyPrefab;
    public GameObject diamondEnemyPrefab;
    public GameObject circleEnemyPrefab;
    public GameObject triangleEnemyPrefab;

    public float spawnCooldown = 5f;
    private Transform player;
    public Transform[] spawnPoints;  // Array of enemy spawn locations

    private int enemiesToSpawn; // Number of enemies to spawn in this wave
    private bool isSpawning;
    public bool HasFinishedSpawning => enemiesToSpawn <= 0 && !isSpawning;

    // New modifiers for challenges
    private float enemyHealthModifier = .05f;      // Multiplier for enemy health
    public float challengeHealth = 1;
    public float enemySpeedModifier = 1f;       // Multiplier for enemy speed
    public float circleSpawnChance = 0.15f;     // Default spawn chance for circle enemies
    public float wave = 1;
    
    public float defaultCircleSpawnChance = 0.15f;  // Base chance for spawning circle enemies

    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        defaultCircleSpawnChance = circleSpawnChance; // Set default value for resetting later
    }

    // New StartSpawning method to begin spawning for a specific wave
    public void StartSpawning(int enemyCount)
    {
        if (isSpawning) StopCoroutine(SpawnEnemyRoutine()); // Ensure no overlapping coroutines
        enemiesToSpawn = enemyCount;
        isSpawning = true;
        StartCoroutine(SpawnEnemyRoutine());
    }

    // Stops the current spawning routine
    public void StopSpawning()
    {
        isSpawning = false;
        StopCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (isSpawning && enemiesToSpawn > 0)
        {
            if (ShouldSpawnEnemy())
            {
                GameObject enemyToSpawn = ChooseEnemy();
                Transform spawnLocation = ChooseSpawnLocation();

                if (enemyToSpawn != null && spawnLocation != null)
                {
                    GameObject enemy = Instantiate(enemyToSpawn, spawnLocation.position, Quaternion.identity);
                    ApplyModifiersToEnemy(enemy);
                    
                    
                    
                    enemiesToSpawn--;

                    // Notify WaveManager on enemy defeat
                    enemy.GetComponent<Enemy>().OnDefeated += FindObjectOfType<WaveManager>().EnemyDefeated;
                }
            }
            yield return new WaitForSeconds(spawnCooldown);
        }

        isSpawning = false;
    }

    private bool ShouldSpawnEnemy()
    {
        float spawnChance = IsClosestSpawnPoint() ? 0.55f : 0.5f;
        return Random.value < spawnChance;
    }

    private GameObject ChooseEnemy()
    {
        float randomValue = Random.value;

        // Use circleSpawnChance if circle enemy is chosen
        if (randomValue < circleSpawnChance)
            return circleEnemyPrefab;
        if (randomValue < 0.5f)
            return squareEnemyPrefab;
        if (randomValue < 0.7f)
            return triangleEnemyPrefab;
        return diamondEnemyPrefab;
    }

    private Transform ChooseSpawnLocation()
    {
        Transform closestSpawnPoint = GetClosestSpawnLocation();

        if (Random.value < 0.55f)
        {
            return closestSpawnPoint;
        }
        else
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
    }

    private Transform GetClosestSpawnLocation()
    {
        Transform closestSpawnPoint = spawnPoints[0];
        float closestDistance = Vector2.Distance(player.position, spawnPoints[0].position);

        foreach (Transform spawnPoint in spawnPoints)
        {
            float distance = Vector2.Distance(player.position, spawnPoint.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSpawnPoint = spawnPoint;
            }
        }

        return closestSpawnPoint;
    }

    private bool IsClosestSpawnPoint()
    {
        Transform closestSpawnPoint = GetClosestSpawnLocation();
        return Vector2.Distance(player.position, closestSpawnPoint.position) < 10f; // Arbitrary range threshold
    }

    // Applies the current modifiers to spawned enemies
    private void ApplyModifiersToEnemy(GameObject enemy)
    {
        var enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.maxHealth *= ((enemyHealthModifier*(wave-1))+1) * challengeHealth;
            Debug.LogAssertion(enemyComponent.maxHealth);
            enemyComponent.speed *= enemySpeedModifier;
        }
    }
}
