using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JungleBoss : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Boss Rewards")]
    [SerializeField] private GameObject levelLoadZoneToEnable;

    [Header("Stationary Behaviour")]
    [SerializeField] private float playerAppearanceDuration = 3f;
    [SerializeField] private float playerDisappearanceDuration = 0.75f;
    [SerializeField] private float repositionSquareSize = 5f;
    [SerializeField] private float repositionPadding = 0.35f;
    [SerializeField] private int shotsBeforeDisappear = 1;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform target;
    [SerializeField] private int projectilesPerShot = 3;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireInterval = 1.25f;
    [SerializeField] private float spreadAngle = 25f;
    [SerializeField] private float projectileDamage = 10f;

    [Header("Smoke Effect")]
    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private float smokeEffectLifetime = 1f;
    [SerializeField] private bool spawnSmokeOnDisappear = true;
    [SerializeField] private bool spawnSmokeOnAppear = true;
    [SerializeField] private string projectileTag = "NinjaStar";

    [Header("Optional")]
    [SerializeField] private float initialDelay = 1f;

    [Header("Animation")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float animationFrameRate = 8f;

    [Header("Idle Sprites")]
    [SerializeField] private Sprite[] idleUp;
    [SerializeField] private Sprite[] idleDown;
    [SerializeField] private Sprite[] idleLeft;
    [SerializeField] private Sprite[] idleRight;

    [Header("Shoot Sprites")]
    [SerializeField] private Sprite[] shootUp;
    [SerializeField] private Sprite[] shootDown;
    [SerializeField] private Sprite[] shootLeft;
    [SerializeField] private Sprite[] shootRight;
    [SerializeField] private float shootAnimationDuration = 0.15f;

    [Header("Compatibility")]
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float roamRadius = 0f;
    [SerializeField] private float destinationThreshold = 0.25f;
    [SerializeField] private float moveDuration = 0f;
    [SerializeField] private float pauseDuration = 0f;
    [SerializeField] private Sprite[] walkUp;
    [SerializeField] private Sprite[] walkDown;
    [SerializeField] private Sprite[] walkLeft;
    [SerializeField] private Sprite[] walkRight;

    [Header("UI")]
    public Slider healthBar;

    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private Vector2 facingDirection = Vector2.down;
    private float fireTimer;
    private float visibilityTimer;
    private float shootAnimationTimer;
    private float animationTimer;
    private int animationFrame;
    private Sprite[] currentAnimationFrames;
    private float currentHealth;
    private bool isDead;
    private bool isVisible = true;
    private Coroutine visibilityRoutine;
    private int shotsFiredThisCycle;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        currentHealth = maxHealth;
        fireTimer = initialDelay > 0f ? initialDelay : fireInterval;
        visibilityTimer = playerAppearanceDuration;
        shotsFiredThisCycle = 0;

        AcquireTarget();

        if (firePoint == null) firePoint = transform;

        UpdateHealthBar();

        if (levelLoadZoneToEnable != null)
            levelLoadZoneToEnable.SetActive(false);
    }

    private void Update()
    {
        if (isDead) return;

        AcquireTarget();

        if (isVisible)
        {
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                FireAtPlayer();
                fireTimer = fireInterval;
            }

            visibilityTimer -= Time.deltaTime;
            if (visibilityTimer <= 0f && shotsFiredThisCycle >= Mathf.Max(1, shotsBeforeDisappear))
                visibilityRoutine = StartCoroutine(DisappearAndReappear());
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    public void TakeDamage(float amount)
    {
        if (isDead || !isVisible || amount <= 0f) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log($"JungleBoss took {amount} damage. Health: {currentHealth}/{maxHealth}");
        UpdateHealthBar();

        if (currentHealth <= 0f) Die();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        isVisible = false;

        if (visibilityRoutine != null)
        {
            StopCoroutine(visibilityRoutine);
            visibilityRoutine = null;
        }

        if (rb != null) rb.linearVelocity = Vector2.zero;

        SetBossVisible(false);

        if (healthBar != null) healthBar.gameObject.SetActive(false);
        if (levelLoadZoneToEnable != null) levelLoadZoneToEnable.SetActive(true);

        Debug.Log("JungleBoss defeated.");
        Destroy(gameObject);
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null) return;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    private IEnumerator DisappearAndReappear()
    {
        if (!isVisible || isDead) yield break;

        isVisible = false;
        fireTimer = fireInterval;

        // removed projectile cleanup - just smoke and teleport
        if (spawnSmokeOnDisappear) SpawnSmoke(transform.position);

        SetBossVisible(false);

        if (playerDisappearanceDuration > 0f)
            yield return new WaitForSeconds(playerDisappearanceDuration);

        if (isDead) yield break;

        transform.position = GetRandomPositionNearTarget();

        if (spawnSmokeOnAppear) SpawnSmoke(transform.position);

        SetBossVisible(true);
        isVisible = true;
        visibilityTimer = playerAppearanceDuration;
        shotsFiredThisCycle = 0;
        visibilityRoutine = null;
    }

    private void SetBossVisible(bool shouldBeVisible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = shouldBeVisible;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] != null)
                colliders[i].enabled = shouldBeVisible;
        }
    }

    private Vector2 GetRandomPositionNearTarget()
    {
        AcquireTarget();

        Vector2 center = target != null ? (Vector2)target.position : (Vector2)transform.position;
        float halfSize = repositionSquareSize * 0.5f;
        float padding = Mathf.Clamp(repositionPadding, 0f, halfSize);

        return new Vector2(
            Random.Range(center.x - halfSize + padding, center.x + halfSize - padding),
            Random.Range(center.y - halfSize + padding, center.y + halfSize - padding)
        );
    }

    private void SpawnSmoke(Vector2 position)
    {
        if (smokeEffectPrefab == null) return;
        GameObject smokeInstance = Instantiate(smokeEffectPrefab, position, Quaternion.identity);
        if (smokeEffectLifetime > 0f) Destroy(smokeInstance, smokeEffectLifetime);
    }

    private void AcquireTarget()
    {
        if (target != null) return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) { target = playerObject.transform; return; }

        Player player = FindFirstObjectByType<Player>();
        if (player != null) target = player.transform;
    }

    private void FireAtPlayer()
    {
        if (!isVisible || projectilePrefab == null) return;

        AcquireTarget();
        if (target == null) return;

        Vector2 direction = ((Vector2)target.position - (Vector2)firePoint.position).normalized;
        if (direction == Vector2.zero) return;

        int shotCount = Mathf.Max(1, projectilesPerShot);
        float totalSpread = Mathf.Max(0f, spreadAngle);
        float startAngle = -totalSpread * 0.5f;
        float angleStep = shotCount > 1 ? totalSpread / (shotCount - 1) : 0f;

        for (int i = 0; i < shotCount; i++)
        {
            float angleOffset = startAngle + (angleStep * i);
            Vector2 shotDirection = Quaternion.Euler(0f, 0f, angleOffset) * direction;

            GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Debug.Log($"[JungleBoss] Spawned projectile at {firePoint.position} dir {shotDirection}");

            // try ShurikenProjectile first
            ShurikenProjectile shuriken = projectileInstance.GetComponent<ShurikenProjectile>();
            if (shuriken != null)
            {
                shuriken.Initialize(shotDirection, projectileSpeed, projectileDamage);
                continue;
            }

            // try BossProjectile
            BossProjectile bossProjectile = projectileInstance.GetComponent<BossProjectile>();
            if (bossProjectile != null)
            {
                bossProjectile.Initialize(shotDirection, projectileSpeed, gameObject);
                continue;
            }

            // try Fire
            Fire fireProjectile = projectileInstance.GetComponent<Fire>();
            if (fireProjectile != null)
            {
                fireProjectile.dx = shotDirection.x;
                fireProjectile.dy = shotDirection.y;
                continue;
            }

            // fallback - raw velocity
            Rigidbody2D projectileRb = projectileInstance.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
                projectileRb.linearVelocity = shotDirection * projectileSpeed;
        }

        facingDirection = direction;
        shootAnimationTimer = shootAnimationDuration;
        shotsFiredThisCycle++;
    }

    private void UpdateAnimation()
    {
        if (spriteRenderer == null || !isVisible) return;

        if (shootAnimationTimer > 0f) shootAnimationTimer -= Time.deltaTime;

        Vector2 animationDirection = facingDirection != Vector2.zero ? facingDirection : Vector2.down;
        Sprite[] nextFrames = shootAnimationTimer > 0f
            ? GetDirectionalFrames(shootUp, shootDown, shootLeft, shootRight, animationDirection)
            : GetDirectionalFrames(idleUp, idleDown, idleLeft, idleRight, animationDirection);

        if (nextFrames == null || nextFrames.Length == 0) return;

        if (currentAnimationFrames != nextFrames)
        {
            currentAnimationFrames = nextFrames;
            animationFrame = 0;
            animationTimer = 0f;
            spriteRenderer.sprite = currentAnimationFrames[animationFrame];
        }

        animationTimer += Time.deltaTime;
        if (animationTimer >= 1f / animationFrameRate)
        {
            animationTimer -= 1f / animationFrameRate;
            animationFrame = (animationFrame + 1) % currentAnimationFrames.Length;
            spriteRenderer.sprite = currentAnimationFrames[animationFrame];
        }
    }

    private static Sprite[] GetDirectionalFrames(Sprite[] upFrames, Sprite[] downFrames, Sprite[] leftFrames, Sprite[] rightFrames, Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x >= 0f ? rightFrames : leftFrames;
        return direction.y >= 0f ? upFrames : downFrames;
    }
}