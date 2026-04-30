using UnityEngine;

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
    public float powerBarSideOffsetX = 1.2f;
    public float powerBarOffsetY = 1.5f;

    private ThrowableObject heldObject;
    private Camera cam;

    private bool isCharging = false;
    private float currentChargeTime = 0f;

    private Vector3 bodyOriginalScale;
    private Vector3 armOriginalScale;
    private Vector3 powerBarOriginalScale;

    void Start()
    {
        cam = Camera.main;

        if (bodyVisual == null)
            bodyVisual = transform;

        bodyOriginalScale = bodyVisual.localScale;

        if (armPivot != null)
            armOriginalScale = armPivot.localScale;

        if (powerBar != null)
            powerBarOriginalScale = powerBar.transform.localScale;
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

                if (powerBar != null)
                    powerBar.Hide();

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
        if (cam == null || armPivot == null || bodyVisual == null) return;

        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        bool facingLeft = mouseWorldPos.x < transform.position.x;

        FlipBody(facingLeft);
        AimArm(mouseWorldPos);
        UpdatePowerBarPosition(facingLeft);
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

    void UpdatePowerBarPosition(bool facingLeft)
    {
        if (powerBar == null) return;

        float xOffset;

        if (facingLeft)
        {
            // Si apunta a la izquierda, la barra aparece a la derecha.
            xOffset = powerBarSideOffsetX;
        }
        else
        {
            // Si apunta a la derecha, la barra aparece a la izquierda.
            xOffset = -powerBarSideOffsetX;
        }

        Vector3 worldPos = transform.position + new Vector3(xOffset, powerBarOffsetY, 0f);
        powerBar.transform.position = worldPos;

        // Como PowerBar está dentro de cuerpo_0, compensamos el flip para que no se vea espejada.
        Vector3 scale = powerBarOriginalScale;

        if (facingLeft)
            scale.x = -Mathf.Abs(powerBarOriginalScale.x);
        else
            scale.x = Mathf.Abs(powerBarOriginalScale.x);

        powerBar.transform.localScale = scale;
    }

    void HandleThrowCharge()
    {
        if (heldObject == null)
        {
            CancelCharge();

            if (powerBar != null)
                powerBar.Hide();

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            currentChargeTime = 0f;

            if (powerBar != null)
            {
                powerBar.Show();
                powerBar.SetPower(0f);
            }
        }

        if (isCharging && Input.GetMouseButton(0))
        {
            currentChargeTime += Time.deltaTime;
            currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, maxChargeTime);

            float chargePercent = currentChargeTime / maxChargeTime;

            if (powerBar != null)
                powerBar.SetPower(chargePercent);
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