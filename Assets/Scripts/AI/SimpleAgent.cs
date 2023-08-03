using UnityEngine;
using UnityEngine.AI;

public class SimpleAgent : MonoBehaviour
{
    public Transform targetTransform;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (targetTransform == null)
        {
            Debug.LogError("Target transform is not set!");
            enabled = false;
        }
    }

    private void Update()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        if (agent.enabled && targetTransform != null)
        {
            // Set the destination for the NavMeshAgent to the target transform position
            agent.SetDestination(targetTransform.position);
        }
    }
}
