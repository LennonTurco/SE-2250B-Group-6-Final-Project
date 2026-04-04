using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LavaHazard : MonoBehaviour
{
    [Header("Burn Settings")]
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private float damagePerTick = 20f;
    [SerializeField] private float tickInterval = 1f;

    private readonly Dictionary<Player, Coroutine> activeBurns = new Dictionary<Player, Coroutine>();
    private readonly Dictionary<Player, float> burnEndTimes = new Dictionary<Player, float>();

    private void Awake()
    {
        Collider2D hazardCollider = GetComponent<Collider2D>();
        if (hazardCollider != null)
        {
            hazardCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryIgnite(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryIgnite(collision.gameObject);
    }

    private void TryIgnite(GameObject otherObject)
    {
        Player player = otherObject.GetComponentInParent<Player>();
        if (player == null)
        {
            return;
        }

        float newBurnEndTime = Time.time + Mathf.Max(burnDuration, tickInterval);

        if (burnEndTimes.TryGetValue(player, out float existingEndTime))
        {
            burnEndTimes[player] = Mathf.Max(existingEndTime, newBurnEndTime);
            return;
        }

        burnEndTimes[player] = newBurnEndTime;
        activeBurns[player] = StartCoroutine(BurnPlayer(player));
    }

    private IEnumerator BurnPlayer(Player player)
    {
        Debug.Log($"{player.name} was lit on fire.");

        PlayerBurnVisual burnVisual = player.GetComponentInChildren<PlayerBurnVisual>(true);
        if (burnVisual != null)
        {
            burnVisual.ShowBurning();
        }

        while (player != null && !player.isDead)
        {
            player.TakeDamage(damagePerTick);

            if (!burnEndTimes.TryGetValue(player, out float burnEndTime))
            {
                break;
            }

            if (Time.time >= burnEndTime)
            {
                break;
            }

            yield return new WaitForSeconds(tickInterval);
        }

        if (burnVisual != null)
        {
            burnVisual.HideBurning();
        }

        burnEndTimes.Remove(player);
        activeBurns.Remove(player);
    }
}
