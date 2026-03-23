using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Base Attributes")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float attackDamage = 10f;
    [SerializeField] public float attackCooldown = 1f;

    [Header("State")]
    public float currentHealth;
    public bool isDead = false;
    public Vector2 facingDirection = Vector2.down;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log($"{gameObject.name} has died.");
        // Common death logic (disable collisions, play animation, etc.)
    }

    public virtual void SetFacingDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            facingDirection = direction.normalized;
        }
    }
}
