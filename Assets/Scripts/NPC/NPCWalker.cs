using UnityEngine;

public class NPCWalker : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public Vector2 direction = Vector2.right;
    public float destroyXLimit = 0f;

    [Header("Drop")]
    public GameObject assignedTrashPrefab;
    public Transform dropPoint;
    public float minDropTime = 1f;
    public float maxDropTime = 4f;

    private bool hasDropped = false;
    private float dropTime;
    private float timer = 0f;

    void Start()
    {
        dropTime = Random.Range(minDropTime, maxDropTime);

        // Girar visualmente según dirección
        Vector3 scale = transform.localScale;
        scale.x = direction.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void Update()
    {
        Move();
        HandleDrop();
        CheckDestroy();
    }

    void Move()
    {
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    void HandleDrop()
    {
        if (hasDropped) return;

        timer += Time.deltaTime;

        if (timer >= dropTime)
        {
            DropTrash();
            hasDropped = true;
        }
    }

    void DropTrash()
    {
        if (assignedTrashPrefab == null || dropPoint == null) return;

        Instantiate(assignedTrashPrefab, dropPoint.position, Quaternion.identity);
    }

    void CheckDestroy()
    {
        if (direction.x > 0 && transform.position.x > destroyXLimit)
        {
            Destroy(gameObject);
        }
        else if (direction.x < 0 && transform.position.x < destroyXLimit)
        {
            Destroy(gameObject);
        }
    }
}