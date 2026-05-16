using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float speed = 5f;

    // FIX: Added the missing score variable definition here!
    private int score = 0; 

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        
        agent.updateRotation = true; 
        agent.speed = speed;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
       float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(x, 0, z).normalized;

        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 targetPosition = transform.position + moveDirection * 0.2f; 
            agent.SetDestination(targetPosition);
        }
    }

    // 1. VICTORY CONDITION: Trigger sensor for coins
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            score += 10;
            Debug.Log("⭐ SUCCESS: Coin collected! +10 Points. Total Score: " + score + " ⭐");
            
            Destroy(other.gameObject); 

            if (score >= 400)
            {
                Debug.Log("🎉 VICTORY! You successfully collected 40 coins! You win the maze! 🎉");
                Time.timeScale = 0f; // Freezes the game
            }
        }
        
        // 2. DEFEAT CONDITION: If the enemy enters the Ghost's trigger space
        if (other.gameObject.name == "Mouse Character" || other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("💀 GAME OVER: The enemy caught you! Player Died. 💀");
            Destroy(gameObject); // Destroys the Ghost
            Time.timeScale = 0f; // Freezes the game
        }
    }
}