using UnityEngine;

public class Player : Entity
{
    // ── private ──────────────────────────────
    private Vector3 spawnPoint;
    public int numCharactersUnlocked = 1;

    protected override void Awake()
    {
        base.Awake();
        PlayerPrefs.DeleteAll();
    }

    protected override void Start()
    {
        base.Start();

        // find and store spawn point
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
            spawnPoint = spawn.transform.position;
        else
            spawnPoint = transform.position;

        LoadStats(); // load persisted stats before anything else

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
            string stat   = PlayerPrefs.GetString("PendingStat_" + i);
            float  amount = PlayerPrefs.GetFloat("PendingAmount_" + i);
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

        // stop movement
        if (rb != null) rb.linearVelocity = Vector2.zero;

        HUDManager.Instance?.RefreshHUD();
        Debug.Log("[Player] Respawned at " + spawnPoint);
    }

    // called by shop on purchase - applies stat boost immediately
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

        SaveStats(); // persist after every upgrade
        Debug.Log("[Player] Applied " + stat + " +" + amount);
    }

    // ── stat persistence ─────────────────────

    public void SaveStats()
    {
        PlayerPrefs.SetFloat("Stat_MoveSpeed",      moveSpeed);
        PlayerPrefs.SetFloat("Stat_AttackDamage",   attackDamage);
        PlayerPrefs.SetFloat("Stat_MaxHP",          maxHealth);
        PlayerPrefs.SetFloat("Stat_AttackCooldown", attackCooldown);
        PlayerPrefs.SetInt("Stat_NumCharactersUnlocked", numCharactersUnlocked);
    }

    private void LoadStats()
    {
        if (PlayerPrefs.HasKey("Stat_MoveSpeed"))
            moveSpeed      = PlayerPrefs.GetFloat("Stat_MoveSpeed");
        if (PlayerPrefs.HasKey("Stat_AttackDamage"))
            attackDamage   = PlayerPrefs.GetFloat("Stat_AttackDamage");
        if (PlayerPrefs.HasKey("Stat_MaxHP"))
            maxHealth      = PlayerPrefs.GetFloat("Stat_MaxHP");
        if (PlayerPrefs.HasKey("Stat_AttackCooldown"))
            attackCooldown = PlayerPrefs.GetFloat("Stat_AttackCooldown");
        if (PlayerPrefs.HasKey("Stat_NumCharactersUnlocked"))
            numCharactersUnlocked = PlayerPrefs.GetInt("Stat_NumCharactersUnlocked");
    }

    public bool SpendGold(int amount)
    {
        return HUDManager.SpendGoldFromTotal(amount); // delegate to hud
    }
}