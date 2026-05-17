using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 1. ONLY proceed if the object hitting the coin is tagged "Player"
        // This ignores CameraBounds, Walls, and everything else.
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerScript = other.GetComponentInParent<PlayerMovement>();

            if (playerScript != null)
            {
                playerScript.OnCoinCollected(); 
                Debug.Log($"⭐ SUCCESS: {gameObject.name} collected! ⭐");
                Destroy(gameObject); 
            }
        }
        else 
        {
            // This is just for your own debugging; you can remove it later
            // It helps confirm that non-player objects are being ignored.
            Debug.Log($"Coin touched by {other.name}, but we are ignoring it because it's not the Player.");
        }
    }
}