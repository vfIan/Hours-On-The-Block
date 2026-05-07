using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;

    [Header("Destroy")]
    public float maxLifeTime = 8f;

    [Header("Damage")]
    public int damage = 1;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    void FixedUpdate()
    {
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            Debug.Log("Jugador golpeado por proyectil enemigo");

            Vector2 hitDirection = collision.transform.position - transform.position;

            playerHealth.TakeDamage(damage, hitDirection);

            Destroy(gameObject);
        }
    }
}