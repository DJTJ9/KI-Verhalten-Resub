using BlackboardSystem;
using UnityEngine;
using UnityEngine.AI;

public class PlayWithOtherDog : Node
{
    private static float cooldownTimer;

    private const float cooldownDuration = 15f;

    private readonly Dog dog;
    private readonly NavMeshAgent dogAgent;
    private readonly NavMeshAgent otherDogAgent;
    private readonly Blackboard blackboard = BlackboardManager.SharedBlackboard;
    private readonly BlackboardKey dogCalledKey;
    private readonly BlackboardKey invitedToPlayKey;
    private readonly float playRange;
    private readonly float speed;
    private readonly int numberOfNewDestinations;

    private int currentDestinationCount = 0;

    public PlayWithOtherDog(Dog dog, NavMeshAgent dogAgent, NavMeshAgent otherDogAgent,
        BlackboardKey invitedToPlayKey, float speed = 5f, float playRange = 15f, int numberOfNewDestinations = 5) {
        this.dog = dog;
        this.dogAgent = dogAgent;
        this.otherDogAgent = otherDogAgent;
        this.invitedToPlayKey = invitedToPlayKey;
        this.speed = speed;
        this.playRange = playRange;
        this.numberOfNewDestinations = numberOfNewDestinations;
    }

    public override NodeState Run() {
        Debug.Log("PlayWithOtherDog");
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0) {
            return NodeState.Failure;
        }

        dogAgent.speed = speed;
        otherDogAgent.speed = speed;

        if (blackboard.TryGetValue(invitedToPlayKey, out bool invitedToPlay)) {
            blackboard.SetValue(invitedToPlayKey, true);
        }

        if (currentDestinationCount >= numberOfNewDestinations) {
            if (blackboard.TryGetValue(invitedToPlayKey, out invitedToPlay)) {
                blackboard.SetValue(invitedToPlayKey, false);
            }

            Debug.Log($"InvitedToPlay: {invitedToPlay}");
            StartCooldown();
            return NodeState.Success;
        }

        if (!dogAgent.pathPending && HasReachedDestination(dogAgent)) {
            SetRandomDestination();
            Debug.Log($"CurrentDestinationCount: {currentDestinationCount}");
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

    private static void StartCooldown() {
        cooldownTimer = cooldownDuration;
    }

    public override void Reset() {
        dogAgent.ResetPath();
        otherDogAgent.ResetPath();
        currentDestinationCount = 0;
        cooldownTimer = 0f;
    }
}