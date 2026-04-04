using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class GateController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private GameObject lockedMessageUI;

    [Header("Gate State")]
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private bool disableColliderWhenOpened = true;

    [Header("Scene Transition")]
    [SerializeField] private string targetSceneName = "Lava Boss Scene";

    private SpriteRenderer spriteRenderer;
    private Collider2D gateCollider;
    private TMP_Text lockedMessageText;
    private Text legacyLockedMessageText;
    private bool playerNearby;
    private bool opened;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gateCollider = GetComponent<Collider2D>();

        if (lockedMessageUI != null)
        {
            lockedMessageText = lockedMessageUI.GetComponent<TMP_Text>();
            if (lockedMessageText == null)
                lockedMessageText = lockedMessageUI.GetComponentInChildren<TMP_Text>(true);

            if (lockedMessageText == null)
            {
                legacyLockedMessageText = lockedMessageUI.GetComponent<Text>();
                if (legacyLockedMessageText == null)
                    legacyLockedMessageText = lockedMessageUI.GetComponentInChildren<Text>(true);
            }
        }
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
            UpdateLockedMessage();

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

        if (!string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.Log($"[GateController] Loading scene '{targetSceneName}'.");
            SceneManager.LoadScene(targetSceneName);
            return;
        }

        Debug.Log("[GateController] Gate opened.");
    }

    private void UpdateLockedMessage()
    {
        if (ScrollManager.Instance == null)
            return;

        string message = $"Gate Locked: {ScrollManager.Instance.CurrentScrolls}/{ScrollManager.Instance.RequiredScrolls} Scrolls";

        if (lockedMessageText != null)
            lockedMessageText.text = message;

        if (legacyLockedMessageText != null)
            legacyLockedMessageText.text = message;
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
