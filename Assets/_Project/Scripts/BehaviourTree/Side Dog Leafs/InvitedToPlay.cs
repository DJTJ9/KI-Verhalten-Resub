using BlackboardSystem;
using UnityEngine;
using UnityEngine.AI;

public class InvitedToPlay : Node
{
    private readonly SideDog sideDog;
    private readonly Dog dog;
    private readonly NavMeshAgent agent;
    private readonly Blackboard blackboard;
    private readonly BlackboardKey invitedToPlayKey;
    private readonly float stoppingDistance;

    public InvitedToPlay(SideDog sideDog, Dog dog, NavMeshAgent agent, Blackboard blackboard, BlackboardKey invitedToPlayKey, float stoppingDistance = 1f) {
        this.sideDog = sideDog;
        this.dog = dog;
        this.agent = agent;
        this.blackboard = blackboard;
        this.invitedToPlayKey = invitedToPlayKey;
        this.stoppingDistance = stoppingDistance;
    }

    public override NodeState Run() {
        if (blackboard.TryGetValue(invitedToPlayKey, out bool invitedToPlay)) {
            if (invitedToPlay == false)
                return NodeState.Failure;
        }
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(dog.transform.position);
        
        sideDog.nextState = AnimationStates.Run;
        return NodeState.Running;
    }
}