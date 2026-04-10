using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrowController : MonoBehaviour
{
    [Header("Pickup")]
    public Transform pickupCheck;
    public float pickupRadius = 0.7f;
    public LayerMask throwableLayer;
    public Transform holdPoint;

    [Header("Throw")]
    public float maxChargeTime = 1.5f;
    public float minThrowForce = 3f;
    public float maxThrowForce = 15f;

    private ThrowableObject heldObject;
    private Camera cam;
    private bool isCharging = false;
    private float currentChargeTime = 0f;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandlePickupOrDrop();
        HandleAiming();
        HandleThrowCharge();
    }

    void HandlePickupOrDrop()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Si ya lleva algo, lo suelta
            if (heldObject != null)
            {
                CancelCharge();
                heldObject.Drop();
                heldObject = null;
                return;
            }

            // Si no lleva nada, intenta coger el objeto más cercano
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                pickupCheck.position,
                pickupRadius,
                throwableLayer
            );

            Collider2D closest = null;
            float minDist = Mathf.Infinity;

            foreach (Collider2D hit in hits)
            {
                float dist = Vector2.Distance(pickupCheck.position, hit.transform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    closest = hit;
                }
            }

            if (closest != null)
            {
                ThrowableObject obj = closest.GetComponent<ThrowableObject>();

                if (obj != null)
                {
                    heldObject = obj;
                    heldObject.PickUp(holdPoint);
                }
            }
        }
    }

    void HandleAiming()
    {
        if (heldObject == null) return;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // Girar el personaje según el ratón
        if (mouseWorldPos.x < transform.position.x)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void HandleThrowCharge()
    {
        if (heldObject == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isCharging = true;
            currentChargeTime = 0f;
        }

        if (isCharging && Mouse.current.leftButton.isPressed)
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, maxChargeTime);
        }

        if (isCharging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0f;

            Vector2 direction = (mouseWorldPos - holdPoint.position).normalized;

            float chargePercent = currentChargeTime / maxChargeTime;
            float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargePercent);

            heldObject.Throw(direction * throwForce);
            heldObject = null;

            CancelCharge();
        }
    }

    void CancelCharge()
    {
        isCharging = false;
        currentChargeTime = 0f;
    }

    void OnDrawGizmosSelected()
    {
        if (pickupCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pickupCheck.position, pickupRadius);
        }
    }
}