using System;
using BlackboardSystem;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class SideDog : MonoBehaviour
{
    [FormerlySerializedAs("DetectionRange")]
    [Header("Dog Settings")] 
    [SerializeField] private float speed = 2f;
    
    [Header("Other Dog")]
    [SerializeField] private Dog dog;
    [SerializeField] private NavMeshAgent dogAgent;
    
    [Header("Blackboard Settings")] 
    [SerializeField] private BlackboardData blackboardData;
    
    private Blackboard blackboard;
    private NavMeshAgent agent;
    private Animator animator;
    private Node behaviourTree;

    private BlackboardKey invitedToPlayKey;
    
    public AnimationStates currentState;
    public AnimationStates nextState;
    
    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        blackboard = BlackboardManager.SharedBlackboard;
        blackboardData.SetValuesOnBlackboard(blackboard);
        invitedToPlayKey = blackboard.GetOrRegisterKey("InvitedToPlay");

        behaviourTree = new Selector();
        
        Node explore = new Sequence();
        explore.Add(new SideDogExplore(this, transform, agent, speed));
        
        Node playWithDog = new Sequence();
        bool InvitedToPlay() {
            if (blackboard.TryGetValue(invitedToPlayKey, out bool invitedToPlay)) {
                if (!invitedToPlay) {
                    playWithDog.Reset();
                    return false;
                }
            }
        
            return true;
        }
        playWithDog.Add(new Condition(InvitedToPlay));
        playWithDog.Add(new InvitedToPlay(this, dog, agent, blackboard, invitedToPlayKey));

        
        behaviourTree.Add(playWithDog);
        behaviourTree.Add(explore);
    }
    
    private void Update() {
        behaviourTree.Run();
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
            }
        }
        
        currentState = nextState;
    }
}
