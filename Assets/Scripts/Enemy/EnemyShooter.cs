using System;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public event Action OnEnemyDestroyed;

    [Header("Target")]
    public Transform player;

    [Header("Visual")]
    public Transform bodyVisual;   // cuerpo_0
    public Transform armPivot;     // brazoPivot, mueve los dos brazos

    [Header("Aiming")]
    public float minArmAngle = -80f;
    public float maxArmAngle = 80f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootForce = 8f;
    public float timeBeforeFirstShot = 5f;
    public float shootInterval = 2f;
    public bool shootRepeatedly = true;

    [Header("Lifetime")]
    public bool destroyAfterOneShot = false;
    public float lifeTime = 15f;

    private float firstShotTimer = 0f;
    private float shootTimer = 0f;
    private float lifeTimer = 0f;

    private bool canShoot = false;
    private bool hasShot = false;

    private Vector3 bodyOriginalScale;
    private Vector3 armOriginalScale;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (bodyVisual == null)
            bodyVisual = transform;

        bodyOriginalScale = bodyVisual.localScale;

        if (armPivot != null)
            armOriginalScale = armPivot.localScale;
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifeTime)
        {
            DestroyEnemy();
            return;
        }

        AimAtPlayer();

        if (!canShoot)
        {
            firstShotTimer += Time.deltaTime;

            if (firstShotTimer >= timeBeforeFirstShot)
            {
                canShoot = true;
                shootTimer = shootInterval;
            }

            return;
        }

        HandleShooting();
    }

    void AimAtPlayer()
    {
        if (player == null || armPivot == null || bodyVisual == null)
            return;

        bool playerIsLeft = player.position.x < transform.position.x;

        // Girar cuerpo
        Vector3 bodyScale = bodyOriginalScale;
        bodyScale.x = playerIsLeft ? -Mathf.Abs(bodyOriginalScale.x) : Mathf.Abs(bodyOriginalScale.x);
        bodyVisual.localScale = bodyScale;

        // Calcular ángulo del brazo respecto al padre del pivot
        Transform armParent = armPivot.parent;

        if (armParent == null)
            return;

        Vector3 localPlayerPos = armParent.InverseTransformPoint(player.position);
        Vector2 localDir = localPlayerPos - armPivot.localPosition;

        float angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;

        if (playerIsLeft)
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

    void HandleShooting()
    {
        if (!shootRepeatedly && hasShot)
            return;

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            Shoot();
            shootTimer = 0f;
            hasShot = true;

            if (destroyAfterOneShot)
            {
                DestroyEnemy();
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || shootPoint == null || player == null)
            return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            shootPoint.position,
            Quaternion.identity
        );

        Vector2 direction = (player.position - shootPoint.position).normalized;

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = direction * shootForce;
        }

        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
        {
            enemyProjectile.speed = shootForce;
        }
    }

    void DestroyEnemy()
    {
        OnEnemyDestroyed?.Invoke();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        OnEnemyDestroyed?.Invoke();
    }
}