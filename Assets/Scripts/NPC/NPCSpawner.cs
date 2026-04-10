using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC Prefabs")]
    public GameObject[] npcPrefabs;

    [Header("Spawn Points")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Destroy Limits")]
    public float leftDestroyX;
    public float rightDestroyX;

    [Header("Timing")]
    public float spawnInterval = 5f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnRandomNPC();
            timer = 0f;
        }
    }

    void SpawnRandomNPC()
    {
        if (npcPrefabs.Length == 0) return;

        GameObject selectedPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        bool spawnFromLeft = Random.value > 0.5f;

        Transform spawnPoint = spawnFromLeft ? leftSpawnPoint : rightSpawnPoint;

        GameObject npc = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);

        NPCWalker walker = npc.GetComponent<NPCWalker>();

        if (walker != null)
        {
            if (spawnFromLeft)
            {
                walker.direction = Vector2.right;
                walker.destroyXLimit = rightDestroyX;
            }
            else
            {
                walker.direction = Vector2.left;
                walker.destroyXLimit = leftDestroyX;
            }
        }
    }
}