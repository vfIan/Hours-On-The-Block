using System;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public event Action OnEnemyDestroyed;

    [Header("Target")]
    public Transform player;

    [Header("Visual")]
    public Transform bodyVisual;   // cuerpo_enemigo
    public Transform armPivot;     // brazoPivot, dentro de cuerpo_enemigo

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
        FindPlayerIfNeeded();

        if (bodyVisual == null)
            bodyVisual = transform.Find("cuerpo_enemigo");

        if (bodyVisual == null)
            bodyVisual = transform.Find("cuerpo_0");

        if (armPivot == null && bodyVisual != null)
            armPivot = bodyVisual.Find("brazoPivot");

        if (shootPoint == null && armPivot != null)
            shootPoint = armPivot.Find("ShootPoint");

        if (bodyVisual != null)
            bodyOriginalScale = bodyVisual.localScale;

        if (armPivot != null)
            armOriginalScale = armPivot.localScale;

        CheckReferences();
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifeTime)
        {
            DestroyEnemy();
            return;
        }

        FindPlayerIfNeeded();

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

    void FindPlayerIfNeeded()
    {
        if (player != null) return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    void AimAtPlayer()
    {
        if (player == null || bodyVisual == null || armPivot == null)
            return;

        bool playerIsLeft = player.position.x < transform.position.x;

        // Girar cuerpo
        Vector3 bodyScale = bodyOriginalScale;
        bodyScale.x = playerIsLeft ? -Mathf.Abs(bodyOriginalScale.x) : Mathf.Abs(bodyOriginalScale.x);
        bodyVisual.localScale = bodyScale;

        // Como brazoPivot está dentro del cuerpo, calculamos respecto a su padre.
        Transform armParent = armPivot.parent;

        if (armParent == null)
            return;

        Vector3 localPlayerPos = armParent.InverseTransformPoint(player.position);
        Vector2 localDirection = localPlayerPos - armPivot.localPosition;

        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;

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
                DestroyEnemy();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || shootPoint == null || player == null)
        {
            CheckReferences();
            return;
        }

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
        else
        {
            Debug.LogWarning("EnemyShooter: El proyectil no tiene Rigidbody2D.", projectile);
        }

        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
            enemyProjectile.speed = shootForce;
    }

    void CheckReferences()
    {
        if (player == null)
            Debug.LogWarning("EnemyShooter: No encuentra Player. Revisa que el jugador tenga Tag = Player.", this);

        if (bodyVisual == null)
            Debug.LogWarning("EnemyShooter: Falta Body Visual. Arrastra cuerpo_enemigo o cuerpo_0.", this);

        if (armPivot == null)
            Debug.LogWarning("EnemyShooter: Falta Arm Pivot. Arrastra brazoPivot.", this);

        if (shootPoint == null)
            Debug.LogWarning("EnemyShooter: Falta Shoot Point. Debe estar dentro de brazoPivot.", this);

        if (projectilePrefab == null)
            Debug.LogWarning("EnemyShooter: Falta Projectile Prefab.", this);
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