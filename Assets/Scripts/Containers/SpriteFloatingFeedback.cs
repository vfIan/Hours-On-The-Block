using UnityEngine;

public class SpriteFloatingFeedback : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 1.5f;

    [Header("Desaparición")]
    public float lifeTime = 1f;

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private Color originalColor;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        // Subir
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Contador de vida
        timer += Time.deltaTime;

        // Porcentaje de vida: 0 al principio, 1 al final
        float progress = timer / lifeTime;

        // Reducir opacidad
        if (spriteRenderer != null)
        {
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, progress);
            spriteRenderer.color = newColor;
        }

        // Destruir al terminar
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}