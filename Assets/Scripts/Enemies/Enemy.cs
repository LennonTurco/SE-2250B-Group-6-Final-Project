using UnityEngine;
public abstract class Enemy : Entity
{
    protected Transform target;

    protected virtual void Update()
    {
        HandleAI();
    }

    protected abstract void HandleAI();
}
