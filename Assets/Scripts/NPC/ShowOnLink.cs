using UnityEngine;
using UnityEngine.AI;

public class SlowOnLink : MonoBehaviour
{
    private NavMeshAgent agent;
    private float normalSpeed;
    public float slowSpeed = 1.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        normalSpeed = agent.speed;
    }

    void Update()
    {
        if (agent.isOnOffMeshLink)
        {
            agent.speed = slowSpeed;
        }
        else
        {
            agent.speed = normalSpeed;
        }
    }
}
