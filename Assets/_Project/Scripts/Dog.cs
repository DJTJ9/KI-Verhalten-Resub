using System.Collections.Generic;
using BlackboardSystem;
using UnityEngine;
using UnityEngine.AI;

public class Dog : MonoBehaviour
{
    [Header("Dog Settings")] [SerializeField]
    float detectionRange = 5f;

    [SerializeField] private float speed = 2f;
    [SerializeField] private float fastSpeed = 5f;
    [SerializeField] private float dogDetectionRange = 7f;

    [Header("Transforms")] 
    [SerializeField] private Transform playerPos;
    [SerializeField] private List<Transform> bowls;
    [SerializeField] private Transform objectGrabPoint;
    [SerializeField] private GameObject ball;

    [Header("Other Dog")] 
    [SerializeField] private Transform otherDogTransform;
    [SerializeField] private NavMeshAgent otherDogAgent;

    [Header("Blackboard Settings")] 
    [SerializeField] private BlackboardData blackboardData;

    private NavMeshAgent agent;
    private Animator animator;
    private Blackboard blackboard;

    private BlackboardKey playerPosKey;
    private BlackboardKey dogCalledKey;
    private BlackboardKey ballThrownKey;
    private BlackboardKey invitedToPlayKey;

    private Node behaviourTree;

    private AnimationStates currentState { get; set;}
    public AnimationStates nextState { get; set;}

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        blackboard = BlackboardManager.SharedBlackboard;
        blackboardData.SetValuesOnBlackboard(blackboard);
        SetBlackboardKeys();

        nextState = AnimationStates.Walk;

        behaviourTree = new Selector();

        Node runToOwnerSequence = new Sequence();
        bool DogCalled() {
            if (blackboard.TryGetValue(dogCalledKey, out bool dogCalled)) {
                if (!dogCalled) {
                    runToOwnerSequence.Reset();
                    return false;
                }
            }

            return true;
        }
        runToOwnerSequence.Add(new Condition(DogCalled));
        runToOwnerSequence.Add(new MoveToTarget(this, playerPos, agent, fastSpeed));

        Node fetchBall = new Sequence();
        bool BallThrown() {
            if (blackboard.TryGetValue(ballThrownKey, out bool ballThrown)) {
                if (!ballThrown) {
                    fetchBall.Reset();
                    return false;
                }
            }

            return true;
        }
        fetchBall.Add(new Condition(BallThrown));
        fetchBall.Add(new FetchBall(this, playerPos, objectGrabPoint, agent, ball, blackboard, ballThrownKey,
            dogCalledKey, 0.5f));

        Node explore = new Sequence();
        explore.Add(new Explore(this, agent, bowls, speed, detectionRange));

        Node playWithOtherDog = new Sequence();
        playWithOtherDog.Add(new CooldownCondition(20f));
        playWithOtherDog.Add(new TargetInRangeCheck(transform, otherDogTransform, dogDetectionRange));
        playWithOtherDog.Add(new PlayWithOtherDog(this, agent, otherDogAgent, invitedToPlayKey, fastSpeed));

        behaviourTree.Add(playWithOtherDog);
        behaviourTree.Add(fetchBall);
        behaviourTree.Add(runToOwnerSequence);
        behaviourTree.Add(explore);
    }

    void Update() {
        UpdateBlackboardValues();
        behaviourTree.Run();

        CallDog();
        DebugBlackboard();
    }

    private void LateUpdate() {
        if (currentState != nextState) {
            switch (nextState) {
                case AnimationStates.Idle:
                    animator.SetTrigger("Idle");
                    break;
                case AnimationStates.Walk:
                    animator.SetTrigger("Walk");
                    break;
                case AnimationStates.Run:
                    animator.SetTrigger("Run");
                    break;
                case AnimationStates.PickUp:
                    animator.SetTrigger("Pick_Up");
                    break;
                case AnimationStates.Drop:
                    animator.SetTrigger("Drop");
                    break;
                case AnimationStates.Sniff:
                    animator.SetTrigger("Sniff");
                    break;
                
                case AnimationStates.Trot:
                    animator.SetTrigger("Trot");
                    break;
            }
        }

        currentState = nextState;
    }

    private void SetBlackboardKeys() {
        playerPosKey = blackboard.GetOrRegisterKey("PlayerPos");
        dogCalledKey = blackboard.GetOrRegisterKey("DogCalled");
        ballThrownKey = blackboard.GetOrRegisterKey("BallThrown");
        invitedToPlayKey = blackboard.GetOrRegisterKey("InvitedToPlay");
    }

    private void UpdateBlackboardValues() {
        blackboard.SetValue(playerPosKey, playerPos.position);
    }

    private void CallDog() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (blackboard.TryGetValue(dogCalledKey, out bool calledDog)) {
                blackboard.SetValue(dogCalledKey, !calledDog);
            }
        }
    }

    private void DebugBlackboard() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            blackboard.Debug();
        }
    }
}

public enum AnimationStates
{
    Idle,
    Walk,
    Run,
    PickUp,
    Drop,
    Sniff,
    Trot
}