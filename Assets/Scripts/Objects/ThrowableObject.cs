using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    [HideInInspector] public bool isHeld = false;

    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void PickUp(Transform holdPoint)
    {
        if (rb == null || holdPoint == null) return;

        isHeld = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (col != null)
            col.enabled = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        if (rb == null) return;

        isHeld = false;

        transform.SetParent(null);
        rb.bodyType = RigidbodyType2D.Dynamic;

        if (col != null)
            col.enabled = true;
    }

    public void Throw(Vector2 force)
    {
        if (rb == null) return;

        isHeld = false;

        transform.SetParent(null);
        rb.bodyType = RigidbodyType2D.Dynamic;

        if (col != null)
            col.enabled = true;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}