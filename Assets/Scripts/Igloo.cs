using UnityEngine;

public class Igloo : MonoBehaviour
{
    [Header("Igloo Settings")]
    [SerializeField] private GameObject snowballPrefab;
    [SerializeField] private float shootCooldown = 2.5f;
    [SerializeField] private float spawnOffset = 1.0f; // how far outside igloo to spawn

    private float shootTimer = 0f;
    private Transform playerTransform;

    private void Start()
    {
        shootTimer = shootCooldown;

        Player player = FindFirstObjectByType<Player>();
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("Igloo: Could not find Player in scene!");
    }

    private void Update()
    {
        if (playerTransform == null) return;

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootCooldown)
        {
            shootTimer = 0f;
            ShootSnowball();
        }
    }

    private void ShootSnowball()
    {
        if (snowballPrefab == null)
        {
            Debug.LogWarning("Igloo: Snowball Prefab is not assigned!");
            return;
        }

        Vector2 direction = (playerTransform.position - transform.position).normalized;

        // Spawn the snowball offset outside the igloo so it never touches the igloo collider
        Vector3 spawnPos = transform.position + (Vector3)(direction * spawnOffset);

        GameObject snowballObj = Instantiate(snowballPrefab, spawnPos, Quaternion.identity);
        Snowball snowball = snowballObj.GetComponent<Snowball>();

        if (snowball != null)
        {
            snowball.Launch(direction, gameObject);
        }
    }
}