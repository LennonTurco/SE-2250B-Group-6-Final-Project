using UnityEngine;

public class PlayerBurnVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer overlayRenderer;
    [SerializeField] private Sprite[] burnFrames;
    [SerializeField] private float frameRate = 10f;

    private bool isBurning;
    private float animationTimer;
    private int animationFrame;

    private void Awake()
    {
        if (overlayRenderer == null)
        {
            overlayRenderer = GetComponent<SpriteRenderer>();
        }

        HideBurning();
    }

    private void Update()
    {
        if (!isBurning || overlayRenderer == null || burnFrames == null || burnFrames.Length == 0)
        {
            return;
        }

        animationTimer += Time.deltaTime;
        if (animationTimer >= 1f / frameRate)
        {
            animationTimer -= 1f / frameRate;
            animationFrame = (animationFrame + 1) % burnFrames.Length;
            overlayRenderer.sprite = burnFrames[animationFrame];
        }
    }

    public void ShowBurning()
    {
        gameObject.SetActive(true);

        if (overlayRenderer == null)
        {
            return;
        }

        isBurning = true;
        animationTimer = 0f;
        animationFrame = 0;

        if (burnFrames != null && burnFrames.Length > 0)
        {
            overlayRenderer.sprite = burnFrames[animationFrame];
        }

        overlayRenderer.enabled = true;
    }

    public void HideBurning()
    {
        isBurning = false;

        if (overlayRenderer != null)
        {
            overlayRenderer.enabled = false;
        }
    }
}
