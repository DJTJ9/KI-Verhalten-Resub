using UnityEngine;
using UnityEngine.AI;

public class SideDogExplore : Node
{
    private readonly SideDog sideDog;
    private readonly Transform dog;
    private readonly NavMeshAgent agent;
    private readonly float speed;
    private readonly float exploreRadius;
    private readonly float dogDetectionRadius;
    
    public SideDogExplore(SideDog sideDog,Transform dog, NavMeshAgent agent, float speed = 2f, float exploreRadius = 10f, float dogDetectionRadius = 5f) {
        this.sideDog = sideDog;
        this.dog = dog;
        this.agent = agent;
        this.speed = speed;
        this.exploreRadius = exploreRadius;
    }

    public override NodeState Run() {
        agent.speed = speed;
        
        if (!agent.pathPending && HasReachedDestination()) {
            SetNewDestination();
        }

        sideDog.nextState = AnimationStates.Walk;
        return NodeState.Running;
    }

    private void SetNewDestination() {
        var randomDirection = Random.insideUnitSphere * exploreRadius;
        randomDirection += dog.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, exploreRadius, NavMesh.AllAreas)) {
            agent.SetDestination(hit.position);
        }
    }

    private bool HasReachedDestination() {
        return agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f);
    }

    public override void Reset() {
        agent.ResetPath();
    }
}