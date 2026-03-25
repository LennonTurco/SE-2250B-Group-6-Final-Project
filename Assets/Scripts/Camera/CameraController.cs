using UnityEngine;

// attach to Main Camera
public class CameraController : MonoBehaviour
{
    // fixing 
    public Transform target;
    public float smoothSpeed = 8f;   // higher = snappier
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public int pixelsPerUnit = 16;   // match your sprite PPU

    // ── private ────────────────────────────────
    private float ppu;

    void Start()
    {
        ppu = pixelsPerUnit;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. lerp toward target (smooth movement)
        Vector3 desired = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // 2. snap to pixel grid AFTER lerp (no gaps/seams)
        smoothed.x = Mathf.Round(smoothed.x * ppu) / ppu;
        smoothed.y = Mathf.Round(smoothed.y * ppu) / ppu;
        smoothed.z = offset.z; // keep z fixed

        transform.position = smoothed;
    }
}