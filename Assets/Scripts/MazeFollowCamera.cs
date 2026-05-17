using UnityEngine;

public class MazeFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Position Settings")]
    [SerializeField] private float distance = 2.5f;
    [SerializeField] private float height = 1.2f;
    [SerializeField] private float positionSmoothing = 15.0f; // High value for tight follow
    [SerializeField] private float rotationDamping = 8.0f;

    [Header("Rotation Constraints")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float horizontalLimit = 60f;

    private float currentYaw = 0f;
    private float fixedPitch = 15f;
    private Vector3 targetPosition;

    void LateUpdate()
{
    if (!target) return;

    // 1. Capture Input (No Lerps, just direct values)
    if (Input.GetMouseButton(0))
    {
        currentYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentYaw = Mathf.Clamp(currentYaw, -horizontalLimit, horizontalLimit);
    }
    else
    {
        // Snap back instantly or very fast
        currentYaw = Mathf.MoveTowards(currentYaw, 0, Time.deltaTime * 200f);
    }

    // 2. HARD SNAP position (No SmoothDamp, No Lerp)
    // This forces the camera to the exact coordinate of the player every frame
    float targetYaw = target.eulerAngles.y + currentYaw;
    Quaternion rotation = Quaternion.Euler(15f, targetYaw, 0);
    
    Vector3 positionOffset = rotation * Vector3.back * distance;
    
    // We add the offset directly to the target position
    transform.position = target.position + positionOffset + Vector3.up * height;

    // 3. HARD LOOK
    transform.LookAt(target.position + Vector3.up * 1.2f);
}
}