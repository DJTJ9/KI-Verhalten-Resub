using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Explore : Node
{
    private readonly Dog dog;
    private readonly NavMeshAgent agent;
    private readonly List<Transform> bowls;
    private readonly float speed;
    private readonly float sniffDuration;
    private readonly float intervalBetweenSniffing;
    private readonly float exploreRadiusMultiplier;
    private readonly float initialExploreRadius;

    private Transform currentBowlTarget;
    private float exploreRadius;

    private float elapsedTimeSinceLastPause;
    private float pauseEndTime;
    private bool isPausing;
    
    private AnimationStates animationState = AnimationStates.Walk;

    public Explore(Dog dog, NavMeshAgent agent, List<Transform> bowls, float speed = 2f, float exploreRadius = 5f,
        float          sniffDuration = 4.7f, float intervalBetweenSniffing = 10f, float exploreRadiusMultiplier = 2f) {
        this.dog = dog;
        this.agent = agent;
        this.bowls = bowls;
        this.speed = speed;
        this.exploreRadius = exploreRadius;
        initialExploreRadius = exploreRadius;
        this.sniffDuration = sniffDuration;
        this.intervalBetweenSniffing = intervalBetweenSniffing;
        this.exploreRadiusMultiplier = exploreRadiusMultiplier;
    }

    public override NodeState Run() {
        elapsedTimeSinceLastPause += Time.deltaTime;

        foreach (var bowl in bowls) {
            if (!bowl.gameObject.activeSelf) continue;

            float distance = Vector3.Distance(dog.transform.position, bowl.position);
            if (distance <= exploreRadius) {
                elapsedTimeSinceLastPause = 0f;
                currentBowlTarget = bowl;
                agent.SetDestination(bowl.position);
                if (!isPausing && !agent.pathPending && HasReachedDestination()) {
                    dog.nextState = AnimationStates.PickUp;
                    currentBowlTarget.gameObject.SetActive(false);
                    currentBowlTarget = null;
                    SetNewDestination();
                }

                return NodeState.Running;
            }
        }

        // Checkt, ob es Zeit für eine Pause ist
        if (elapsedTimeSinceLastPause >= intervalBetweenSniffing) {
            if (!isPausing) {
                StartPause();
            }

            // Während der Pause nicht weiterlaufen
            if (Time.time >= pauseEndTime) {
                EndPause();
            }
        }

        if (!isPausing && !agent.pathPending && HasReachedDestination()) {
            if (currentBowlTarget != null) {
                currentBowlTarget.gameObject.SetActive(false);
                currentBowlTarget = null;
            }

            SetNewDestination();
        }

        dog.nextState = animationState;
        return NodeState.Running;
    }

    private void StartPause() {
        Debug.Log("Starting pause.");
        isPausing = true;
        pauseEndTime = Time.time + sniffDuration;
        agent.speed = 0f;
        exploreRadius *= exploreRadiusMultiplier;

        animationState = AnimationStates.Sniff;

        foreach (var bowl in bowls) {
            if (!bowl.gameObject.activeSelf) continue;

            float distance = Vector3.Distance(dog.transform.position, bowl.position);
            if (distance <= exploreRadius) {
                EndPause();
                currentBowlTarget = bowl;
                agent.SetDestination(bowl.position);
            }
        }
    }

    private void EndPause() {
        Debug.Log("Ending pause.");
        isPausing = false;
        agent.speed = speed;
        exploreRadius = initialExploreRadius;
        elapsedTimeSinceLastPause = 0f;

        animationState = AnimationStates.Walk;
        SetNewDestination();
    }
    
    private void SetNewDestination() {
        var randomDirection = Random.insideUnitSphere * exploreRadius;
        randomDirection += dog.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, exploreRadius, NavMesh.AllAreas)) {
            agent.SetDestination(hit.position);
        } 
        else {
            Debug.LogWarning("Failed to find a valid NavMesh position.");
        }
    }

    private bool HasReachedDestination() {
        return agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f);
    }

    public override void Reset() {
        isPausing = false;
        agent.isStopped = false;
        elapsedTimeSinceLastPause = 0f;
        agent.ResetPath();
    }
}