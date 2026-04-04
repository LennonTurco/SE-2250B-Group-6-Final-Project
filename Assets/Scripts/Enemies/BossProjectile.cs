using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float damage = 15f;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifetime = 5f;

    private Vector2 direction = Vector2.right;
    private Rigidbody2D rb;
    private GameObject owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
        else
        {
            transform.Translate(direction * speed * Time.fixedDeltaTime);
        }
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
            if (other.gameObject == owner || other.transform.root.gameObject == owner)
            {
                return;
            }
        }

        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.GetComponent<SolomonBoss>() != null) return;
        if (other.GetComponent<BossProjectile>() != null) return;

        Destroy(gameObject);
    }
}
