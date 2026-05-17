using UnityEngine;
using Unity.Cinemachine;

namespace RPGGame
{
    public class CameraController : MonoBehaviour
{
    // In Cinemachine 3, the FreeLook component is often referenced as CinemachineCamera
    // or CinemachineFreeLook if you are using the compatibility wrapper.
    [SerializeField] private CinemachineCamera followCamera;

    void Update()
    {
        if (Input.GetMouseButton(1)) // Detect Right Click
        {
            followCamera.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            followCamera.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
}