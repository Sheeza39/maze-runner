using UnityEngine;

public class SpinItem : MonoBehaviour
{
    void Update()
    {
        // Spins 100 degrees per second around the Y axis
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }
}