using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    // A static variable keeps track of the score across the entire game
    public static int score = 0; 

    // This handles the coin interaction on its own file
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object the Ghost ran into is tagged as a Coin
        if (other.gameObject.CompareTag("Coin"))
        {
            // 1. Add 10 points to the total score
            score += 10;

            // 2. Print the updated score to the console bright and clear
            Debug.Log("⭐ SUCCESS: Coin collected! +10 Points. Total Score: " + score + " ⭐");

            // 3. Make the coin vanish instantly
            Destroy(other.gameObject);
        }
    }
}