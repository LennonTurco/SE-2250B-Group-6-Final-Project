using UnityEngine;

public class JoseTheBoss : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject iciclePrefab;
    [SerializeField] private float shootCooldown = 1.5f;
    [SerializeField] private float spawnOffset = 0.8f;

    [Header("Boss Settings")]
    [SerializeField] private int hitsToDefeat = 5;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject levelLoadZoneToEnable;

    [Header("Activation")]
    [SerializeField] private float activationYThreshold = 40f;

    private float shootTimer = 0f;
    private Transform playerTransform;
    private int hitCount = 0;
    private bool defeated = false;
    private bool playerInContact = false;
    private bool activated = false;

    private void Start()
    {
        shootTimer = shootCooldown;

        Player player = FindFirstObjectByType<Player>();
        if (player != null)
            playerTransform = player.transform;

        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (levelLoadZoneToEnable != null) levelLoadZoneToEnable.SetActive(false);
    }

    private void Update()
    {
        if (defeated) return;
        if (playerTransform == null) return;

        if (!activated)
        {
            if (playerTransform.position.y >= activationYThreshold)
            {
                activated = true;
                // Advance objective to defeat boss
                IceHUDManager.Instance?.SetObjective(IceHUDManager.Objective.DefeatBoss);
                Debug.Log("[Jose] Activated!");
            }
            return;
        }

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootCooldown)
        {
            shootTimer = 0f;
            ShootIcicle();
        }
    }

    private void ShootIcicle()
    {
        if (iciclePrefab == null)
        {
            Debug.LogWarning("JoseTheBoss: Icicle prefab not assigned!");
            return;
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        Vector3 spawnPos = transform.position + (Vector3)(direction * spawnOffset);

        GameObject icicleObj = Instantiate(iciclePrefab, spawnPos, Quaternion.identity);
        Icicle icicle = icicleObj.GetComponent<Icicle>();
        if (icicle != null)
            icicle.Launch(direction, gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (defeated) return;
        if (!other.CompareTag("Player")) return;
        if (playerInContact) return;

        playerInContact = true;
        hitCount++;
        Debug.Log("[Jose] Hit " + hitCount + "/" + hitsToDefeat);

        if (hitCount >= hitsToDefeat)
            Defeat();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInContact = false;
    }

    private void Defeat()
    {
        defeated = true;
        Debug.Log("[Jose] Defeated!");

        if (defeatPanel != null) defeatPanel.SetActive(true);
        if (levelLoadZoneToEnable != null) levelLoadZoneToEnable.SetActive(true);

        Destroy(gameObject, 2f);
    }
}
