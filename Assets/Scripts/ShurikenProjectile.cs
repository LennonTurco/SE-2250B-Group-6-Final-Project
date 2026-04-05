using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShurikenProjectile : MonoBehaviour
{
    private float damage = 10f;
    private float speed = 8f;
    private Vector2 direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Start()
    {
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, 5f);
    }

    public void Initialize(Vector2 dir, float moveSpeed, float dmg)
    {
        direction = dir.normalized;
        speed = moveSpeed;
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NinjaEnemy>() != null) return;
        if (other.GetComponent<ShurikenProjectile>() != null) return;

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