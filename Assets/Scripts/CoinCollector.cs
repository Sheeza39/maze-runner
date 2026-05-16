using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 1. Verify if the object touching the coin is your Ghost player
        if (other.gameObject.CompareTag("Player") || other.gameObject.name == "Ghost")
        {
            // 2. Fetch the script component directly from the Ghost object
            PlayerMovement playerScript = other.gameObject.GetComponent<PlayerMovement>();

            if (playerScript != null)
            {
                // Send score and UI update markers to the master movement script
                playerScript.OnCoinCollected(); 
                
                // 📢 Logs the exact unique clone name to the console!
                Debug.Log($"⭐ SUCCESS: {gameObject.name} collected! +10 Points. ⭐");
                
                // 3. Make this coin instance disappear instantly
                Destroy(gameObject); 
            }
        }
    }
}