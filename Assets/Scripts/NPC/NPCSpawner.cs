using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC Prefabs")]
    public GameObject[] npcPrefabs;

    [Header("Spawn Points")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Destroy Limits")]
    public float leftDestroyX = -20f;
    public float rightDestroyX = 20f;

    [Header("Timing")]
    public float spawnInterval = 5f;
    public bool spawnOnStart = true;

    private float timer = 0f;

    void Start()
    {
        if (spawnOnStart)
            SpawnRandomNPC();
    }

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
        // Validaciones
        if (npcPrefabs == null || npcPrefabs.Length == 0)
        {
            Debug.LogWarning("NPCSpawner: No hay prefabs asignados");
            return;
        }

        if (leftSpawnPoint == null || rightSpawnPoint == null)
        {
            Debug.LogWarning("NPCSpawner: Falta asignar spawn points");
            return;
        }

        // Elegir prefab aleatorio
        GameObject selectedPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

        // Elegir lado
        bool spawnFromLeft = Random.value > 0.5f;
        Transform spawnPoint = spawnFromLeft ? leftSpawnPoint : rightSpawnPoint;

        // Instanciar
        GameObject npc = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);

        // Configurar movimiento
        NPCWalker walker = npc.GetComponent<NPCWalker>();

        if (walker != null)
        {
            walker.direction = spawnFromLeft ? Vector2.right : Vector2.left;
            walker.destroyXLimit = spawnFromLeft ? rightDestroyX : leftDestroyX;
        }
        else
        {
            Debug.LogWarning("NPCSpawner: El NPC no tiene NPCWalker");
        }
    }

    // 🔧 Para testear desde botón en inspector
    [ContextMenu("Spawn NPC")]
    void SpawnDebug()
    {
        SpawnRandomNPC();
    }

    // 👀 Para ver límites en escena
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (leftSpawnPoint != null)
            Gizmos.DrawSphere(leftSpawnPoint.position, 0.3f);

        if (rightSpawnPoint != null)
            Gizmos.DrawSphere(rightSpawnPoint.position, 0.3f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftDestroyX, -100, 0), new Vector3(leftDestroyX, 100, 0));
        Gizmos.DrawLine(new Vector3(rightDestroyX, -100, 0), new Vector3(rightDestroyX, 100, 0));
    }
}