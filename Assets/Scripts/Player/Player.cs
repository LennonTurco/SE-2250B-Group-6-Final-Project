using UnityEngine;

public class Player : Entity
{
    // ── private ──────────────────────────────
    private Vector3 spawnPoint;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        // find and store spawn point
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
            spawnPoint = spawn.transform.position;
        else
            spawnPoint = transform.position; // fallback to starting pos

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

        // refresh hud now that player is fully initialized
        HUDManager.Instance?.RefreshHUD();
    }

    protected override void Die()
    {
        // don't call base.Die() as it destroys the gameobject
        if (isDead) return;
        isDead = true;
        Debug.Log("[Player] Died - respawning");
        Respawn();
    }

    private void Respawn()
    {
        // reset state
        isDead = false;
        currentHealth = maxHealth;
        transform.position = spawnPoint;

        // re-enable components
        if (rb != null) rb.linearVelocity = Vector2.zero;

        HUDManager.Instance?.RefreshHUD();
        Debug.Log("[Player] Respawned at " + spawnPoint);
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