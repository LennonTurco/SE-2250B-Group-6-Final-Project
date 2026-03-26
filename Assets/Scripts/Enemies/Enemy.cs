using UnityEngine;

public abstract class Enemy : Entity
{
    protected Transform target;

    [SerializeField] protected int goldDrop = 5; // set per enemy in inspector

    protected virtual void Update()
    {
        HandleAI();
    }

    protected abstract void HandleAI();

    protected override void Die()
    {
        HUDManager.AddGoldToTotal(goldDrop); // drop gold on death
        base.Die();
    }
}