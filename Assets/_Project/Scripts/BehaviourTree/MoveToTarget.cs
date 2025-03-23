using UnityEngine;
using UnityEngine.AI;

public class MoveToTarget : Node
{
    private readonly Dog dog;
    private readonly Transform target;
    private readonly NavMeshAgent agent;
    private readonly float speed;
    private readonly float reachDistance;
    
    private bool hasLookedAtTarget;

    public MoveToTarget(Dog dog, Transform target, NavMeshAgent agent, float speed = 3f, float reachDistance = 2f)
    {
        this.dog = dog;
        this.target = target;
        this.agent = agent;
        this.speed = speed;
        this.reachDistance = reachDistance;
    }

    public override NodeState Run()
    {
        agent.speed = speed;
        agent.stoppingDistance = reachDistance;
        
        if (Vector3.Distance(dog.transform.position, target.position) < reachDistance) {
            agent.ResetPath();
            // agent.isStopped = true;
            Reset();

            if (!hasLookedAtTarget) {
                dog.transform.LookAt(target);
                hasLookedAtTarget = true;
            }

            dog.nextState = AnimationStates.Idle;
            return NodeState.Success;
        }

        agent.SetDestination(target.position);
        dog.transform.LookAt(target);

        if (agent.pathPending) {
            hasLookedAtTarget = false; 
        }

        dog.nextState = AnimationStates.Walk;
        return NodeState.Running;
    }
}
