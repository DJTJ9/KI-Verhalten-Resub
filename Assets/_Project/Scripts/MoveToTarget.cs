using UnityEngine;

public class MoveToTarget : Node
{
    private Transform npc;
    private Transform target;
    private float speed;

    public MoveToTarget(Transform npc, Transform target, float speed)
    {
        this.npc = npc;
        this.target = target;
        this.speed = speed;
    }

    public override NodeState Run()
    {
        npc.position = Vector3.MoveTowards(npc.position, target.position, speed * Time.deltaTime);
        return NodeState.Running;
    }
}
