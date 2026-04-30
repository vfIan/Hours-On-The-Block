using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Timing")]
    public float spawnInterval = 10f;
    public bool spawnOnStart = true;

    [Header("Control")]
    public int maxEnemiesAlive = 1;

    private float timer;
    private int enemiesAlive = 0;

    void Start()
    {
        timer = 0f;

        if (spawnOnStart)
            SpawnEnemy();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawner: falta enemyPrefab.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: no hay spawn points.");
            return;
        }

        if (enemiesAlive >= maxEnemiesAlive)
            return;

        Transform selectedSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (selectedSpawn == null)
            return;

        GameObject enemy = Instantiate(enemyPrefab, selectedSpawn.position, Quaternion.identity);

        EnemyShooter enemyShooter = enemy.GetComponent<EnemyShooter>();

        if (enemyShooter != null)
        {
            enemyShooter.OnEnemyDestroyed += HandleEnemyDestroyed;
        }

        enemiesAlive++;
    }

    void HandleEnemyDestroyed()
    {
        enemiesAlive--;

        if (enemiesAlive < 0)
            enemiesAlive = 0;
    }
}