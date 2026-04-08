using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class IcePlayerMovement : PlayerMovement
{
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

    // at initialization, create player object and sets normal drag
    protected override void Start()
    {
        base.Start();
        player.rb.linearDamping = normalDrag;
    }

    protected override void Update()
    {
        // Don't call base.Update() because we use force-based movement in FixedUpdate instead
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        CheckIce();

        // if the player is on an ice tile, the acceleration and max speed is increased
        float acceleration = isOnIce ? iceAcceleration : normalAcceleration;
        float maxSpeed = isOnIce ? Mathf.Min(player.moveSpeed, 5f) * iceMaxSpeedMultiplier : player.moveSpeed;

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

    // checks if the player is on an ice tile
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
}
