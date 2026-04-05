using UnityEngine;
using UnityEngine.UI;

public class LavaBoss : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Boss Rewards")]
    [SerializeField] private GameObject levelLoadZoneToEnable;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float roamRadius = 4f;
    [SerializeField] private float destinationThreshold = 0.25f;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private float pauseDuration = 0.75f;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform target;
    [SerializeField] private int projectilesPerShot = 5;
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private float fireInterval = 0.2f;
    [SerializeField] private float spreadAngle = 30f;

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

    [Header("Walk Sprites")]
    [SerializeField] private Sprite[] walkUp;
    [SerializeField] private Sprite[] walkDown;
    [SerializeField] private Sprite[] walkLeft;
    [SerializeField] private Sprite[] walkRight;

    [Header("Shoot Sprites")]
    [SerializeField] private Sprite[] shootUp;
    [SerializeField] private Sprite[] shootDown;
    [SerializeField] private Sprite[] shootLeft;
    [SerializeField] private Sprite[] shootRight;
    [SerializeField] private float shootAnimationDuration = 2f;

    private Rigidbody2D rb;
    private Vector2 spawnPosition;
    private Vector2 currentDestination;
    private Vector2 currentMoveDirection;
    private Vector2 facingDirection = Vector2.down;
    private float fireTimer;
    private float moveTimer;
    private float pauseTimer;
    private float shootAnimationTimer;
    private float animationTimer;
    private int animationFrame;
    private Sprite[] currentAnimationFrames;
    private float currentHealth;
    private bool isDead;

    [Header("UI")]
    public Slider healthBar;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        spawnPosition = transform.position;
        currentHealth = maxHealth;
        fireTimer = initialDelay > 0f ? initialDelay : fireInterval;
        AcquireTarget();

        if (firePoint == null)
        {
            firePoint = transform;
        }

        UpdateHealthBar();
        PickNewDestination();

        if (levelLoadZoneToEnable != null)
        {
            levelLoadZoneToEnable.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FireAtPlayer();
            fireTimer = fireInterval;
        }

        UpdateMovementState();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        MoveBoss();
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0f)
        {
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log($"LavaBoss took {amount} damage. Health: {currentHealth}/{maxHealth}");
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        currentMoveDirection = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        if (levelLoadZoneToEnable != null)
        {
            levelLoadZoneToEnable.SetActive(true);
        }

        Debug.Log("LavaBoss defeated.");
        Destroy(gameObject);
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null)
        {
            return;
        }

        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    private void UpdateMovementState()
    {
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            currentMoveDirection = Vector2.zero;
            return;
        }

        moveTimer -= Time.deltaTime;

        Vector2 currentPosition = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 toDestination = currentDestination - currentPosition;

        if (toDestination.sqrMagnitude <= destinationThreshold * destinationThreshold || moveTimer <= 0f)
        {
            pauseTimer = pauseDuration;
            PickNewDestination();
            currentMoveDirection = Vector2.zero;
            return;
        }

        currentMoveDirection = toDestination.normalized;
        facingDirection = currentMoveDirection;
    }

    private void MoveBoss()
    {
        if (currentMoveDirection == Vector2.zero)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        if (rb != null)
        {
            rb.linearVelocity = currentMoveDirection * moveSpeed;
            return;
        }

        transform.position += (Vector3)(currentMoveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void PickNewDestination()
    {
        Vector2 randomOffset = Random.insideUnitCircle * roamRadius;
        currentDestination = spawnPosition + randomOffset;
        moveTimer = moveDuration;
    }

    private void AcquireTarget()
    {
        if (target != null)
        {
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
            return;
        }

        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            target = player.transform;
        }
    }

    private void FireAtPlayer()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("LavaBoss: projectilePrefab is not assigned.");
            return;
        }

        AcquireTarget();

        if (target == null)
        {
            Debug.LogWarning("LavaBoss: could not find a Player target.");
            return;
        }

        Vector2 direction = ((Vector2)target.position - (Vector2)firePoint.position).normalized;
        if (direction == Vector2.zero)
        {
            return;
        }

        int shotCount = Mathf.Max(1, projectilesPerShot);
        float totalSpread = Mathf.Max(0f, spreadAngle);
        float startAngle = -totalSpread * 0.5f;
        float angleStep = shotCount > 1 ? totalSpread / (shotCount - 1) : 0f;

        for (int i = 0; i < shotCount; i++)
        {
            float angleOffset = startAngle + (angleStep * i);
            Vector2 shotDirection = Quaternion.Euler(0f, 0f, angleOffset) * direction;

            GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            BossProjectile bossProjectile = projectileInstance.GetComponent<BossProjectile>();
            if (bossProjectile != null)
            {
                bossProjectile.Initialize(shotDirection, projectileSpeed, gameObject);
                continue;
            }

            Fire fireProjectile = projectileInstance.GetComponent<Fire>();
            if (fireProjectile != null)
            {
                fireProjectile.dx = shotDirection.x;
                fireProjectile.dy = shotDirection.y;
                continue;
            }

            Rigidbody2D projectileRb = projectileInstance.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = shotDirection * projectileSpeed;
            }
        }

        facingDirection = direction;
        shootAnimationTimer = shootAnimationDuration;
    }

    private void UpdateAnimation()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (shootAnimationTimer > 0f)
        {
            shootAnimationTimer -= Time.deltaTime;
        }

        Vector2 animationDirection = facingDirection != Vector2.zero ? facingDirection : Vector2.down;
        bool isMoving = currentMoveDirection.sqrMagnitude > 0.001f;

        Sprite[] nextFrames;
        if (shootAnimationTimer > 0f)
        {
            nextFrames = GetDirectionalFrames(shootUp, shootDown, shootLeft, shootRight, animationDirection);
        }
        else if (isMoving)
        {
            nextFrames = GetDirectionalFrames(walkUp, walkDown, walkLeft, walkRight, animationDirection);
        }
        else
        {
            nextFrames = GetDirectionalFrames(idleUp, idleDown, idleLeft, idleRight, animationDirection);
        }

        if (nextFrames == null || nextFrames.Length == 0)
        {
            return;
        }

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
        {
            return direction.x >= 0f ? rightFrames : leftFrames;
        }

        return direction.y >= 0f ? upFrames : downFrames;
    }
}
