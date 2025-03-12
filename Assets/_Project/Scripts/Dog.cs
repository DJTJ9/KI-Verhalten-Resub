using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Dog : MonoBehaviour
{
    public Transform Player;
    public float DetectionRange = 5f;
    public float Speed = 2f;
    public Vector3[] patrolPoints;

    private Node behaviourTree;

    void Start()
    {
        // Baue den Baum
        Node checkTargetInRange = new TargetInRangeCheck(transform, Player, DetectionRange);
        Node moveToTarget = new MoveToTarget(transform, Player, Speed);
        Node chaseSequence = new Sequence(new List<Node> { checkTargetInRange, moveToTarget });

        // Node patrol = new Patrol(transform, patrolPoints, speed);

        behaviourTree = new Selector(new List<Node> { chaseSequence});
    }

    void Update()
    {
        behaviourTree.Run();
    }
}
