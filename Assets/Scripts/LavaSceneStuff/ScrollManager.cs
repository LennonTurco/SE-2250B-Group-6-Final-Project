using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{
    public static ScrollManager Instance { get; private set; }

    [SerializeField] private int requiredScrolls = 3;
    [SerializeField] private GameObject scrollTextObject;
    [SerializeField] private string scrollTextTag = "ScrollTXT";

    public int CurrentScrolls { get; private set; }
    public int RequiredScrolls => requiredScrolls;

    private TMP_Text scrollText;
    private Text legacyScrollText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CacheScrollText();
        RefreshScrollText();
    }

    public void AddScroll(int amount = 1)
    {
        CurrentScrolls += amount;
        CurrentScrolls = Mathf.Max(0, CurrentScrolls);

        RefreshScrollText();
        Debug.Log($"[ScrollManager] Scrolls: {CurrentScrolls}/{requiredScrolls}");
    }

    public bool HasEnoughScrolls()
    {
        return CurrentScrolls >= requiredScrolls;
    }

    public void ResetScrolls()
    {
        CurrentScrolls = 0;
        RefreshScrollText();
    }

    private void CacheScrollText()
    {
        GameObject targetObject = scrollTextObject;

        if (targetObject == null)
        {
            try
            {
                targetObject = GameObject.FindGameObjectWithTag(scrollTextTag);
            }
            catch (UnityException)
            {
                Debug.LogWarning($"[ScrollManager] Tag '{scrollTextTag}' is not defined. Assign the text object directly in the Inspector or create that tag.");
                return;
            }
        }

        if (targetObject == null)
        {
            Debug.LogWarning($"[ScrollManager] No object found for the scroll text. Assign it in the Inspector or use tag '{scrollTextTag}'.");
            return;
        }

        scrollText = targetObject.GetComponent<TMP_Text>();
        if (scrollText == null)
            legacyScrollText = targetObject.GetComponent<Text>();

        if (scrollText == null && legacyScrollText == null)
        {
            scrollText = targetObject.GetComponentInChildren<TMP_Text>(true);
            if (scrollText == null)
                legacyScrollText = targetObject.GetComponentInChildren<Text>(true);
        }
    }

    private void RefreshScrollText()
    {
        if (scrollText == null && legacyScrollText == null)
            CacheScrollText();

        string displayText = $"Scrolls Collected: {CurrentScrolls}/{requiredScrolls}";

        if (scrollText != null)
            scrollText.text = displayText;

        if (legacyScrollText != null)
            legacyScrollText.text = displayText;
    }
}
