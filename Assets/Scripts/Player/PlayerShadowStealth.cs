using UnityEngine;

public class PlayerShadowStealth : MonoBehaviour
{
    public static PlayerShadowStealth Instance { get; private set; }

    [Header("Shadow Visuals")]
    [SerializeField] private SpriteRenderer[] targetRenderers;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hiddenColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    private int shadowZoneCount;

    public bool IsHiddenInShadow => shadowZoneCount > 0;

    private void Awake()
    {
        Instance = this;

        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            targetRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        }

        ApplyVisualState();
    }

    public void EnterShadow()
    {
        shadowZoneCount++;
        ApplyVisualState();
    }

    public void ExitShadow()
    {
        shadowZoneCount = Mathf.Max(0, shadowZoneCount - 1);
        ApplyVisualState();
    }

    private void ApplyVisualState()
    {
        Color nextColor = IsHiddenInShadow ? hiddenColor : normalColor;

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null)
            {
                targetRenderers[i].color = nextColor;
            }
        }
    }
}
