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

    public void Initialize(Vector2 dir, float moveSpeed, float dmg)
    {
        direction = dir.normalized;
        speed = moveSpeed;
        damage = dmg;

        // set velocity immediately so it doesn't wait for Start
        if (rb != null) rb.linearVelocity = direction * speed;
    }

    private void Start()
    {
        // fallback if Initialize wasn't called before Start
        if (rb.linearVelocity == Vector2.zero && direction != Vector2.zero)
            rb.linearVelocity = direction * speed;

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NinjaEnemy>() != null) return;
        if (other.GetComponent<JungleBoss>() != null) return;
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