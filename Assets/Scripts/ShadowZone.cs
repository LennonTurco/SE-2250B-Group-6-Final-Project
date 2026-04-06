using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShadowZone : MonoBehaviour
{
    private void Reset()
    {
        Collider2D triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerShadowStealth.Instance?.EnterShadow();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerShadowStealth.Instance?.ExitShadow();
    }
}
