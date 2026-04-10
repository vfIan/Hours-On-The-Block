using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrowController : MonoBehaviour
{
    [Header("Pickup")]
    public Transform pickupCheck;
    public float pickupRadius = 0.7f;
    public LayerMask throwableLayer;
    public Transform holdPoint;

    [Header("Aiming")]
    public Transform bodyVisual;   // cuerpo_0
    public Transform armPivot;     // brazoPivot
    public float minArmAngle = -80f;
    public float maxArmAngle = 80f;

    [Header("Throw")]
    public float maxChargeTime = 1.5f;
    public float minThrowForce = 3f;
    public float maxThrowForce = 15f;

    private ThrowableObject heldObject;
    private Camera cam;
    private bool isCharging = false;
    private float currentChargeTime = 0f;

    private Vector3 bodyOriginalScale;
    private Vector3 armOriginalScale;

    void Start()
    {
        cam = Camera.main;

        if (bodyVisual == null)
            bodyVisual = transform;

        if (armPivot != null)
            armOriginalScale = armPivot.localScale;

        bodyOriginalScale = bodyVisual.localScale;
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
            if (heldObject != null)
            {
                CancelCharge();
                heldObject.Drop();
                heldObject = null;
                return;
            }

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
        if (cam == null || armPivot == null || bodyVisual == null) return;

        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        // ¿Ratón a la izquierda o derecha del personaje?
        bool facingLeft = mouseWorldPos.x < transform.position.x;

        // 1) El cuerpo se pone mirando a izquierda o derecha
        Vector3 bodyScale = bodyOriginalScale;
        bodyScale.x = facingLeft ? -Mathf.Abs(bodyOriginalScale.x) : Mathf.Abs(bodyOriginalScale.x);
        bodyVisual.localScale = bodyScale;

        // 2) Dirección del brazo al ratón en coordenadas LOCALES del Player
        Vector3 localMouse = transform.InverseTransformPoint(mouseWorldPos);
        Vector3 localPivot = transform.InverseTransformPoint(armPivot.position);
        Vector2 aimDir = localMouse - localPivot;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        // 3) Si está mirando a la izquierda, remapeamos el ángulo
        // para que el brazo siga al ratón pero sin darse la vuelta
        if (facingLeft)
        {
            if (angle > 0f)
                angle = 180f - angle;
            else
                angle = -180f - angle;
        }

        // 4) Limitamos el rango del brazo
        angle = Mathf.Clamp(angle, minArmAngle, maxArmAngle);

        armPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
        armPivot.localScale = armOriginalScale;
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