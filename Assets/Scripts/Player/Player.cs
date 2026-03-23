using UnityEngine;

public class Player : Entity
{
    protected override void Awake()
    {
        base.Awake();
        // Player specific initialization
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Die()
    {
        base.Die();
        // Player specific death (game over screen, respawn, etc.)
    }

    // Add other player specific methods here
}
