using UnityEngine;

public class AntunaBoss : Enemy
{
    [Header("AntunaBoss Settings")]
    [SerializeField] private GameObject bombPrefab; // Drag the Fire prefab here in the Inspector
    [SerializeField] private float fireRange = 12f;
    [SerializeField] private float teleportCooldown = 1f;
    
    private float bombTimer = 0f;
    private float teleportTimer = 0f;
    private bool isTeleporting = false;

    protected override void Start()
    {
        base.Start();
        
        // Find the player specifically
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            target = player.transform;
        }

        teleportTimer = teleportCooldown; // Initial cooldown
    }

    protected override void HandleAI()
    {
        if (target == null || isTeleporting) return;

        // Teleport timing
        teleportTimer -= Time.deltaTime;
        if (teleportTimer <= 0f)
        {
            teleportTimer = teleportCooldown;
            StartCoroutine(TeleportToRandomPosition());
            return; // Skip other AI while starting teleport
        }

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        if (distanceToPlayer > fireRange)
        {
            bombTimer = 0f;
            return;
        }

        bombTimer += Time.deltaTime;

        // 750 total health
        if(currentHealth > 600)
        {
            if (bombTimer >= attackCooldown*1.5)
            {
                bombTimer = 0f;
                ShootBomb();
            }
        }
        else if(currentHealth > 450)
        {
            if (bombTimer >= attackCooldown*1.5)
            {
                bombTimer = 0f;
                SpawnRandomBomb();
                SpawnRandomBomb();
            }

        }
        else if(currentHealth > 300)
        {
            if (bombTimer >= attackCooldown)
            {
                bombTimer = 0f;
                ShootBomb();
            }
        }
        else if(currentHealth > 150) {
            if (bombTimer >= attackCooldown)
            {
                bombTimer = 0f;
                SpawnRandomBomb();
                SpawnRandomBomb();
            }
        }
        else if(currentHealth > 0) {
            if (bombTimer >= attackCooldown*0.7)
            {
                bombTimer = 0f;
                ShootBomb();
            }
        }
    }

    private System.Collections.IEnumerator TeleportToRandomPosition()
    {
        isTeleporting = true;
        
        // Pick random position in specified range
        float targetX = Random.Range(-17f, -6f);
        float targetY = Random.Range(40f, 48f);
        Vector2 targetPos = new Vector2(targetX, targetY);
        Vector2 startPos = transform.position;
        
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Linear movement
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            
            yield return null;
        }

        transform.position = targetPos;
        isTeleporting = false;
    }

    private void ShootBomb()
    {
        if (bombPrefab == null)
        {
            Debug.LogWarning("AntunaBoss: Bomb Prefab is not assigned!");
            return;
        }

        GameObject bombObj = Instantiate(bombPrefab, transform.position, Quaternion.identity);
        Bomb bombScript = bombObj.GetComponent<Bomb>();

        if (bombScript != null)
        {
            // Calculate direction to the target
            Vector2 direction = (target.position - transform.position).normalized;
            
            // Set the dx and dy on the bomb script
            bombScript.dx = direction.x;
            bombScript.dy = direction.y;
            bombScript.fireDistance = Random.Range(2, 7);
        }
    }

    private void SpawnRandomBomb() {
        if (bombPrefab == null)
        {
            Debug.LogWarning("AntunaBoss: Bomb Prefab is not assigned!");
            return;
        }

        float targetX = Random.Range(-17f, -6f);
        float targetY = Random.Range(40f, 48f);
        Vector2 targetPos = new Vector2(targetX, targetY);

        GameObject bombObj = Instantiate(bombPrefab, targetPos, Quaternion.identity);
        Bomb bombScript = bombObj.GetComponent<Bomb>();
    }

    // -17 to -6
    // 40 to 48
}
