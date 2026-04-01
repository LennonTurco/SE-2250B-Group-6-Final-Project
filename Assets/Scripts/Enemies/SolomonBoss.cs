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

    [Header("Tank Idle Animation Frames (6 each)")]
    [SerializeField] Sprite[] tankIdleUp;
    [SerializeField] Sprite[] tankIdleDown;
    [SerializeField] Sprite[] tankIdleLeft;
    [SerializeField] Sprite[] tankIdleRight;

    [Header("Tank Move Animation Frames (6 each)")]
    [SerializeField] Sprite[] tankMoveUp;
    [SerializeField] Sprite[] tankMoveDown;
    [SerializeField] Sprite[] tankMoveLeft;
    [SerializeField] Sprite[] tankMoveRight;

    [Header("Tank Fire Animation Frames (6 each)")]
    [SerializeField] Sprite[] tankFireUp;
    [SerializeField] Sprite[] tankFireDown;
    [SerializeField] Sprite[] tankFireLeft;
    [SerializeField] Sprite[] tankFireRight;

    [Header("Animation Settings")]
    [SerializeField] float animFrameRate = 8f;

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
    float animTimer;
    int animFrame;
    Sprite[] lastFrames;
    Vector2 lastDir = Vector2.down;

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

        // always update animation, even while firing
        UpdateFacing(lastDir);
        UpdateHealthBar();
    }

    void TankAI()
    {
        float dist = Vector2.Distance(transform.position, target.position);
        Vector2 dir = (target.position - transform.position).normalized;
        lastDir = dir;

        // don't move or start new burst while firing
        if (isFiring)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

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
        float time = Time.time * heliMoveSpeed * 0.3f;
        Vector2 offset = new Vector2(Mathf.Cos(time), Mathf.Sin(time)) * heliStrafeWidth;
        Vector2 targetPos = (Vector2)target.position + offset;
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        lastDir = dir;

        if (!isFiring)
            rb.linearVelocity = dir * moveSpeed;
        else
            rb.linearVelocity = Vector2.zero;

        if (fireTimer <= 0f && !isFiring)
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

        Sprite[] frames;
        bool moving = rb.linearVelocity.magnitude > 0.1f;

        if (currentPhase == Phase.Tank)
        {
            if (isFiring)
            {
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    frames = dir.x > 0 ? tankFireRight : tankFireLeft;
                else
                    frames = dir.y > 0 ? tankFireUp : tankFireDown;
            }
            else if (moving)
            {
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    frames = dir.x > 0 ? tankMoveRight : tankMoveLeft;
                else
                    frames = dir.y > 0 ? tankMoveUp : tankMoveDown;
            }
            else
            {
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    frames = dir.x > 0 ? tankIdleRight : tankIdleLeft;
                else
                    frames = dir.y > 0 ? tankIdleUp : tankIdleDown;
            }
        }
        else
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                visualRenderer.sprite = dir.x > 0 ? heliRight : heliLeft;
            else
                visualRenderer.sprite = dir.y > 0 ? heliUp : heliDown;
            return;
        }

        if (frames == null || frames.Length == 0) return;

        if (frames != lastFrames)
        {
            lastFrames = frames;
            animFrame = 0;
            animTimer = 0f;
        }

        animTimer += Time.deltaTime;
        if (animTimer >= 1f / animFrameRate)
        {
            animTimer -= 1f / animFrameRate;
            animFrame = (animFrame + 1) % frames.Length;
        }

        visualRenderer.sprite = frames[animFrame];
    }

    void FireProjectile(GameObject prefab, float speed)
    {
        if (prefab == null || target == null) return;

        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;

        Vector2 barrelOffset;
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
        {
            if (lastDir.x > 0)
                barrelOffset = new Vector2(4.2f, 1f);
            else
                barrelOffset = new Vector2(-4.2f, 1f);
        }
        else
        {
            if (lastDir.y > 0)
                barrelOffset = new Vector2(0f, 4.2f);
            else
                barrelOffset = new Vector2(0f, -4.2f);
        }

        Vector2 spawnPos = (Vector2)transform.position + barrelOffset;
        Vector2 aimDir = ((Vector2)target.position - spawnPos).normalized;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle);

        GameObject proj = Instantiate(prefab, spawnPos, rot);
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
        if (projRb != null)
            projRb.linearVelocity = aimDir * speed;

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