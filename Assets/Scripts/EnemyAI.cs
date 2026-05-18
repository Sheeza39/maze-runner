using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private Transform playerTransform;
    private Animator anim;
private NavMeshAgent agent;
    // Adjust this number to change how close the player must get to trigger the chase
    public float detectionRadius = 10f; 

    void Start()
{
    anim = GetComponent<Animator>();
    enemyAgent = GetComponent<NavMeshAgent>();
    
    // SPEED TWEAK: Set the enemy speed to be slower than the player
    // If your player speed is 5, set this to 3.5 or 4.
    enemyAgent.speed = 2f; 

    GameObject player = GameObject.Find("Ghost");
    if (player != null)
    {
        playerTransform = player.transform;
    }
}

void Update()
{
    if (playerTransform != null && enemyAgent != null)
    {
        // REMOVED: The distance check. 
        // Now the enemy constantly updates its path to the Ghost's position.
        enemyAgent.SetDestination(playerTransform.position);
    }

    // Animation logic remains the same
    bool isMoving = enemyAgent.velocity.magnitude > 0.1f;
    anim.SetBool("isMoving", isMoving);
}

    // DEFEAT CONDITION
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ghost")
        {
            Debug.Log("💀 GAME OVER: The enemy caught you! Player Died. 💀");
            Destroy(collision.gameObject);
            Time.timeScale = 0f; 
        }
    }

    // This lets you visually see the detection radius circle in the Scene view!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}