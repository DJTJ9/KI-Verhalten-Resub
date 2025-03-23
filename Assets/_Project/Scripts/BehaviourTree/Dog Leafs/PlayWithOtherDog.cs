using BlackboardSystem;
using UnityEngine;
using UnityEngine.AI;

public class PlayWithOtherDog : Node
{
    private readonly Dog dog;
    private readonly NavMeshAgent dogAgent;
    private readonly NavMeshAgent otherDogAgent;
    private readonly Blackboard blackboard = BlackboardManager.SharedBlackboard;
    private readonly BlackboardKey dogCalledKey;
    private readonly BlackboardKey invitedToPlayKey;
    private readonly float playRange;
    private readonly float speed;
    private readonly int numberOfNewDestinations;

    private int currentDestinationCount;

    public PlayWithOtherDog(Dog dog, NavMeshAgent dogAgent, NavMeshAgent otherDogAgent,
        BlackboardKey invitedToPlayKey, float speed = 5f, float playRange = 20f, int numberOfNewDestinations = 7) {
        this.dog = dog;
        this.dogAgent = dogAgent;
        this.otherDogAgent = otherDogAgent;
        this.invitedToPlayKey = invitedToPlayKey;
        this.speed = speed;
        this.playRange = playRange;
        this.numberOfNewDestinations = numberOfNewDestinations;
    }

    public override NodeState Run() {
        dogAgent.speed = speed;
        otherDogAgent.speed = speed;

        blackboard.SetValue(invitedToPlayKey, true);

        if (currentDestinationCount >= numberOfNewDestinations) {
            blackboard.SetValue(invitedToPlayKey, false);
            Reset();

            return NodeState.Success;
        }

        if (!dogAgent.pathPending && HasReachedDestination(dogAgent)) {
            SetRandomDestination();
        }

        dog.nextState = AnimationStates.Run;
        return NodeState.Running;
    }

    private void SetRandomDestination() {
        Vector3 randomDirection = Random.insideUnitSphere * playRange;
        randomDirection += dog.transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, playRange, NavMesh.AllAreas)) {
            Vector3 newDestination = hit.position;
            dogAgent.SetDestination(newDestination);
            currentDestinationCount++;
        }
    }

    private bool HasReachedDestination(NavMeshAgent agent, Vector3 targetPosition = default) {
        if (targetPosition != default) {
            float distanceToTarget = Vector3.Distance(agent.transform.position, targetPosition);
            return distanceToTarget <= agent.stoppingDistance;
        }

        return agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f);
    }


    public override void Reset() {
        dogAgent.ResetPath();
        otherDogAgent.ResetPath();
        currentDestinationCount = 0;
    }
}