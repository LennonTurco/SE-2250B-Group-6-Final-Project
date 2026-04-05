using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Base Attributes")]
    [SerializeField] protected float maxHealth = 100f;
    // getter 
    public float MaxHealth => maxHealth;
    
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float attackDamage = 10f;
    [SerializeField] public float attackCooldown = 1f;
    [SerializeField] public float invulTime = 0f;
    [SerializeField] public bool isInvul = false;
    [SerializeField] public float collisionDamage = 0f;


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
        if (isInvul || isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // refresh the hud if its for the player
        if (this is Player)
        {
            HUDManager.Instance?.RefreshHUD();
        }
            
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        if(gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has died. L!");
        }

        // FILLER for now
        Destroy(gameObject);

        // Common death logic (disable collisions, play animation, etc.)
    }

    public virtual void SetFacingDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            facingDirection = direction.normalized;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        HandleCollision(collision.gameObject, collision.isTrigger);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject, false);
    }

    protected virtual void HandleCollision(GameObject obj, bool isTrigger)
    {
        if (collisionDamage > 0f)
        {
            Entity other = obj.GetComponentInParent<Entity>();
            if (other != null && other != this)
            {
                other.TakeDamage(collisionDamage);
            }
        }
    }
}
