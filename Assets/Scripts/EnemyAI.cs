using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent enemyAgent;
    private Transform playerTransform;
    private Animator anim;
private NavMeshAgent agent;
    // Adjust this number to change how close the player must get to trigger the chase
    public float detectionRadius = 8f; 

    void Start()
    {
        anim = GetComponent<Animator>();
    agent = GetComponent<NavMeshAgent>();
        enemyAgent = GetComponent<NavMeshAgent>();
        
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
            // Calculate the exact distance between the Enemy and the Ghost
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // CHASE CONDITION: Only move if the player is inside the detection radius
            if (distanceToPlayer <= detectionRadius)
            {
                enemyAgent.SetDestination(playerTransform.position);
            }
            else
            {
                // If the player escapes outside the radius, the enemy stops chasing
                enemyAgent.ResetPath(); 
            }
        }
        bool isMoving = agent.velocity.magnitude > 0.1f;
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