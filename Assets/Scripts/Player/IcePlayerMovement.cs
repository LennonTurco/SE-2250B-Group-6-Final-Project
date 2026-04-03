using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class IcePlayerMovement : MonoBehaviour
{
    private Player player;
    private Vector2 moveInput;
    private bool isAttacking = false;

    [Header("Combat Settings")]
    [SerializeField] private GameObject tossedCoinPrefab;

    [Header("Ice Settings")]
    [SerializeField] private Tilemap iceTilemap;

    [Tooltip("How quickly speed builds up on ice (higher = less slippery feeling)")]
    [SerializeField] private float iceAcceleration = 5f;

    [Tooltip("How quickly speed builds up off ice (higher = more responsive)")]
    [SerializeField] private float normalAcceleration = 40f;

    [Tooltip("How fast the player slides to a stop on ice when no key is held")]
    [SerializeField] private float iceDrag = 0.6f;

    [Tooltip("How fast the player stops off ice when no key is held")]
    [SerializeField] private float normalDrag = 25f;

    [Tooltip("Top speed multiplier on ice relative to moveSpeed")]
    [SerializeField] private float iceMaxSpeedMultiplier = 1.5f;

    private bool isOnIce = false;

    void Start()
    {
        player = GetComponent<Player>();
        player.rb.linearDamping = normalDrag;
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        CheckIce();

        float acceleration = isOnIce ? iceAcceleration : normalAcceleration;
        float maxSpeed = player.moveSpeed * (isOnIce ? iceMaxSpeedMultiplier : 1f);

        // Always use force-based movement — smooth on both surfaces
        Vector2 targetVelocity = moveInput * maxSpeed;
        Vector2 velocityDiff = targetVelocity - player.rb.linearVelocity;
        Vector2 force = velocityDiff * acceleration;

        player.rb.AddForce(force, ForceMode2D.Force);

        // Clamp to max speed so ice doesn't let you go infinitely fast
        if (player.rb.linearVelocity.magnitude > maxSpeed)
        {
            player.rb.linearVelocity = player.rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void CheckIce()
    {
        if (iceTilemap == null) return;

        // Sample a small cross around the player's feet for reliable edge detection
        bool onIce = IsTileIce(transform.position)
                  || IsTileIce(transform.position + new Vector3( 0.1f, 0, 0))
                  || IsTileIce(transform.position + new Vector3(-0.1f, 0, 0))
                  || IsTileIce(transform.position + new Vector3(0,  0.1f, 0))
                  || IsTileIce(transform.position + new Vector3(0, -0.1f, 0));

        if (onIce != isOnIce)
        {
            isOnIce = onIce;
            player.rb.linearDamping = isOnIce ? iceDrag : normalDrag;
        }
    }

    private bool IsTileIce(Vector3 worldPos)
    {
        Vector3Int cell = iceTilemap.WorldToCell(worldPos);
        return iceTilemap.HasTile(cell);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            player.anim.SetBool("isWalking", false);
            player.anim.SetFloat("LastInputX", moveInput.x);
            player.anim.SetFloat("LastInputY", moveInput.y);
        }
        else
        {
            player.anim.SetBool("isWalking", true);
        }

        moveInput = context.ReadValue<Vector2>();
        player.anim.SetFloat("InputX", moveInput.x);
        player.anim.SetFloat("InputY", moveInput.y);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isAttacking = true;
            player.anim.SetBool("isAttacking", true);
            if (moveInput != Vector2.zero)
            {
                player.anim.SetFloat("LastInputX", moveInput.x);
                player.anim.SetFloat("LastInputY", moveInput.y);
            }
            player.rb.linearVelocity = Vector2.zero;
            SpawnTossedCoin();
        }
    }

    private void SpawnTossedCoin()
    {
        if (tossedCoinPrefab == null) return;

        Vector2 spawnDir = moveInput;
        if (spawnDir == Vector2.zero)
        {
            spawnDir = new Vector2(
                player.anim.GetFloat("LastInputX"),
                player.anim.GetFloat("LastInputY")
            );
            if (spawnDir == Vector2.zero) spawnDir = Vector2.down;
        }

        GameObject coinObj = Instantiate(tossedCoinPrefab, player.transform.position, Quaternion.identity);
        TossedCoin coin = coinObj.GetComponent<TossedCoin>();
        if (coin != null)
        {
            coin.dx = spawnDir.x;
            coin.dy = spawnDir.y;
            coin.collisionDamage = player.attackDamage;
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        player.anim.SetBool("isAttacking", false);
    }
}
