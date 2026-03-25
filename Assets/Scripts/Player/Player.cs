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
    // called by shop on purchase to immediately apply stat boost
    public void ApplyStat(string stat, float amount)
    {
        switch (stat)
        {
            case "MoveSpeed":
                moveSpeed += amount;
                break;
            case "AttackDamage":
                attackDamage += amount;
                break;
            case "MaxHP":
                maxHealth += amount;
                currentHealth += amount; // give hp immediately too
                break;
            case "AttackCooldown":
                attackCooldown = Mathf.Max(0.1f, attackCooldown - amount); // lower = faster
                break;
            default:
                Debug.LogWarning("[Player] Unknown stat: " + stat);
                break;
        }

        Debug.Log("[Player] Applied " + stat + " +" + amount);
    }

    public bool SpendGold(int amount)
    {
        return HUDManager.SpendGoldFromTotal(amount); // delegate to hud
    }
}
