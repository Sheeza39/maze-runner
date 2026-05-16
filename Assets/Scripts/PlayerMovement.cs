using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public float speed = 8f;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true; 
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(x, 0, z).normalized;

        if (moveDirection.magnitude > 0.1f)
{
    // Change the 2f to 0.2f so the target stays right in front of his nose
    Vector3 targetPosition = transform.position + moveDirection * 0.2f; 
    agent.SetDestination(targetPosition);
}
    }

    // THIS HANDLES THE COIN INTERACTION
    private void OnTriggerEnter(Collider other)
    {
        // Debug message to see if the ghost is hitting ANYTHING at all
        Debug.Log("Ghost touched an object named: " + other.gameObject.name + " with Tag: " + other.gameObject.tag);

        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            
            // This will show up bright and clear in your Console tab!
            Debug.Log("⭐ SUCCESS: Coin collected successfully! ⭐");
        }
    }
}