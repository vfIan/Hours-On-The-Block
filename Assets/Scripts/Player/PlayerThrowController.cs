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
    public Transform bodyVisual;
    public Transform armPivot;
    public float minArmAngle = -80f;
    public float maxArmAngle = 80f;

    [Header("Throw")]
    public float maxChargeTime = 1.5f;
    public float minThrowForce = 3f;
    public float maxThrowForce = 15f;

    [Header("Power Bar")]
    public WorldPowerBar powerBar;

    private ThrowableObject heldObject;
    private Camera cam;

    private bool isCharging = false;
    private float currentChargeTime = 0f;

    private Vector2 mouseScreenPosition;

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
        HandleAiming();
        HandleThrowCharge();
    }

    public void OnMousePosition(InputValue value)
    {
        mouseScreenPosition = value.Get<Vector2>();
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (heldObject != null)
        {
            CancelCharge();

            if (powerBar != null)
                powerBar.Hide();

            heldObject.Drop();
            heldObject = null;
            return;
        }

        TryPickUpObject();
    }

    public void OnThrow(InputValue value)
    {
        if (value.isPressed)
        {
            StartChargingThrow();
        }
        else
        {
            ReleaseThrow();
        }
    }

    void TryPickUpObject()
    {
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

    void HandleAiming()
    {
        if (cam == null || armPivot == null || bodyVisual == null) return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();

        bool facingLeft = mouseWorldPos.x < transform.position.x;

        FlipBody(facingLeft);
        AimArm(mouseWorldPos);

        if (powerBar != null)
            powerBar.SetSide(facingLeft);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPos.z = 0f;

        return mouseWorldPos;
    }

    void FlipBody(bool facingLeft)
    {
        if (bodyVisual == null) return;

        Vector3 scale = bodyOriginalScale;
        scale.x = facingLeft ? -Mathf.Abs(bodyOriginalScale.x) : Mathf.Abs(bodyOriginalScale.x);
        bodyVisual.localScale = scale;
    }

    void AimArm(Vector3 mouseWorldPos)
    {
        if (armPivot == null) return;

        Transform parent = armPivot.parent;

        if (parent == null) return;

        Vector3 localMouse = parent.InverseTransformPoint(mouseWorldPos);
        Vector2 localDir = localMouse - armPivot.localPosition;

        float angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;

        angle = Mathf.Clamp(angle, minArmAngle, maxArmAngle);

        armPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
        armPivot.localScale = armOriginalScale;
    }

    void StartChargingThrow()
    {
        if (heldObject == null) return;

        isCharging = true;
        currentChargeTime = 0f;

        if (powerBar != null)
        {
            powerBar.Show();
            powerBar.SetPower(0f);
        }
    }

    void HandleThrowCharge()
    {
        if (!isCharging) return;

        if (heldObject == null)
        {
            CancelCharge();

            if (powerBar != null)
                powerBar.Hide();

            return;
        }

        currentChargeTime += Time.deltaTime;
        currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, maxChargeTime);

        float chargePercent = currentChargeTime / maxChargeTime;

        if (powerBar != null)
            powerBar.SetPower(chargePercent);
    }

    void ReleaseThrow()
    {
        if (!isCharging) return;

        if (heldObject == null || holdPoint == null)
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