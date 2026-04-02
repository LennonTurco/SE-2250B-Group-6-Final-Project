using UnityEngine;

public class ScrollManager : MonoBehaviour
{
    public static ScrollManager Instance { get; private set; }

    [SerializeField] private int requiredScrolls = 3;

    public int CurrentScrolls { get; private set; }
    public int RequiredScrolls => requiredScrolls;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddScroll(int amount = 1)
    {
        CurrentScrolls += amount;
        CurrentScrolls = Mathf.Max(0, CurrentScrolls);

        Debug.Log($"[ScrollManager] Scrolls: {CurrentScrolls}/{requiredScrolls}");
    }

    public bool HasEnoughScrolls()
    {
        return CurrentScrolls >= requiredScrolls;
    }

    public void ResetScrolls()
    {
        CurrentScrolls = 0;
    }
}
