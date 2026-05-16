using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // Drag your Ghost here
    public Vector3 offset = new Vector3(0f, 6f, -6f); // Default position behind/above
    public float smoothSpeed = 5f;    
    public LayerMask wallLayer;       // Optional: To specify what counts as a wall

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculate the ideal position for the camera
        Vector3 desiredPosition = target.position + offset;
        Vector3 targetDirection = desiredPosition - target.position;

        // 2. Shoot an invisible line (Raycast) from the Ghost to the Camera position
        RaycastHit hit;
        if (Physics.Raycast(target.position, targetDirection.normalized, out hit, targetDirection.magnitude))
        {
            // If the ray hits a wall, bring the camera right in front of the wall
            desiredPosition = hit.point - targetDirection.normalized * 0.2f;
        }

        // 3. Smoothly move to the final safe position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 4. Always keep eyes locked on the ghost
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}