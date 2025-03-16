using System.Collections.Generic;
using BlackboardSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Dog : MonoBehaviour
{
    [Header("Dog Settings")] [SerializeField]
    float DetectionRange = 5f;

    [SerializeField] float Speed = 2f;

    [Header("Blackboard Settings")] [SerializeField]
    Transform PlayerPos;

    [SerializeField] BlackboardData BlackboardData;

    private NavMeshAgent agent;

    private Blackboard blackboard = new();

    private BlackboardKey playerPosKey;
    private BlackboardKey dogCalledKey;
    private BlackboardKey ballInHandKey;
    private BlackboardKey ballThrownKey;
    private BlackboardKey foodBowlPosKey;
    private BlackboardKey waterBowlPosKey;

    private Node behaviourTree;

    void Start() {
        agent = GetComponent<NavMeshAgent>();

        BlackboardData.SetValuesOnBlackboard(blackboard);
        SetBlackboardKeys();

        bool DogCalled() {
            if (blackboard.TryGetValue(dogCalledKey, out bool dogCalled)) {
                if (!dogCalled) {
                    return false;
                }
            }

            return true;
        }

        Node dogCalled = new Condition(DogCalled);
        Node runToOwner = new MoveToTarget(transform, PlayerPos, agent, Speed);
        Node runToOwnerSequence = new Sequence(new List<Node> { dogCalled, runToOwner });

        Node checkTargetInRange = new TargetInRangeCheck(transform, PlayerPos, DetectionRange);
        Node moveToTarget = new MoveToTarget(transform, PlayerPos, agent, Speed);
        Node chaseSequence = new Sequence(new List<Node> { checkTargetInRange, moveToTarget });

        behaviourTree = new Selector(new List<Node> { runToOwnerSequence, chaseSequence });
    }

    void Update() {
        UpdateBlackboardValues();
        behaviourTree.Run();

        CallDog();
        DebugBlackboard();
    }

    private void SetBlackboardKeys() {
        playerPosKey = blackboard.GetOrRegisterKey("PlayerPos");
        dogCalledKey = blackboard.GetOrRegisterKey("DogCalled");
        ballInHandKey = blackboard.GetOrRegisterKey("BallInHand");
        ballThrownKey = blackboard.GetOrRegisterKey("BallThrown");
        foodBowlPosKey = blackboard.GetOrRegisterKey("FoodBowlPos");
        waterBowlPosKey = blackboard.GetOrRegisterKey("WaterBowlPos");
    }

    private void UpdateBlackboardValues() {
        blackboard.SetValue(playerPosKey, PlayerPos.position);
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