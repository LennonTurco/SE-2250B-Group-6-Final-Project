using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ChestController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject promptUI;

    [Header("Rewards")]
    [SerializeField] private int goldAmount = 10;
    [SerializeField] private int scrollAmount = 1;

    [Header("Sprites")]
    [SerializeField] private Sprite openedSprite;

    private SpriteRenderer spriteRenderer;
    private bool playerNearby;
    private bool opened;

    

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (opened || !playerNearby || Keyboard.current == null)
            return;

        if (Keyboard.current.xKey.wasPressedThisFrame)
            OpenChest();
    }

    private void OpenChest()
    {
        opened = true;

        if (promptUI != null)
            promptUI.SetActive(false);

        if (openedSprite != null)
            spriteRenderer.sprite = openedSprite;

        HUDManager.AddGoldToTotal(goldAmount);

        if (ScrollManager.Instance != null)
            ScrollManager.Instance.AddScroll(scrollAmount);
        else
            Debug.LogWarning("[ChestController] No ScrollManager found in the scene.");

        Debug.Log($"[ChestController] Opened {gameObject.name}. Awarded {goldAmount} gold and {scrollAmount} scroll.");
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
