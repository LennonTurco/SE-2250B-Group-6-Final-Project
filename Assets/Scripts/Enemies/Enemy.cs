using UnityEngine;

public abstract class Enemy : Entity
{
    public static bool isPaused = false; // Global

    protected Transform target;

    [SerializeField] protected int goldDrop = 5; // set per enemy in inspector

    protected virtual void Update()
    {
        if (isPaused) return;
        HandleAI();
    }

    protected abstract void HandleAI();

    protected override void Die()
    {
        HUDManager.AddGoldToTotal(goldDrop); // drop gold on death
        base.Die();
    }
}