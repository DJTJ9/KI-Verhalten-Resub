using UnityEngine;
using UnityEngine.AI;

public class MoveToTarget : Node
{
    readonly Transform dog;
    readonly Transform target;
    readonly NavMeshAgent agent;
    readonly float speed;
    readonly float reachDistance;
    
    bool hasLookedAtTarget;

    public MoveToTarget(Transform dog, Transform target, NavMeshAgent agent, float speed, float reachDistance = 2f)
    {
        this.dog = dog;
        this.target = target;
        this.agent = agent;
        this.speed = speed;
        this.reachDistance = reachDistance;
    }

    public override NodeState Run()
    {
        if (Vector3.Distance(dog.position, target.position) < reachDistance) {
            agent.ResetPath();
            agent.isStopped = true;

            if (!hasLookedAtTarget) {
                dog.LookAt(target);
                hasLookedAtTarget = true;
            }

            return NodeState.Success;
        }

        agent.speed = speed;
        agent.stoppingDistance = reachDistance;
        agent.SetDestination(target.position);
        dog.LookAt(target);

        if (agent.pathPending) {
            hasLookedAtTarget = false; 
        }

        return NodeState.Running;
    }
}
