using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // inspector
    public Transform target;
    public float smoothSpeed = 8f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public int pixelsPerUnit = 16;

    [Header("Level Bounds")]
    public float minX, maxX, minY, maxY;

    // private fields
    private float ppu;
    private Camera cam;

    void Start()
    {
        ppu = pixelsPerUnit;
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // lerp toward target
        Vector3 desired = target.position + offset;
        Vector3 smoothed = target.position + offset;

        // clamp so camera stays inside level bounds
        float camH = cam.orthographicSize;
        float camW = cam.orthographicSize * cam.aspect;

        smoothed.x = Mathf.Clamp(smoothed.x, minX + camW, maxX - camW);
        smoothed.y = Mathf.Clamp(smoothed.y, minY + camH, maxY - camH);

        // snap to pixel grid after clamp (fixes tile seams)
        smoothed.x = Mathf.Round(smoothed.x * ppu) / ppu;
        smoothed.y = Mathf.Round(smoothed.y * ppu) / ppu;
        smoothed.z = offset.z;

        transform.position = smoothed;
    }
}