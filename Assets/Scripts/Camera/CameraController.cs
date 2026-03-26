using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 4f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    [Header("Level Bounds")]
    public float minX, maxX, minY, maxY;
    [Header("Pixel Snapping")]
    public bool usePixelSnap = false;  // toggle in inspector
    public int pixelsPerUnit = 16;
    private Camera cam;
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // lerp toward target
        Vector3 desired = target.position + offset;
        Vector3 smoothed = target.position + offset;

        // clamp to level bounds
        float camH = cam.orthographicSize;
        float camW = cam.orthographicSize * cam.aspect;

        smoothed.x = Mathf.Clamp(smoothed.x, minX + camW, maxX - camW);
        smoothed.y = Mathf.Clamp(smoothed.y, minY + camH, maxY - camH);

        // optional pixel snap
        if (usePixelSnap)
        {
            float ppu = pixelsPerUnit;
            smoothed.x = Mathf.Round(smoothed.x * ppu) / ppu;
            smoothed.y = Mathf.Round(smoothed.y * ppu) / ppu;
        }

        smoothed.z = offset.z;
        transform.position = smoothed;
    }
}