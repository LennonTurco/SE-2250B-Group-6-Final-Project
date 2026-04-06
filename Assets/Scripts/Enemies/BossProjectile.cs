using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 15f;
    [SerializeField] private float lifetime = 5f;

    private Vector2 direction = Vector2.right;
    private float speed = 6f;
    private Rigidbody2D rb;
    private GameObject owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.bodyType = RigidbodyType2D.Dynamic;; // dynamic so triggers fire correctly
    }

    private void Start()
    {
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector2 moveDirection, float moveSpeed)
    {
        direction = moveDirection.normalized;
        speed = moveSpeed;
    }

    public void Initialize(Vector2 moveDirection, float moveSpeed, GameObject projectileOwner)
    {
        Initialize(moveDirection, moveSpeed);
        owner = projectileOwner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner != null)
        {
            if (other.gameObject == owner || other.transform.root.gameObject == owner) return;
        }

        if (other.GetComponent<SolomonBoss>() != null) return;
        if (other.GetComponent<BossProjectile>() != null) return;

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}