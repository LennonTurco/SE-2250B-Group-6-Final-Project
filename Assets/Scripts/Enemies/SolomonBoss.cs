using UnityEngine;
using System.Collections;

public class SolomonBoss : Enemy
{
    [Header("Phase Settings")]
    [SerializeField] float phase2HealthThreshold = 0.5f;

    [Header("Tank Phase")]
    [SerializeField] GameObject tankProjectilePrefab;
    [SerializeField] float tankFireRate = 1.5f;
    [SerializeField] float tankProjectileSpeed = 8f;
    [SerializeField] float tankMoveSpeed = 3f;
    [SerializeField] float tankStopRange = 5f;
    [SerializeField] int tankBurstCount = 3;
    [SerializeField] float tankBurstDelay = 0.3f;

    [Header("Helicopter Phase")]
    [SerializeField] GameObject heliProjectilePrefab;
    [SerializeField] float heliFireRate = 0.8f;
    [SerializeField] float heliProjectileSpeed = 10f;
    [SerializeField] float heliMoveSpeed = 5f;
    [SerializeField] float heliStrafeWidth = 8f;
    [SerializeField] int heliStrafeCount = 5;
    [SerializeField] float heliStrafeBurstDelay = 0.15f;

    [Header("Visuals")]
    [SerializeField] GameObject tankVisuals;
    [SerializeField] GameObject heliVisuals;

    [Header("Tank Directional Sprites")]
    [SerializeField] Sprite tankUp;
    [SerializeField] Sprite tankDown;
    [SerializeField] Sprite tankLeft;
    [SerializeField] Sprite tankRight;

    [Header("Heli Directional Sprites")]
    [SerializeField] Sprite heliUp;
    [SerializeField] Sprite heliDown;
    [SerializeField] Sprite heliLeft;
    [SerializeField] Sprite heliRight;

    [Header("UI")]
    [SerializeField] UnityEngine.UI.Slider healthBar;

    enum Phase { Tank, Helicopter }
    Phase currentPhase = Phase.Tank;
    float fireTimer;
    bool isFiring = false;
    bool phaseTransitioning = false;
    SpriteRenderer visualRenderer;

    protected override void Start()
    {
        base.Start();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;

        if (tankVisuals != null)
        {
            tankVisuals.SetActive(true);
            visualRenderer = tankVisuals.GetComponentInChildren<SpriteRenderer>();
        }
        if (heliVisuals != null) heliVisuals.SetActive(false);

        moveSpeed = tankMoveSpeed;
        UpdateHealthBar();
    }

    protected override void HandleAI()
    {
        if (isDead || phaseTransitioning || target == null) return;

        if (currentPhase == Phase.Tank && currentHealth <= maxHealth * phase2HealthThreshold)
        {
            StartCoroutine(TransitionToPhase2());
            return;
        }

        fireTimer -= Time.deltaTime;

        if (currentPhase == Phase.Tank)
            TankAI();
        else
            HelicopterAI();

        UpdateHealthBar();
    }

    void TankAI()
    {
        if (isFiring) return;

        float dist = Vector2.Distance(transform.position, target.position);
        Vector2 dir = (target.position - transform.position).normalized;

        UpdateFacing(dir);

        if (dist > tankStopRange)
            rb.linearVelocity = dir * moveSpeed;
        else
            rb.linearVelocity = Vector2.zero;

        if (fireTimer <= 0f)
        {
            StartCoroutine(TankBurst());
            fireTimer = tankFireRate + (tankBurstCount * tankBurstDelay);
        }
    }

    IEnumerator TankBurst()
    {
        isFiring = true;
        rb.linearVelocity = Vector2.zero;

        for (int i = 0; i < tankBurstCount; i++)
        {
            FireProjectile(tankProjectilePrefab, tankProjectileSpeed);
            yield return new WaitForSeconds(tankBurstDelay);
        }

        isFiring = false;
    }

    IEnumerator TransitionToPhase2()
    {
        phaseTransitioning = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1.5f);

        if (tankVisuals != null) tankVisuals.SetActive(false);
        if (heliVisuals != null)
        {
            heliVisuals.SetActive(true);
            visualRenderer = heliVisuals.GetComponentInChildren<SpriteRenderer>();
        }

        currentPhase = Phase.Helicopter;
        moveSpeed = heliMoveSpeed;
        fireTimer = heliFireRate;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.isTrigger = true;

        phaseTransitioning = false;
    }

    void HelicopterAI()
    {
        if (isFiring) return;

        float time = Time.time * heliMoveSpeed * 0.3f;
        Vector2 offset = new Vector2(Mathf.Cos(time), Mathf.Sin(time)) * heliStrafeWidth;
        Vector2 targetPos = (Vector2)target.position + offset;
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        UpdateFacing(dir);

        if (fireTimer <= 0f)
        {
            StartCoroutine(HeliStrafe());
            fireTimer = heliFireRate + (heliStrafeCount * heliStrafeBurstDelay) + 1f;
        }
    }

    IEnumerator HeliStrafe()
    {
        isFiring = true;

        for (int i = 0; i < heliStrafeCount; i++)
        {
            FireProjectile(heliProjectilePrefab, heliProjectileSpeed);
            yield return new WaitForSeconds(heliStrafeBurstDelay);
        }

        isFiring = false;
    }

    void UpdateFacing(Vector2 dir)
    {
        if (visualRenderer == null) return;

        Sprite up, down, left, right;
        if (currentPhase == Phase.Tank)
        {
            up = tankUp; down = tankDown; left = tankLeft; right = tankRight;
        }
        else
        {
            up = heliUp; down = heliDown; left = heliLeft; right = heliRight;
        }

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            visualRenderer.sprite = dir.x > 0 ? right : left;
        else
            visualRenderer.sprite = dir.y > 0 ? up : down;
    }

    void FireProjectile(GameObject prefab, float speed)
    {
        if (prefab == null || target == null) return;

        // barrel offset based on current facing
        Vector2 barrelOffset;
        if (visualRenderer != null && visualRenderer.sprite != null)
        {
            if (visualRenderer.sprite == tankRight || visualRenderer.sprite == heliRight)
                barrelOffset = new Vector2(4.2f, 1f);
            else if (visualRenderer.sprite == tankLeft || visualRenderer.sprite == heliLeft)
                barrelOffset = new Vector2(-4.2f, 1f);
            else if (visualRenderer.sprite == tankUp || visualRenderer.sprite == heliUp)
                barrelOffset = new Vector2(0f, 4.2f);
            else
                barrelOffset = new Vector2(0f, -4.2f);
        }
        else
        {
            barrelOffset = Vector2.zero;
        }

        Vector2 spawnPos = (Vector2)transform.position + barrelOffset;

        // aim at player from barrel position
        Vector2 dir = ((Vector2)target.position - spawnPos).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        GameObject proj = Instantiate(prefab, spawnPos, rot);
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
            projRb.linearVelocity = dir * speed;

        Destroy(proj, 5f);
    }

    void UpdateHealthBar()
    {
        if (healthBar == null) return;
        healthBar.value = currentHealth / maxHealth;
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        UpdateHealthBar();
    }

    protected override void Die()
    {
        rb.linearVelocity = Vector2.zero;
        StopAllCoroutines();

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        Debug.Log("[SolomonBoss] Defeated!");
        base.Die();
    }
}