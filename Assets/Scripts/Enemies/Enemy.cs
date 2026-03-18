protected int health;
protected int damage;
protected float speed;

public virtual void TakeDamage(int amount)
{
    health -= amount;
    if (health <= 0) Die();
}

protected virtual void Die()
{
    Destroy(gameObject);
}