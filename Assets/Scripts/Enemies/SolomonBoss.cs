using UnityEngine;
using System.Collections;

public class SolomonBoss : Enemy
{
    [Header("Phase Threshold")]
    [SerializeField] float phase2HealthThreshold = 0.5f;

    [Header("Projectile")]
    [SerializeField] GameObject tankProjectilePrefab;
    [SerializeField] float tankProjectileSpeed = 8f;

    [Header("Phase 1 - Tank")]
    [SerializeField] float p1MoveSpeed = 3f;
    [SerializeField] float p1StopRange = 5f;
    [SerializeField] float p1FireRate = 1.5f;
    [SerializeField] int p1BurstCount = 3;
    [SerializeField] float p1BurstDelay = 0.3f;

    [Header("Phase 2 - Enraged Tank")]
    [SerializeField] float p2MoveSpeed = 6f;
    [SerializeField] float p2FireRate = 1.0f;
    [SerializeField] int p2CircleBullets = 12;     // bullets per circular burst
    [SerializeField] float p2DashSpeed = 18f;
    [SerializeField] float p2DashDuration = 0.25f;
    [SerializeField] float p2DashCooldown = 3f;

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

    [Header("Animation")]
    [SerializeField] float animFrameRate = 8f;

    [Header("UI")]
    [SerializeField] UnityEngine.UI.Slider healthBar;

    // internal state
    enum Phase { One, Two }
    Phase currentPhase = Phase.One;

    float fireTimer;
    float dashTimer;
    bool isFiring = false;
    bool isDashing = false;
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

        visualRenderer = GetComponentInChildren<SpriteRenderer>();
        moveSpeed = p1MoveSpeed;

        if (healthBar == null)
        {
            GameObject sliderGO = GameObject.Find("Slider");
            if (sliderGO != null)
                healthBar = sliderGO.GetComponent<UnityEngine.UI.Slider>();
        }

        if (healthBar != null)
        {
            CanvasGroup cg = healthBar.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 1f;
        }

        UpdateHealthBar();
    }

    protected override void HandleAI()
    {
        if (isDead || phaseTransitioning || target == null) return;

        // check phase transition
        if (currentPhase == Phase.One && currentHealth <= maxHealth * phase2HealthThreshold)
        {
            StartCoroutine(TransitionToPhase2());
            return;
        }

        fireTimer -= Time.deltaTime;
        if (currentPhase == Phase.Two) dashTimer -= Time.deltaTime;

        if (currentPhase == Phase.One)
            Phase1AI();
        else
            Phase2AI();

        UpdateFacing(lastDir);
        UpdateHealthBar();
    }

    // phase 1 - move toward player, burst fire at target
    void Phase1AI()
    {
        float dist = Vector2.Distance(transform.position, target.position);
        Vector2 dir = (target.position - transform.position).normalized;
        lastDir = dir;

        if (isFiring)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = dist > p1StopRange ? dir * moveSpeed : Vector2.zero;

        if (fireTimer <= 0f)
        {
            StartCoroutine(Phase1Burst());
            fireTimer = p1FireRate + (p1BurstCount * p1BurstDelay);
        }
    }

    IEnumerator Phase1Burst()
    {
        isFiring = true;
        rb.linearVelocity = Vector2.zero;

        for (int i = 0; i < p1BurstCount; i++)
        {
            FireDirectional(tankProjectilePrefab, tankProjectileSpeed);
            yield return new WaitForSeconds(p1BurstDelay);
        }

        isFiring = false;
    }

    // phase 2 - dashes at player, fires circular bursts
    void Phase2AI()
    {
        Vector2 dir = (target.position - transform.position).normalized;
        lastDir = dir;

        if (isFiring || isDashing) return;

        rb.linearVelocity = dir * moveSpeed;

        // dash takes priority over fire
        if (dashTimer <= 0f)
        {
            StartCoroutine(Dash(dir));
            dashTimer = p2DashCooldown;
        }
        else if (fireTimer <= 0f)
        {
            StartCoroutine(CircularBurst());
            fireTimer = p2FireRate;
        }
    }

    IEnumerator Dash(Vector2 dir)
    {
        isDashing = true;
        rb.linearVelocity = dir * p2DashSpeed;
        yield return new WaitForSeconds(p2DashDuration);
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }

    IEnumerator CircularBurst()
    {
        isFiring = true;
        rb.linearVelocity = Vector2.zero;

        float angleStep = 360f / p2CircleBullets;

        for (int i = 0; i < p2CircleBullets; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );
            FireInDirection(tankProjectilePrefab, tankProjectileSpeed, dir);
            yield return new WaitForSeconds(0.05f);
        }

        isFiring = false;
    }

    IEnumerator TransitionToPhase2()
    {
        phaseTransitioning = true;
        rb.linearVelocity = Vector2.zero;

        // brief pause to signal phase change
        yield return new WaitForSeconds(1.5f);

        currentPhase = Phase.Two;
        moveSpeed = p2MoveSpeed;
        fireTimer = p2FireRate;
        dashTimer = 1f; // short delay before first dash

        phaseTransitioning = false;
    }

    // fire toward current target position
    void FireDirectional(GameObject prefab, float speed)
    {
        if (prefab == null || target == null) return;

        Vector2 barrelOffset = GetBarrelOffset();
        Vector2 spawnPos = (Vector2)transform.position + barrelOffset;
        Vector2 aimDir = ((Vector2)target.position - spawnPos).normalized;

        SpawnProjectile(prefab, spawnPos, aimDir, speed);
    }

    // fire in an explicit direction (used for circular burst)
    void FireInDirection(GameObject prefab, float speed, Vector2 dir)
    {
        if (prefab == null) return;

        Vector2 spawnPos = (Vector2)transform.position;
        SpawnProjectile(prefab, spawnPos, dir.normalized, speed);
    }

    void SpawnProjectile(GameObject prefab, Vector2 spawnPos, Vector2 dir, float speed)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameObject proj = Instantiate(prefab, spawnPos, Quaternion.Euler(0, 0, angle));

        BossProjectile bp = proj.GetComponent<BossProjectile>();
        if (bp != null)
            bp.Initialize(dir, speed, gameObject);
        else
        {
            Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
            if (projRb != null) projRb.linearVelocity = dir * speed;
        }

        Destroy(proj, 5f);
    }

    Vector2 GetBarrelOffset()
    {
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
            return lastDir.x > 0 ? new Vector2(4.2f, 1f) : new Vector2(-4.2f, 1f);
        else
            return lastDir.y > 0 ? new Vector2(0f, 4.2f) : new Vector2(0f, -4.2f);
    }

    void UpdateFacing(Vector2 dir)
    {
        if (visualRenderer == null) return;

        Sprite[] frames;
        bool moving = rb.linearVelocity.magnitude > 0.1f;

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
        {
            CanvasGroup cg = healthBar.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0f;
        }

        Debug.Log("[SolomonBoss] Defeated!");
        base.Die();
    }
}