using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Barra de vida")]
    public GameObject vida100;
    public GameObject vida66;
    public GameObject vida33;
    public GameObject vida0;

    [Header("Empuje")]
    public float knockbackForceX = 8f;
    public float knockbackForceY = 4f;

    [Header("Invulnerabilidad")]
    public float invulnerabilityTime = 0.5f;
    private bool isInvulnerable = false;
    private float invulnerabilityTimer = 0f;

    [Header("Game Over")]
    public string gameOverSceneName = "GameOver";

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        UpdateLifeBar();
    }

    void Update()
    {
        if (isInvulnerable)
        {
            invulnerabilityTimer += Time.deltaTime;

            if (invulnerabilityTimer >= invulnerabilityTime)
            {
                isInvulnerable = false;
                invulnerabilityTimer = 0f;
            }
        }
    }

    public void TakeDamage(int damage, Vector2 hitDirection)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateLifeBar();
        ApplyKnockback(hitDirection);

        isInvulnerable = true;
        invulnerabilityTimer = 0f;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ApplyKnockback(Vector2 hitDirection)
    {
        if (rb == null) return;

        Vector2 knockbackDirection = hitDirection.normalized;

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(
            new Vector2(
                knockbackDirection.x * knockbackForceX,
                knockbackForceY
            ),
            ForceMode2D.Impulse
        );
    }

    void UpdateLifeBar()
    {
        if (vida100 != null) vida100.SetActive(false);
        if (vida66 != null) vida66.SetActive(false);
        if (vida33 != null) vida33.SetActive(false);
        if (vida0 != null) vida0.SetActive(false);

        if (currentHealth >= 3)
        {
            if (vida100 != null) vida100.SetActive(true);
        }
        else if (currentHealth == 2)
        {
            if (vida66 != null) vida66.SetActive(true);
        }
        else if (currentHealth == 1)
        {
            if (vida33 != null) vida33.SetActive(true);
        }
        else
        {
            if (vida0 != null) vida0.SetActive(true);
        }
    }

    void Die()
    {
        Debug.Log("Jugador muerto");

        SceneManager.LoadScene(gameOverSceneName);
    }
}