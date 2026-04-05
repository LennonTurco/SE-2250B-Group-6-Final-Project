using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class GateController : MonoBehaviour
{
    private const string LavaNpcTextTag = "LavaNPCTXT";

    [Header("UI")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private GameObject lavaNpcTextObject;
    [SerializeField] [TextArea] private string interactionMessage = "You need to collect the required scrolls before entering.";
    [SerializeField] private float interactionMessageDuration = 10f;

    [Header("Gate State")]
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private bool disableColliderWhenOpened = true;

    [Header("Scene Transition")]
    [SerializeField] private string targetSceneName = "Lava Boss Scene";

    private SpriteRenderer spriteRenderer;
    private Collider2D gateCollider;
    private TMP_Text lavaNpcText;
    private Text legacyLavaNpcText;
    private bool playerNearby;
    private bool opened;
    private float interactionMessageTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gateCollider = GetComponent<Collider2D>();

        CacheLavaNpcText();
    }

    private void Update()
    {
        UpdateInteractionMessageTimer();

        if (opened || !playerNearby || Keyboard.current == null)
            return;

        if (Keyboard.current.xKey.wasPressedThisFrame)
            TryOpenGate();
    }

    private void TryOpenGate()
    {
        if (ScrollManager.Instance == null)
        {
            ShowInteractionMessage();
            Debug.LogWarning("[GateController] No ScrollManager found in the scene.");
            return;
        }

        if (!ScrollManager.Instance.HasEnoughScrolls())
        {
            ShowInteractionMessage();

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

    private void CacheLavaNpcText()
    {
        GameObject targetObject = lavaNpcTextObject;
        if (targetObject == null)
        {
            try
            {
                targetObject = GameObject.FindGameObjectWithTag(LavaNpcTextTag);
            }
            catch (UnityException)
            {
                Debug.LogWarning($"[GateController] Tag '{LavaNpcTextTag}' is not defined. Assign the lava NPC text object directly in the Inspector.");
                return;
            }
        }

        if (targetObject == null)
        {
            Debug.LogWarning($"[GateController] No object found for lava NPC text. Assign it in the Inspector or use tag '{LavaNpcTextTag}'.");
            return;
        }

        lavaNpcText = targetObject.GetComponent<TMP_Text>();
        if (lavaNpcText == null)
            lavaNpcText = targetObject.GetComponentInChildren<TMP_Text>(true);

        if (lavaNpcText == null)
        {
            legacyLavaNpcText = targetObject.GetComponent<Text>();
            if (legacyLavaNpcText == null)
                legacyLavaNpcText = targetObject.GetComponentInChildren<Text>(true);
        }
    }

    private void ShowInteractionMessage()
    {
        if (string.IsNullOrWhiteSpace(interactionMessage))
            return;

        if (lavaNpcText == null && legacyLavaNpcText == null)
            CacheLavaNpcText();

        if (lavaNpcText != null)
        {
            EnsureUiHierarchyVisible(lavaNpcText.transform);
            lavaNpcText.text = interactionMessage;
        }

        if (legacyLavaNpcText != null)
        {
            EnsureUiHierarchyVisible(legacyLavaNpcText.transform);
            legacyLavaNpcText.text = interactionMessage;
        }

        interactionMessageTimer = interactionMessageDuration;
    }

    private void UpdateInteractionMessageTimer()
    {
        if (interactionMessageTimer <= 0f)
            return;

        interactionMessageTimer -= Time.deltaTime;
        if (interactionMessageTimer > 0f)
            return;

        ClearInteractionMessage();
    }

    private void ClearInteractionMessage()
    {
        interactionMessageTimer = 0f;

        if (lavaNpcText != null)
            lavaNpcText.text = string.Empty;

        if (legacyLavaNpcText != null)
            legacyLavaNpcText.text = string.Empty;
    }

    private void EnsureUiHierarchyVisible(Transform targetTransform)
    {
        Transform current = targetTransform;
        while (current != null)
        {
            current.gameObject.SetActive(true);

            Canvas canvas = current.GetComponent<Canvas>();
            if (canvas != null)
                canvas.enabled = true;

            current = current.parent;
        }
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
    }
}
