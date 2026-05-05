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
    public Transform bodyVisual;      // cuerpo_0
    public Transform armPivot;        // brazoPivot
    public float minArmAngle = -80f;
    public float maxArmAngle = 80f;

    [Header("Throw")]
    public float maxChargeTime = 1.5f;
    public float minThrowForce = 3f;
    public float maxThrowForce = 15f;

    [Header("Power Bar")]
    public WorldPowerBar powerBar;    // PowerBar

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

        bodyOriginalScale = bodyVisual.localScale;

        if (armPivot != null)
            armOriginalScale = armPivot.localScale;

        if (powerBar != null)
            powerBar.Hide();
    }

    void Update()
    {
        HandlePickupOrDrop();
        HandleAiming();
        HandleThrowCharge();
    }

    void HandlePickupOrDrop()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (heldObject != null)
            {
                CancelCharge();

                if (powerBar != null)
                    powerBar.Hide();

                heldObject.Drop();
                heldObject = null;
                return;
            }

            if (pickupCheck == null) return;

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

                if (obj != null && holdPoint != null)
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
        if (Mouse.current == null) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();

        bool facingLeft = mouseWorldPos.x < transform.position.x;

        FlipBody(facingLeft);
        AimArm(mouseWorldPos);

        if (powerBar != null)
        {
            powerBar.SetSide(facingLeft);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        return mouseWorldPos;
    }

    void FlipBody(bool facingLeft)
    {
        Vector3 scale = bodyOriginalScale;
        scale.x = facingLeft ? -Mathf.Abs(bodyOriginalScale.x) : Mathf.Abs(bodyOriginalScale.x);
        bodyVisual.localScale = scale;
    }

    void AimArm(Vector3 mouseWorldPos)
    {
        Transform parent = armPivot.parent;

        if (parent == null) return;

        Vector3 localMouse = parent.InverseTransformPoint(mouseWorldPos);
        Vector2 localDir = localMouse - armPivot.localPosition;

        float angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;

        angle = Mathf.Clamp(angle, minArmAngle, maxArmAngle);

        armPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
        armPivot.localScale = armOriginalScale;
    }

    void HandleThrowCharge()
    {
        if (Mouse.current == null) return;

        if (heldObject == null)
        {
            CancelCharge();

            if (powerBar != null)
                powerBar.Hide();

            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isCharging = true;
            currentChargeTime = 0f;

            if (powerBar != null)
            {
                powerBar.Show();
                powerBar.SetPower(0f);
            }
        }

        if (isCharging && Mouse.current.leftButton.isPressed)
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, maxChargeTime);

            float chargePercent = currentChargeTime / maxChargeTime;

            if (powerBar != null)
                powerBar.SetPower(chargePercent);
        }

        if (isCharging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (holdPoint == null)
            {
                CancelCharge();
                return;
            }

            Vector3 mouseWorldPos = GetMouseWorldPosition();

            Vector2 direction = (mouseWorldPos - holdPoint.position).normalized;

            float chargePercent = currentChargeTime / maxChargeTime;
            float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, chargePercent);

            heldObject.Throw(direction * throwForce);
            heldObject = null;

            CancelCharge();

            if (powerBar != null)
                powerBar.Hide();
        }
    }

    void CancelCharge()
    {
        isCharging = false;
        currentChargeTime = 0f;

        if (powerBar != null)
            powerBar.SetPower(0f);
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