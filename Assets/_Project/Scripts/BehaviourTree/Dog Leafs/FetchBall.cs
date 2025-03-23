using BlackboardSystem;
using UnityEngine;
using UnityEngine.AI;

public class FetchBall : Node
{
    private readonly Dog dog;
    private readonly Transform playerTransform;
    private readonly Transform objectGrabPoint;
    private readonly NavMeshAgent dogAgent;
    private readonly Animator animator;
    private readonly GameObject ball;
    private readonly Blackboard blackboard;
    private readonly BlackboardKey ballThrownKey;
    private readonly BlackboardKey calledDogKey;
    private readonly float pickupRange;
    private readonly float dropRange;

    private enum State
    {
        WaitingForThrow,
        CloseUpToPlayer,
        MovingToBall,
        PickingUpBall,
        ReturningToPlayer,
        DroppingBall
    }

    private State currentState;

    public FetchBall(Dog dog, Transform playerTransform, Transform objectGrabPoint,
        NavMeshAgent dogAgent, GameObject ball, Blackboard blackboard, BlackboardKey ballThrownKey,
        BlackboardKey calledDogKey, float pickupRange = 0.1f, float dropRange = 1f) {
        this.dog = dog;
        this.playerTransform = playerTransform;
        this.objectGrabPoint = objectGrabPoint;
        this.dogAgent = dogAgent;
        this.ball = ball;
        this.blackboard = blackboard;
        this.ballThrownKey = ballThrownKey;
        this.calledDogKey = calledDogKey;
        this.pickupRange = pickupRange;
        this.dropRange = dropRange;
    }

    public override NodeState Run() {
        switch (currentState) {
            case State.WaitingForThrow:
                return WaitForBallThrow();

            case State.CloseUpToPlayer:
                return CloseUpToPlayer();

            case State.MovingToBall:
                return MoveToBall();

            case State.PickingUpBall:
                return PickUpBall();

            case State.ReturningToPlayer:
                return ReturnToPlayer();

            case State.DroppingBall:
                return DropBall();

            default:
                return NodeState.Failure;
        }
    }

    private NodeState WaitForBallThrow() {
        if (blackboard.TryGetValue(calledDogKey, out bool calledDog) && !calledDog) {
            return NodeState.Failure;
        }

        dogAgent.stoppingDistance = 3f;
        if (Vector3.Distance(dog.transform.position, playerTransform.position) > dogAgent.stoppingDistance) {
            currentState = State.CloseUpToPlayer;
        }

        if (blackboard.TryGetValue(ballThrownKey, out bool ballThrown) && ballThrown) {
            currentState = State.MovingToBall;
            blackboard.SetValue(ballThrownKey, false);
            dog.nextState = AnimationStates.Run;
            return NodeState.Running;
        }

        dog.nextState = AnimationStates.Idle;
        return NodeState.Running;
    }
    
    private NodeState CloseUpToPlayer() {
        dogAgent.SetDestination(playerTransform.position);
        dog.nextState = AnimationStates.Walk;

        if (Vector3.Distance(dog.transform.position, playerTransform.position) <= dogAgent.stoppingDistance)
            currentState = State.WaitingForThrow;

        if (blackboard.TryGetValue(ballThrownKey, out bool ballThrown) && ballThrown) {
            currentState = State.MovingToBall;
            blackboard.SetValue(ballThrownKey, false);
            dog.nextState = AnimationStates.Run;
        }

        return NodeState.Running;
    }

    private NodeState MoveToBall() {
        dogAgent.SetDestination(ball.transform.position);
        dogAgent.stoppingDistance = pickupRange - 0.1f;

        if (Vector3.Distance(dog.transform.position, ball.transform.position) <= pickupRange) {
            currentState = State.PickingUpBall;
        }

        dog.nextState = AnimationStates.Run;
        return NodeState.Running;
    }

    private NodeState PickUpBall() {
        dog.nextState = AnimationStates.PickUp;

        if (ball.TryGetComponent(out GrabbableObject grabbableObject)) {
            grabbableObject.Grab(objectGrabPoint);
            currentState = State.ReturningToPlayer;
        }


        return NodeState.Running;
    }

    private NodeState ReturnToPlayer() {
        dogAgent.SetDestination(playerTransform.position);
        dogAgent.stoppingDistance = dropRange - 0.2f;

        if (Vector3.Distance(dog.transform.position, playerTransform.position) <= dropRange) {
            currentState = State.DroppingBall;
        }

        dog.nextState = AnimationStates.Trot;
        return NodeState.Running;
    }

    private NodeState DropBall() {
        dog.nextState = AnimationStates.Drop;
        if (ball.TryGetComponent(out GrabbableObject grabbableObject)) {
            grabbableObject.Drop();
            currentState = State.WaitingForThrow;
        }
        else {
            Debug.LogError("GrabbableObject-Komponente fehlt! Ball konnte nicht abgelegt werden.");
        }

        blackboard.SetValue(ballThrownKey, false);

        return NodeState.Running;
    }
}