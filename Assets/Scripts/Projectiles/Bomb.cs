using UnityEngine;

public class Bomb : Entity
{
    [Header("Bomb Settings")]
    [SerializeField] private GameObject firePrefab; // Drag the Fire prefab here in the Inspector
    [SerializeField] public float fireDistance = 5f; // How far it moves before stopping
    [SerializeField] private float lifetime = 5f;
    
    [Header("Direction")]
    public float dx;
    public float dy;

    private float decayDuration = 0.5f;
    private float decayTimer = 0f;
    private Vector2 initialVelocity;
    private bool hasExploded = false;

    protected override void Awake()
    {
        base.Awake();
        
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; 
        rb.bodyType = RigidbodyType2D.Dynamic; 

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Intangible: no collision damage
        collisionDamage = 0f;
    }

    protected override void Start()
    {
        base.Start();
        
        Vector2 moveDir = new Vector2(dx, dy);
        
        // If no direction is specified in dx/dy, try to inherit it from existing velocity (e.g. if set by Boss)
        if (moveDir == Vector2.zero && rb.linearVelocity != Vector2.zero)
        {
            moveDir = rb.linearVelocity.normalized;
        }

        if (moveDir != Vector2.zero)
        {
            // To cover 'fireDistance' in 0.5s with linear decay, initial V0 = 4 * distance
            initialVelocity = moveDir.normalized * (fireDistance * 4f);
            rb.linearVelocity = initialVelocity;
        }
        else
        {
            initialVelocity = Vector2.zero;
        }

        // Safety cleanup
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (hasExploded) return;

        decayTimer += Time.deltaTime;
        float progress = Mathf.Clamp01(decayTimer / decayDuration);

        // Linearly decay velocity to zero
        rb.linearVelocity = initialVelocity * (1f - progress);

        if (progress >= 1f)
        {
            Invoke(nameof(Explode), attackCooldown);
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        
        rb.linearVelocity = Vector2.zero;

        // Perform the 4-way fire burst sequence
        ShootFireFourWay();
        Invoke(nameof(ShootFireFourWay), 0.2f);
        Invoke(nameof(ShootFireFourWay), 0.4f);
        
        // Self-destruct after the bursts are complete
        Invoke(nameof(DestroySelf), 0.5f);
    }

    private void DestroySelf()
    {
        Die();
    }

    private void ShootFire(float fireDx, float fireDy)
    {
        if (firePrefab == null)
        {
            return;
        }

        GameObject fireObj = Instantiate(firePrefab, transform.position, Quaternion.identity);
        Fire fireScript = fireObj.GetComponent<Fire>();

        if (fireScript != null)
        {
            fireScript.dx = fireDx;
            fireScript.dy = fireDy;
        }
    }

    private void ShootFireFourWay()
    {
        ShootFire(1, 0);
        ShootFire(-1, 0);
        ShootFire(0, 1);
        ShootFire(0, -1);
    }
}
