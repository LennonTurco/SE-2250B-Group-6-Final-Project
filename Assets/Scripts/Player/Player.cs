using UnityEngine;

public class Player : Entity
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        // restore position if returning from shop
        if (PlayerPrefs.HasKey("PlayerX"))
        {
            transform.position = new Vector3(
                PlayerPrefs.GetFloat("PlayerX"),
                PlayerPrefs.GetFloat("PlayerY"),
                0f
            );
            PlayerPrefs.DeleteKey("PlayerX");
            PlayerPrefs.DeleteKey("PlayerY");
        }

        // apply all pending shop upgrades
        int count = PlayerPrefs.GetInt("PendingCount", 0);
        for (int i = 0; i < count; i++)
        {
            string stat = PlayerPrefs.GetString("PendingStat_" + i);
            float amount = PlayerPrefs.GetFloat("PendingAmount_" + i);
            ApplyStat(stat, amount);
            PlayerPrefs.DeleteKey("PendingStat_" + i);
            PlayerPrefs.DeleteKey("PendingAmount_" + i);
        }
        PlayerPrefs.DeleteKey("PendingCount");
    }

    protected override void Die()
    {
        base.Die();
        // player death (game over, respawn, etc.)
    }

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
                currentHealth += amount;
                break;
            case "AttackCooldown":
                attackCooldown = Mathf.Max(0.1f, attackCooldown - amount);
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