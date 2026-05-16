using UnityEngine;
using UnityEngine.AI; // Required for NavMesh

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player; // This will be the Ghost

    void Update()
    {
        // Set the Mousey's destination to the Ghost's position every frame
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }
}