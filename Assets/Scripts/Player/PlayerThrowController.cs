using UnityEngine;

public class PlayerThrowController : MonoBehaviour
{
    [Header("Pickup")]
    public Transform pickupCheck;
    public float pickupRadius = 0.7f;
    public LayerMask throwableLayer;
    public Transform holdPoint;

    [Header("Aiming")]
    public Transform bodyVisual;
    public Transform armPivot;
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
        if (Input.GetKeyDown(KeyCode.E))
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
                ThrowableObject obj = closest.GetComponentInParent<ThrowableObject>();
                
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
        if (cam == null || armPivot == null) return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        bool facingLeft = mouseWorldPos.x < transform.position.x;

        Vector3 bodyScale = bodyOriginalScale;
        bodyScale.x = facingLeft ? -Mathf.Abs(bodyOriginalScale.x) : Mathf.Abs(bodyOriginalScale.x);
        bodyVisual.localScale = bodyScale;

        Vector3 localMouse = transform.InverseTransformPoint(mouseWorldPos);
        Vector3 localPivot = transform.InverseTransformPoint(armPivot.position);

        Vector2 localAimDir = localMouse - localPivot;

        float angle = Mathf.Atan2(localAimDir.y, localAimDir.x) * Mathf.Rad2Deg;

        if (facingLeft)
        {
            if (angle > 0f)
                angle = 180f - angle;
            else
                angle = -180f - angle;
        }

        angle = Mathf.Clamp(angle, minArmAngle, maxArmAngle);

        armPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
        armPivot.localScale = armOriginalScale;
    }

    void HandleThrowCharge()
    {
        if (heldObject == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            currentChargeTime = 0f;
        }

        if (isCharging && Input.GetMouseButton(0))
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, maxChargeTime);
        }

        if (isCharging && Input.GetMouseButtonUp(0))
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
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