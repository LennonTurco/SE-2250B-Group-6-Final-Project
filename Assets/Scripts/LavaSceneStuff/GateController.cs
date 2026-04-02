using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class GateController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private GameObject lockedMessageUI;

    [Header("Gate State")]
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private bool disableColliderWhenOpened = true;

    private SpriteRenderer spriteRenderer;
    private Collider2D gateCollider;
    private bool playerNearby;
    private bool opened;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gateCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (opened || !playerNearby || Keyboard.current == null)
            return;

        if (Keyboard.current.xKey.wasPressedThisFrame)
            TryOpenGate();
    }

    private void TryOpenGate()
    {
        if (ScrollManager.Instance == null)
        {
            Debug.LogWarning("[GateController] No ScrollManager found in the scene.");
            return;
        }

        if (!ScrollManager.Instance.HasEnoughScrolls())
        {
            if (lockedMessageUI != null)
                lockedMessageUI.SetActive(true);

            Debug.Log($"[GateController] Gate locked. Need {ScrollManager.Instance.RequiredScrolls} scrolls.");
            return;
        }

        OpenGate();
    }

    private void OpenGate()
    {
        opened = true;

        if (promptUI != null)
            promptUI.SetActive(false);

        if (lockedMessageUI != null)
            lockedMessageUI.SetActive(false);

        if (spriteRenderer != null && openedSprite != null)
            spriteRenderer.sprite = openedSprite;

        if (gateCollider != null && disableColliderWhenOpened)
            gateCollider.enabled = false;

        Debug.Log("[GateController] Gate opened.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = true;

        if (promptUI != null && !opened)
            promptUI.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = false;

        if (promptUI != null)
            promptUI.SetActive(false);

        if (lockedMessageUI != null)
            lockedMessageUI.SetActive(false);
    }
}
